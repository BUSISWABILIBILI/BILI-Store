namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminCustomerMeasurementProfileDto(
    Guid Id,
    string Name,
    bool IsDefault,
    string? Notes,
    int MeasurementCount,
    IReadOnlyList<AdminCustomerMeasurementValueDto> Measurements);
