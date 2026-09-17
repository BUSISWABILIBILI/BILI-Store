namespace BiliStore.Application.DTOs.Catalogue;

public sealed record MeasurementTypeDto(
    Guid Id,
    string Name,
    string Key,
    string Unit,
    string Description,
    string Instructions,
    string Tip,
    string? IllustrationUrl,
    int DisplayOrder);
