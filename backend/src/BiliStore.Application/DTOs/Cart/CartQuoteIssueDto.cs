namespace BiliStore.Application.DTOs.Cart;

public sealed record CartQuoteIssueDto(
    string ProductSlug,
    string Message);
