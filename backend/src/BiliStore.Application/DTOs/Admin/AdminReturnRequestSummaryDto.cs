namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminReturnRequestSummaryDto(
    Guid Id,
    string OrderNumber,
    string CustomerEmail,
    string Reason,
    string Details,
    string Status,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ResolvedAt);
