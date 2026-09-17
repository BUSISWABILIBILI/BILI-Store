using BiliStore.Application.Catalogue;
using BiliStore.Application.DTOs.Catalogue;
using BiliStore.Application.DTOs.Measurements;
using BiliStore.Application.Measurements;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/measurements")]
public sealed class MeasurementsController(
    ICatalogueService catalogueService,
    IMeasurementValidationService measurementValidationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MeasurementTypeDto>>> GetMeasurements(CancellationToken cancellationToken)
    {
        var measurements = await catalogueService.GetMeasurementTypesAsync(cancellationToken);

        return Ok(measurements);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<MeasurementValidationResponseDto>> ValidateProductMeasurements(
        ValidateProductMeasurementsRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await measurementValidationService.ValidateProductMeasurementsAsync(request, cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }
}
