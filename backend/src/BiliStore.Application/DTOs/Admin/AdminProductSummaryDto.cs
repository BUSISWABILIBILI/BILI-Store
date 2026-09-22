namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminProductSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    decimal Price,
    string ClothingType,
    int EstimatedProductionDays,
    bool IsAvailable,
    bool IsFeatured,
    bool AllowsCustomColor,
    string PrimaryImageUrl,
    IReadOnlyList<string> CategoryNames,
    IReadOnlyList<string> ColourNames,
    IReadOnlyList<string> RequiredMeasurementNames);
