namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderItemMeasurementDto(
    string MeasurementKey,
    string MeasurementName,
    decimal ValueCm);
