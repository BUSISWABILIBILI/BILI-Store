namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderConfirmationDto(
    Guid Id,
    string OrderNumber,
    DateTimeOffset OrderDate,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    string PaymentStatus,
    string OrderStatus,
    DateTimeOffset? ReturnPolicyAcceptedAt,
    OrderCustomerDto Customer,
    OrderDeliveryDto Delivery,
    IReadOnlyList<OrderItemDto> Items,
    IReadOnlyList<OrderPaymentDto> Payments);
