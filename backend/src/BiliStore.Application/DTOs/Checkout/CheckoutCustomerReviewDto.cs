namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutCustomerReviewDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone);
