namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminPaymentSummaryDto(
    Guid Id,
    string OrderNumber,
    string CustomerEmail,
    string Provider,
    string? ProviderReference,
    decimal Amount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt);
