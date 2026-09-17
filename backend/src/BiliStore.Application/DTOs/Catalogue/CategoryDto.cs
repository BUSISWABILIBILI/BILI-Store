namespace BiliStore.Application.DTOs.Catalogue;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? ParentSlug,
    int DisplayOrder);
