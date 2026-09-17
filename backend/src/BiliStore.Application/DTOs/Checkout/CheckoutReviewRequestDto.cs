using BiliStore.Application.DTOs.Cart;

namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutReviewRequestDto(
    CheckoutCustomerDetailsDto? Customer,
    CheckoutDeliveryDetailsDto? Delivery,
    bool ReturnPolicyAccepted,
    CartQuoteRequestDto? Cart);
