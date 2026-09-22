namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminDashboardRecentOrderDto(
    string OrderNumber,
    string CustomerName,
    decimal Total,
    string OrderStatus,
    string PaymentStatus,
    DateTimeOffset OrderDate);
