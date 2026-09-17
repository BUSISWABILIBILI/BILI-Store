namespace BiliStore.Application.DTOs.System;

public sealed record ApiInfoResponse(
    string Name,
    string Version,
    string Description,
    string CurrentPhase);

