namespace BiliStore.Application.DTOs.Measurements;

public sealed record MeasurementValueInputDto(
    string MeasurementKey,
    decimal? ValueCm);
