namespace BiliStore.Application.DTOs.Customers;

public sealed record CustomerReturnRequestDto(
    Guid Id,
    string OrderNumber,
    string Reason,
    string Details,
    string Status,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ResolvedAt);
