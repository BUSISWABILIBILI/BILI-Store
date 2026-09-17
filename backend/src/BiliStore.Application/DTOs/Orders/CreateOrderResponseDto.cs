using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Application.DTOs.Orders;

public sealed record CreateOrderResponseDto(
    bool CanCreateOrder,
    IReadOnlyList<CheckoutValidationIssueDto> Issues,
    OrderConfirmationDto? Order);
