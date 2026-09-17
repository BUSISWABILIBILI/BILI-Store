namespace BiliStore.Application.DTOs.System;

public sealed record HealthCheckResponse(
    string Service,
    string Status,
    string Environment,
    DateTimeOffset ServerTime);

