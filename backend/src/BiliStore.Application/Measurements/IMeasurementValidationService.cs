using BiliStore.Application.DTOs.Measurements;

namespace BiliStore.Application.Measurements;

public interface IMeasurementValidationService
{
    Task<MeasurementValidationResponseDto?> ValidateProductMeasurementsAsync(
        ValidateProductMeasurementsRequestDto request,
        CancellationToken cancellationToken);
}
