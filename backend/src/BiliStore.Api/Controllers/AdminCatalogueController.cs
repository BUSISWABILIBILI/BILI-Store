using System.Text.RegularExpressions;
using BiliStore.Application.DTOs.Admin;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public sealed partial class AdminCatalogueController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyList<AdminProductSummaryDto>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Include(product => product.ProductCategories)
                .ThenInclude(productCategory => productCategory.Category)
            .Include(product => product.AvailableColours)
                .ThenInclude(productColour => productColour.Colour)
            .Include(product => product.Images)
            .Include(product => product.MeasurementRequirements)
                .ThenInclude(requirement => requirement.MeasurementType)
            .OrderByDescending(product => product.IsAvailable)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);

        return Ok(products.Select(MapProductSummary).ToList());
    }

    [HttpGet("products/{id:guid}")]
    public async Task<ActionResult<AdminProductDetailsDto>> GetProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await LoadProductForAdminAsync(id, cancellationToken);

        return product is null ? NotFound() : Ok(MapProductDetails(product));
    }

    [HttpPost("products")]
    public async Task<ActionResult<AdminProductDetailsDto>> CreateProduct(
        UpsertAdminProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var slug = NormalizeSlug(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);

        if (await ProductSlugExistsAsync(slug, null, cancellationToken))
        {
            return BadRequest(new { errors = new[] { "A product with this slug already exists." } });
        }

        var product = new Product();
        ApplyProductFields(product, request, slug);
        await ApplyProductRelationsAsync(product, request, cancellationToken);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdProduct = await LoadProductForAdminAsync(product.Id, cancellationToken);

        return createdProduct is null ? Problem("Product was created but could not be loaded.") : Ok(MapProductDetails(createdProduct));
    }

    [HttpPut("products/{id:guid}")]
    public async Task<ActionResult<AdminProductDetailsDto>> UpdateProduct(
        Guid id,
        UpsertAdminProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(currentProduct => currentProduct.ProductCategories)
            .Include(currentProduct => currentProduct.AvailableColours)
            .Include(currentProduct => currentProduct.Images)
            .Include(currentProduct => currentProduct.MeasurementRequirements)
            .SingleOrDefaultAsync(currentProduct => currentProduct.Id == id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        var slug = NormalizeSlug(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);

        if (await ProductSlugExistsAsync(slug, id, cancellationToken))
        {
            return BadRequest(new { errors = new[] { "A product with this slug already exists." } });
        }

        ApplyProductFields(product, request, slug);
        product.UpdatedAt = DateTimeOffset.UtcNow;
        await ApplyProductRelationsAsync(product, request, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var updatedProduct = await LoadProductForAdminAsync(product.Id, cancellationToken);

        return updatedProduct is null ? NotFound() : Ok(MapProductDetails(updatedProduct));
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<ActionResult<AdminProductDetailsDto>> DeactivateProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(currentProduct => currentProduct.Id == id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        product.IsAvailable = false;
        product.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var updatedProduct = await LoadProductForAdminAsync(product.Id, cancellationToken);

        return updatedProduct is null ? NotFound() : Ok(MapProductDetails(updatedProduct));
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<AdminCategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Include(category => category.ParentCategory)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .Select(category => new AdminCategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.ParentCategoryId,
                category.ParentCategory == null ? null : category.ParentCategory.Name,
                category.DisplayOrder,
                category.IsActive))
            .ToListAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<AdminCategoryDto>> CreateCategory(
        UpsertAdminCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var slug = NormalizeSlug(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);

        if (await CategorySlugExistsAsync(slug, null, cancellationToken))
        {
            return BadRequest(new { errors = new[] { "A category with this slug already exists." } });
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Slug = slug,
            ParentCategoryId = request.ParentCategoryId,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await MapCategoryAsync(category.Id, cancellationToken));
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<ActionResult<AdminCategoryDto>> UpdateCategory(
        Guid id,
        UpsertAdminCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.SingleOrDefaultAsync(currentCategory => currentCategory.Id == id, cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        var slug = NormalizeSlug(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);

        if (await CategorySlugExistsAsync(slug, id, cancellationToken))
        {
            return BadRequest(new { errors = new[] { "A category with this slug already exists." } });
        }

        category.Name = request.Name.Trim();
        category.Slug = slug;
        category.ParentCategoryId = request.ParentCategoryId == id ? null : request.ParentCategoryId;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await MapCategoryAsync(category.Id, cancellationToken));
    }

    [HttpGet("colours")]
    public async Task<ActionResult<IReadOnlyList<AdminColourDto>>> GetColours(CancellationToken cancellationToken)
    {
        var colours = await dbContext.Colours
            .AsNoTracking()
            .OrderBy(colour => colour.Name)
            .Select(colour => new AdminColourDto(colour.Id, colour.Name, colour.HexCode, colour.IsActive))
            .ToListAsync(cancellationToken);

        return Ok(colours);
    }

    [HttpPost("colours")]
    public async Task<ActionResult<AdminColourDto>> CreateColour(
        UpsertAdminColourRequestDto request,
        CancellationToken cancellationToken)
    {
        var colour = new Colour
        {
            Name = request.Name.Trim(),
            HexCode = request.HexCode.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Colours.Add(colour);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new AdminColourDto(colour.Id, colour.Name, colour.HexCode, colour.IsActive));
    }

    [HttpPut("colours/{id:guid}")]
    public async Task<ActionResult<AdminColourDto>> UpdateColour(
        Guid id,
        UpsertAdminColourRequestDto request,
        CancellationToken cancellationToken)
    {
        var colour = await dbContext.Colours.SingleOrDefaultAsync(currentColour => currentColour.Id == id, cancellationToken);

        if (colour is null)
        {
            return NotFound();
        }

        colour.Name = request.Name.Trim();
        colour.HexCode = request.HexCode.Trim();
        colour.IsActive = request.IsActive;
        colour.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new AdminColourDto(colour.Id, colour.Name, colour.HexCode, colour.IsActive));
    }

    private async Task<Product?> LoadProductForAdminAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(product => product.ProductCategories)
                .ThenInclude(productCategory => productCategory.Category)
            .Include(product => product.AvailableColours)
                .ThenInclude(productColour => productColour.Colour)
            .Include(product => product.Images)
            .Include(product => product.MeasurementRequirements)
                .ThenInclude(requirement => requirement.MeasurementType)
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    private static void ApplyProductFields(Product product, UpsertAdminProductRequestDto request, string slug)
    {
        product.Name = request.Name.Trim();
        product.Slug = slug;
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.ClothingType = request.ClothingType.Trim();
        product.EstimatedProductionDays = Math.Max(1, request.EstimatedProductionDays);
        product.IsAvailable = request.IsAvailable;
        product.IsFeatured = request.IsFeatured;
        product.AllowsCustomColor = request.AllowsCustomColor;
        product.FabricDescription = string.IsNullOrWhiteSpace(request.FabricDescription) ? null : request.FabricDescription.Trim();
    }

    private async Task ApplyProductRelationsAsync(
        Product product,
        UpsertAdminProductRequestDto request,
        CancellationToken cancellationToken)
    {
        product.ProductCategories.Clear();
        product.AvailableColours.Clear();
        product.MeasurementRequirements.Clear();

        foreach (var categoryId in request.CategoryIds?.Distinct() ?? Array.Empty<Guid>())
        {
            if (await dbContext.Categories.AnyAsync(category => category.Id == categoryId, cancellationToken))
            {
                product.ProductCategories.Add(new ProductCategory { ProductId = product.Id, CategoryId = categoryId });
            }
        }

        foreach (var colourId in request.ColourIds?.Distinct() ?? Array.Empty<Guid>())
        {
            if (await dbContext.Colours.AnyAsync(colour => colour.Id == colourId, cancellationToken))
            {
                product.AvailableColours.Add(new ProductColour { ProductId = product.Id, ColourId = colourId });
            }
        }

        foreach (var requirement in request.MeasurementRequirements?.DistinctBy(current => current.MeasurementTypeId) ?? Array.Empty<AdminProductMeasurementRequirementInputDto>())
        {
            if (await dbContext.MeasurementTypes.AnyAsync(measurement => measurement.Id == requirement.MeasurementTypeId, cancellationToken))
            {
                product.MeasurementRequirements.Add(new ProductMeasurementRequirement
                {
                    ProductId = product.Id,
                    MeasurementTypeId = requirement.MeasurementTypeId,
                    IsRequired = requirement.IsRequired,
                    MinValueCm = requirement.MinValueCm,
                    MaxValueCm = requirement.MaxValueCm,
                    HelpText = string.IsNullOrWhiteSpace(requirement.HelpText) ? null : requirement.HelpText.Trim()
                });
            }
        }

        UpsertPrimaryImage(product, request);
    }

    private static void UpsertPrimaryImage(Product product, UpsertAdminProductRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.PrimaryImageUrl))
        {
            return;
        }

        var primaryImage = product.Images.FirstOrDefault(image => image.IsPrimary);

        if (primaryImage is null)
        {
            primaryImage = new ProductImage
            {
                ProductId = product.Id,
                IsPrimary = true
            };

            product.Images.Add(primaryImage);
        }

        foreach (var image in product.Images.Where(image => image.Id != primaryImage.Id))
        {
            image.IsPrimary = false;
        }

        primaryImage.Url = request.PrimaryImageUrl.Trim();
        primaryImage.AltText = string.IsNullOrWhiteSpace(request.PrimaryImageAltText)
            ? product.Name
            : request.PrimaryImageAltText.Trim();
        primaryImage.DisplayOrder = 0;
        primaryImage.IsPrimary = true;
    }

    private async Task<AdminCategoryDto> MapCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Include(category => category.ParentCategory)
            .Where(category => category.Id == id)
            .Select(category => new AdminCategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.ParentCategoryId,
                category.ParentCategory == null ? null : category.ParentCategory.Name,
                category.DisplayOrder,
                category.IsActive))
            .SingleAsync(cancellationToken);
    }

    private async Task<bool> ProductSlugExistsAsync(string slug, Guid? ignoredProductId, CancellationToken cancellationToken)
    {
        return await dbContext.Products.AnyAsync(
            product => product.Slug == slug && product.Id != ignoredProductId,
            cancellationToken);
    }

    private async Task<bool> CategorySlugExistsAsync(string slug, Guid? ignoredCategoryId, CancellationToken cancellationToken)
    {
        return await dbContext.Categories.AnyAsync(
            category => category.Slug == slug && category.Id != ignoredCategoryId,
            cancellationToken);
    }

    private static AdminProductSummaryDto MapProductSummary(Product product)
    {
        var primaryImage = GetPrimaryImage(product);

        return new AdminProductSummaryDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Price,
            product.ClothingType,
            product.EstimatedProductionDays,
            product.IsAvailable,
            product.IsFeatured,
            product.AllowsCustomColor,
            primaryImage.Url,
            product.ProductCategories.Select(productCategory => productCategory.Category.Name).OrderBy(name => name).ToList(),
            product.AvailableColours.Select(productColour => productColour.Colour.Name).OrderBy(name => name).ToList(),
            product.MeasurementRequirements
                .Where(requirement => requirement.IsRequired)
                .Select(requirement => requirement.MeasurementType.Name)
                .OrderBy(name => name)
                .ToList());
    }

    private static AdminProductDetailsDto MapProductDetails(Product product)
    {
        var primaryImage = GetPrimaryImage(product);

        return new AdminProductDetailsDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.Price,
            product.ClothingType,
            product.EstimatedProductionDays,
            product.IsAvailable,
            product.IsFeatured,
            product.AllowsCustomColor,
            product.FabricDescription,
            primaryImage.Url,
            primaryImage.AltText,
            product.ProductCategories.Select(productCategory => productCategory.CategoryId).ToList(),
            product.AvailableColours.Select(productColour => productColour.ColourId).ToList(),
            product.MeasurementRequirements
                .OrderBy(requirement => requirement.MeasurementType.DisplayOrder)
                .Select(requirement => new AdminProductMeasurementRequirementDto(
                    requirement.Id,
                    requirement.MeasurementTypeId,
                    requirement.MeasurementType.Name,
                    requirement.MeasurementType.Key,
                    requirement.IsRequired,
                    requirement.MinValueCm,
                    requirement.MaxValueCm,
                    requirement.HelpText))
                .ToList());
    }

    private static ProductImage GetPrimaryImage(Product product)
    {
        return product.Images
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.DisplayOrder)
            .FirstOrDefault()
            ?? new ProductImage
            {
                Url = "/images/bili-atelier-hero.png",
                AltText = product.Name,
                IsPrimary = true
            };
    }

    private static string NormalizeSlug(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        normalized = WhitespaceRegex().Replace(normalized, "-");
        normalized = UnsafeSlugRegex().Replace(normalized, "");
        normalized = DuplicateDashRegex().Replace(normalized, "-").Trim('-');

        return string.IsNullOrWhiteSpace(normalized) ? $"product-{Guid.NewGuid():N}"[..16] : normalized;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex("[^a-z0-9-]")]
    private static partial Regex UnsafeSlugRegex();

    [GeneratedRegex("-+")]
    private static partial Regex DuplicateDashRegex();
}
