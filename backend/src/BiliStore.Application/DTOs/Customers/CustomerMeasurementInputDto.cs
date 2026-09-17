namespace BiliStore.Application.DTOs.Customers;

public sealed record CustomerMeasurementInputDto(
    string MeasurementKey,
    decimal ValueCm);
