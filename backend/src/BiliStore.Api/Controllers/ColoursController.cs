using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/colours")]
public sealed class ColoursController(ICatalogueService catalogueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ColourDto>>> GetColours(CancellationToken cancellationToken)
    {
        var colours = await catalogueService.GetColoursAsync(cancellationToken);

        return Ok(colours);
    }
}
