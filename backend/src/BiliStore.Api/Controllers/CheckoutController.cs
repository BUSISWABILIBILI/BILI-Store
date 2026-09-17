using BiliStore.Application.Checkout;
using BiliStore.Application.DTOs.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/checkout")]
public sealed class CheckoutController(ICheckoutReviewService checkoutReviewService) : ControllerBase
{
    [HttpPost("review")]
    public async Task<ActionResult<CheckoutReviewResponseDto>> ReviewCheckout(
        CheckoutReviewRequestDto request,
        CancellationToken cancellationToken)
    {
        var review = await checkoutReviewService.ReviewAsync(request, cancellationToken);

        return Ok(review);
    }
}
