namespace BiliStore.Application.DTOs.Cart;

public sealed record CartQuoteResponseDto(
    IReadOnlyList<CartQuoteItemResponseDto> Items,
    IReadOnlyList<CartQuoteIssueDto> Issues,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal GrandTotal,
    string Currency);
