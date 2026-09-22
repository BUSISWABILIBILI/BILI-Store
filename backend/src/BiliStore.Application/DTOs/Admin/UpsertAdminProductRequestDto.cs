namespace BiliStore.Application.DTOs.Admin;

public sealed record UpsertAdminProductRequestDto(
    string Name,
    string? Slug,
    string Description,
    decimal Price,
    string ClothingType,
    int EstimatedProductionDays,
    bool IsAvailable,
    bool IsFeatured,
    bool AllowsCustomColor,
    string? FabricDescription,
    string? PrimaryImageUrl,
    string? PrimaryImageAltText,
    IReadOnlyList<Guid>? CategoryIds,
    IReadOnlyList<Guid>? ColourIds,
    IReadOnlyList<AdminProductMeasurementRequirementInputDto>? MeasurementRequirements);
