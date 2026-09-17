namespace BiliStore.Application.DTOs.Cart;

public sealed record CartQuoteItemRequestDto(
    string ProductSlug,
    Guid? SelectedColourId,
    string? CustomColourName,
    string? CustomColourHex,
    int Quantity,
    IReadOnlyList<CartMeasurementInputDto> Measurements);
