namespace BiliStore.Application.DTOs.Auth;

public sealed record UpdateCustomerProfileRequestDto(
    string FirstName,
    string LastName,
    string Phone);
