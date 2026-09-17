using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICatalogueService catalogueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await catalogueService.GetCategoriesAsync(cancellationToken);

        return Ok(categories);
    }
}
