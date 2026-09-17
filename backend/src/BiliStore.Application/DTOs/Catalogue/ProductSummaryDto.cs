namespace BiliStore.Application.DTOs.Catalogue;

public sealed record ProductSummaryDto(
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
    IReadOnlyList<CategoryDto> Categories,
    IReadOnlyList<ColourDto> AvailableColours,
    IReadOnlyList<string> RequiredMeasurements);
