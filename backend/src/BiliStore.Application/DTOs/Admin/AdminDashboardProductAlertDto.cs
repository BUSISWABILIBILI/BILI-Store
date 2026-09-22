namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminDashboardProductAlertDto(
    Guid Id,
    string Name,
    string Slug,
    bool IsAvailable,
    bool IsFeatured,
    string Issue);
