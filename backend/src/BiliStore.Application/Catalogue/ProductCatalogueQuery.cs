namespace BiliStore.Application.Catalogue;

public sealed record ProductCatalogueQuery(
    string? Search,
    string? Category,
    string? ClothingType,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? Featured);
