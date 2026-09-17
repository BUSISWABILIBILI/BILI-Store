namespace BiliStore.Application.DTOs.Cart;

public sealed record CartMeasurementInputDto(
    string MeasurementKey,
    string MeasurementName,
    decimal ValueCm,
    string Unit);
