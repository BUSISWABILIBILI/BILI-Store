namespace BiliStore.Application.DTOs.Customers;

public sealed record CustomerMeasurementProfileDto(
    Guid Id,
    string Name,
    bool IsDefault,
    string? Notes,
    IReadOnlyList<CustomerMeasurementValueDto> Measurements);
