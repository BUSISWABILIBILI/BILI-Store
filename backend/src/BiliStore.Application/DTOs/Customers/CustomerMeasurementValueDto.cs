namespace BiliStore.Application.DTOs.Customers;

public sealed record CustomerMeasurementValueDto(
    Guid MeasurementTypeId,
    string MeasurementKey,
    string MeasurementName,
    decimal ValueCm,
    string Unit);
