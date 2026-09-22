namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminProductMeasurementRequirementDto(
    Guid Id,
    Guid MeasurementTypeId,
    string MeasurementName,
    string MeasurementKey,
    bool IsRequired,
    decimal? MinValueCm,
    decimal? MaxValueCm,
    string? HelpText);
