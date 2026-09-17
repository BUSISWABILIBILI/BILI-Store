namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderItemDto(
    Guid Id,
    string ProductName,
    string ProductDescription,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal,
    string? SelectedColourName,
    string? SelectedColourHex,
    string? CustomColourName,
    string? CustomColourHex,
    int ProductionDaysEstimate,
    IReadOnlyList<OrderItemMeasurementDto> Measurements);
