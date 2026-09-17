namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutValidationIssueDto(
    string Field,
    string Message);
