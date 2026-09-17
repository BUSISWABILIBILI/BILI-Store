namespace BiliStore.Application.DTOs.Auth;

public sealed record LoginCustomerRequestDto(
    string Email,
    string Password);
