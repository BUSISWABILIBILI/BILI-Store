namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminCustomerSummaryDto(
    Guid Id,
    Guid ApplicationUserId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int MeasurementProfileCount,
    int OrderCount,
    decimal TotalSpent,
    DateTimeOffset? LatestOrderDate);
