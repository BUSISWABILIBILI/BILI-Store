using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Application.DTOs.Payments;

public sealed record CreatePaymentSessionRequestDto(
    CheckoutReviewRequestDto? Checkout,
    string? SuccessUrl,
    string? CancelUrl);
