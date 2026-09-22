namespace BiliStore.Application.DTOs.Admin;

public sealed record UpsertAdminColourRequestDto(
    string Name,
    string HexCode,
    bool IsActive);
