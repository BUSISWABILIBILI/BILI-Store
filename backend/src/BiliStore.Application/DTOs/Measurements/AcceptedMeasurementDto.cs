namespace BiliStore.Application.DTOs.Measurements;

public sealed record AcceptedMeasurementDto(
    string MeasurementKey,
    string MeasurementName,
    decimal ValueCm,
    string Unit);
