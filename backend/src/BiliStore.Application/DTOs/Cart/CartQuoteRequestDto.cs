namespace BiliStore.Application.DTOs.Cart;

public sealed record CartQuoteRequestDto(
    IReadOnlyList<CartQuoteItemRequestDto> Items);
