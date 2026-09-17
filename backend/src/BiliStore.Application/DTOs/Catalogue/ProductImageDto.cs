namespace BiliStore.Application.DTOs.Catalogue;

public sealed record ProductImageDto(
    Guid Id,
    string Url,
    string AltText,
    int DisplayOrder,
    bool IsPrimary);
