namespace BiliStore.Application.DTOs.Customers;

public sealed record CustomerOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderDate,
    decimal Total,
    string PaymentStatus,
    string OrderStatus,
    int ItemCount);
