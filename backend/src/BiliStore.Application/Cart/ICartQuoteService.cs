using BiliStore.Application.DTOs.Cart;

namespace BiliStore.Application.Cart;

public interface ICartQuoteService
{
    Task<CartQuoteResponseDto> QuoteAsync(CartQuoteRequestDto request, CancellationToken cancellationToken);
}
