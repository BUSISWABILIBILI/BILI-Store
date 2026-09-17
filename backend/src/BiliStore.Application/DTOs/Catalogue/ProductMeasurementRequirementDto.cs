namespace BiliStore.Application.DTOs.Catalogue;

public sealed record ProductMeasurementRequirementDto(
    Guid Id,
    Guid MeasurementTypeId,
    string Name,
    string Key,
    string Unit,
    bool IsRequired,
    decimal? MinValueCm,
    decimal? MaxValueCm,
    string? HelpText);
