namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminCustomerMeasurementValueDto(
    string MeasurementName,
    string MeasurementKey,
    decimal ValueCm,
    string Unit);
