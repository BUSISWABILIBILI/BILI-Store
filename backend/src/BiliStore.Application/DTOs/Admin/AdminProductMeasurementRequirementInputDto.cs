namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminProductMeasurementRequirementInputDto(
    Guid MeasurementTypeId,
    bool IsRequired,
    decimal? MinValueCm,
    decimal? MaxValueCm,
    string? HelpText);
