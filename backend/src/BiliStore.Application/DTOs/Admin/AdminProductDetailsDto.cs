namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminProductDetailsDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string ClothingType,
    int EstimatedProductionDays,
    bool IsAvailable,
    bool IsFeatured,
    bool AllowsCustomColor,
    string? FabricDescription,
    string PrimaryImageUrl,
    string PrimaryImageAltText,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<Guid> ColourIds,
    IReadOnlyList<AdminProductMeasurementRequirementDto> MeasurementRequirements);
