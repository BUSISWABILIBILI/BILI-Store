using BiliStore.Application.Cart;
using BiliStore.Application.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(ICartQuoteService cartQuoteService) : ControllerBase
{
    [HttpPost("quote")]
    public async Task<ActionResult<CartQuoteResponseDto>> QuoteCart(
        CartQuoteRequestDto request,
        CancellationToken cancellationToken)
    {
        var quote = await cartQuoteService.QuoteAsync(request, cancellationToken);

        return Ok(quote);
    }
}
