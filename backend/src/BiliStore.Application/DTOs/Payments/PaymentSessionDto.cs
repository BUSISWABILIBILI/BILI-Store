namespace BiliStore.Application.DTOs.Payments;

public sealed record PaymentSessionDto(
    string Provider,
    string ProviderReference,
    decimal Amount,
    string Currency,
    string Status,
    string? PaymentUrl,
    DateTimeOffset ExpiresAt);
