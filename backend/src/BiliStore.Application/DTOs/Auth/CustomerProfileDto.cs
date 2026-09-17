namespace BiliStore.Application.DTOs.Auth;

public sealed record CustomerProfileDto(
    Guid Id,
    Guid ApplicationUserId,
    string FirstName,
    string LastName,
    string Email,
    string Phone);
