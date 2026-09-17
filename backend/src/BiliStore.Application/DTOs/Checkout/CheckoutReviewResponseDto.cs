using BiliStore.Application.DTOs.Cart;

namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutReviewResponseDto(
    bool CanContinue,
    IReadOnlyList<CheckoutValidationIssueDto> Issues,
    CartQuoteResponseDto Quote,
    CheckoutCustomerReviewDto? Customer,
    CheckoutDeliveryReviewDto? Delivery,
    DateTimeOffset? ReturnPolicyAcceptedAt);
