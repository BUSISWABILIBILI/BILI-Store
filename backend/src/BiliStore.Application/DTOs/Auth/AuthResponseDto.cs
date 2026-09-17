namespace BiliStore.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    CustomerProfileDto Customer);
