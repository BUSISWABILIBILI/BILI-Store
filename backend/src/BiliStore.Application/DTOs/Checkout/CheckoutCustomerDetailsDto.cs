namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutCustomerDetailsDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone);
