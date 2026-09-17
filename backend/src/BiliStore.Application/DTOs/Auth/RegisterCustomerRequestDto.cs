namespace BiliStore.Application.DTOs.Auth;

public sealed record RegisterCustomerRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password,
    string ConfirmPassword);
