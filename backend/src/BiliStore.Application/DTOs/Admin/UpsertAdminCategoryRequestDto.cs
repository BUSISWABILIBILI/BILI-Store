namespace BiliStore.Application.DTOs.Admin;

public sealed record UpsertAdminCategoryRequestDto(
    string Name,
    string? Slug,
    Guid? ParentCategoryId,
    int DisplayOrder,
    bool IsActive);
