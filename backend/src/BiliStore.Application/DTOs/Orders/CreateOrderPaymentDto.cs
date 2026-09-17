namespace BiliStore.Application.DTOs.Orders;

public sealed record CreateOrderPaymentDto(
    string Provider,
    string ProviderReference,
    decimal Amount,
    string Currency,
    string? PaymentUrl);
