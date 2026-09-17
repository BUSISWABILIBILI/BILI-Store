using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ICatalogueService catalogueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductSummaryDto>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? clothingType,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? featured,
        CancellationToken cancellationToken)
    {
        var query = new ProductCatalogueQuery(search, category, clothingType, minPrice, maxPrice, featured);
        var products = await catalogueService.GetProductsAsync(query, cancellationToken);

        return Ok(products);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var product = await catalogueService.GetProductBySlugAsync(slug, cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }
}
