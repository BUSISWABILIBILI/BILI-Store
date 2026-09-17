using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Application.DTOs.Payments;

public sealed record CreatePaymentSessionResponseDto(
    bool CanStartPayment,
    IReadOnlyList<CheckoutValidationIssueDto> Issues,
    CheckoutReviewResponseDto CheckoutReview,
    PaymentSessionDto? Payment);
