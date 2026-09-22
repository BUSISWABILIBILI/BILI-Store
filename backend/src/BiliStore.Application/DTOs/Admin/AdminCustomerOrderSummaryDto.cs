namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminCustomerOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderDate,
    decimal Total,
    string PaymentStatus,
    string OrderStatus,
    int ItemCount);
