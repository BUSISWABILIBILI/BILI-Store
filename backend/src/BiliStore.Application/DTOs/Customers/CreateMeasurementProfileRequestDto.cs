namespace BiliStore.Application.DTOs.Customers;

public sealed record CreateMeasurementProfileRequestDto(
    string Name,
    bool IsDefault,
    string? Notes,
    IReadOnlyList<CustomerMeasurementInputDto> Measurements);
