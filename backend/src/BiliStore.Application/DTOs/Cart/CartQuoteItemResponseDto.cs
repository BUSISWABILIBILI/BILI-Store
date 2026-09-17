namespace BiliStore.Application.DTOs.Cart;

public sealed record CartQuoteItemResponseDto(
    string ProductSlug,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal,
    string? SelectedColourName,
    string? SelectedColourHex,
    string? CustomColourName,
    string? CustomColourHex,
    IReadOnlyList<CartMeasurementInputDto> Measurements);
