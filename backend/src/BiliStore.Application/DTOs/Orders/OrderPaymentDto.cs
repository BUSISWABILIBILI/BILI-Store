namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderPaymentDto(
    string Provider,
    string? ProviderReference,
    decimal Amount,
    string Currency,
    string Status,
    string? PaymentUrl,
    DateTimeOffset CreatedAt);
