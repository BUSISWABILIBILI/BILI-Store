using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/measurements")]
public sealed class MeasurementsController(ICatalogueService catalogueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MeasurementTypeDto>>> GetMeasurements(CancellationToken cancellationToken)
    {
        var measurements = await catalogueService.GetMeasurementTypesAsync(cancellationToken);

        return Ok(measurements);
    }
}
