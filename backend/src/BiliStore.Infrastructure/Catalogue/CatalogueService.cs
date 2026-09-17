using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Catalogue;

internal sealed class CatalogueService(BiliStoreDbContext dbContext) : ICatalogueService
{
    private static readonly string[] CustomerCategoryNames = ["Women", "Men", "Girls", "Boys"];

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Include(category => category.ParentCategory)
            .Where(category => category.IsActive)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.ParentCategory == null ? null : category.ParentCategory.Slug,
                category.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ColourDto>> GetColoursAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Colours
            .AsNoTracking()
            .Where(colour => colour.IsActive)
            .OrderBy(colour => colour.Name)
            .Select(colour => new ColourDto(colour.Id, colour.Name, colour.HexCode))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MeasurementTypeDto>> GetMeasurementTypesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.MeasurementTypes
            .AsNoTracking()
            .OrderBy(measurement => measurement.DisplayOrder)
            .Select(measurement => new MeasurementTypeDto(
                measurement.Id,
                measurement.Name,
                measurement.Key,
                measurement.Unit,
                measurement.Description,
                measurement.Instructions,
                measurement.Tip,
                measurement.IllustrationUrl,
                measurement.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(
        ProductCatalogueQuery query,
        CancellationToken cancellationToken)
    {
        var productsQuery = dbContext.Products
            .AsNoTracking()
            .Include(product => product.ProductCategories)
                .ThenInclude(productCategory => productCategory.Category)
            .Include(product => product.Images)
            .Include(product => product.AvailableColours)
                .ThenInclude(productColour => productColour.Colour)
            .Include(product => product.MeasurementRequirements)
                .ThenInclude(requirement => requirement.MeasurementType)
            .AsQueryable();

        productsQuery = ApplyFilters(productsQuery, query);

        var products = await productsQuery
            .OrderByDescending(product => product.IsFeatured)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);

        return products.Select(MapSummary).ToList();
    }

    public async Task<ProductDetailsDto?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(currentProduct => currentProduct.ProductCategories)
                .ThenInclude(productCategory => productCategory.Category)
            .Include(currentProduct => currentProduct.Images)
            .Include(currentProduct => currentProduct.AvailableColours)
                .ThenInclude(productColour => productColour.Colour)
            .Include(currentProduct => currentProduct.MeasurementRequirements)
                .ThenInclude(requirement => requirement.MeasurementType)
            .SingleOrDefaultAsync(
                currentProduct => currentProduct.Slug == slug && currentProduct.IsAvailable,
                cancellationToken);

        return product is null ? null : MapDetails(product);
    }

    private static IQueryable<Product> ApplyFilters(IQueryable<Product> productsQuery, ProductCatalogueQuery query)
    {
        productsQuery = productsQuery.Where(product => product.IsAvailable);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            productsQuery = productsQuery.Where(product =>
                product.Name.Contains(search) ||
                product.Description.Contains(search) ||
                product.ClothingType.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = Normalize(query.Category);

            productsQuery = productsQuery.Where(product =>
                product.ProductCategories.Any(productCategory =>
                    productCategory.Category.Slug == category ||
                    productCategory.Category.Name.ToLower() == category));
        }

        if (!string.IsNullOrWhiteSpace(query.ClothingType))
        {
            var clothingType = query.ClothingType.Trim();
            productsQuery = productsQuery.Where(product => product.ClothingType.Contains(clothingType));
        }

        if (query.MinPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price >= query.MinPrice);
        }

        if (query.MaxPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price <= query.MaxPrice);
        }

        if (query.Featured is not null)
        {
            productsQuery = productsQuery.Where(product => product.IsFeatured == query.Featured);
        }

        return productsQuery;
    }

    private static ProductSummaryDto MapSummary(Product product)
    {
        var primaryImage = GetPrimaryImage(product);

        return new ProductSummaryDto(
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
            MapCategories(product),
            MapColours(product),
            MapRequiredMeasurements(product));
    }

    private static ProductDetailsDto MapDetails(Product product)
    {
        var primaryImage = GetPrimaryImage(product);

        return new ProductDetailsDto(
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
            MapCategories(product),
            MapColours(product),
            product.Images
                .OrderByDescending(image => image.IsPrimary)
                .ThenBy(image => image.DisplayOrder)
                .Select(image => new ProductImageDto(image.Id, image.Url, image.AltText, image.DisplayOrder, image.IsPrimary))
                .ToList(),
            product.MeasurementRequirements
                .OrderBy(requirement => requirement.MeasurementType.DisplayOrder)
                .Select(requirement => new ProductMeasurementRequirementDto(
                    requirement.Id,
                    requirement.MeasurementTypeId,
                    requirement.MeasurementType.Name,
                    requirement.MeasurementType.Key,
                    requirement.MeasurementType.Unit,
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
                AltText = product.Name
            };
    }

    private static IReadOnlyList<CategoryDto> MapCategories(Product product)
    {
        return product.ProductCategories
            .Select(productCategory => productCategory.Category)
            .OrderBy(category => CustomerCategoryNames.Contains(category.Name) ? 0 : 1)
            .ThenBy(category => category.DisplayOrder)
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.ParentCategory?.Slug,
                category.DisplayOrder))
            .ToList();
    }

    private static IReadOnlyList<ColourDto> MapColours(Product product)
    {
        return product.AvailableColours
            .Select(productColour => productColour.Colour)
            .Where(colour => colour.IsActive)
            .OrderBy(colour => colour.Name)
            .Select(colour => new ColourDto(colour.Id, colour.Name, colour.HexCode))
            .ToList();
    }

    private static IReadOnlyList<string> MapRequiredMeasurements(Product product)
    {
        return product.MeasurementRequirements
            .Where(requirement => requirement.IsRequired)
            .OrderBy(requirement => requirement.MeasurementType.DisplayOrder)
            .Select(requirement => requirement.MeasurementType.Name)
            .ToList();
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(" ", "-");
    }
}
