using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Application.DTOs.Orders;

public sealed record CreateOrderRequestDto(
    CheckoutReviewRequestDto? Checkout,
    CreateOrderPaymentDto? Payment);
