namespace BiliStore.Application.DTOs.Measurements;

public sealed record ValidateProductMeasurementsRequestDto(
    string ProductSlug,
    IReadOnlyList<MeasurementValueInputDto> Measurements);
