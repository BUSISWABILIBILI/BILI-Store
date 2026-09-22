namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderDate,
    string CustomerName,
    string CustomerEmail,
    decimal Total,
    string PaymentStatus,
    string OrderStatus,
    string DeliveryStatus,
    int ItemCount,
    string? LatestPaymentReference);
