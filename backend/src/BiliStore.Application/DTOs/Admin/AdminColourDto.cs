namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminColourDto(
    Guid Id,
    string Name,
    string HexCode,
    bool IsActive);
