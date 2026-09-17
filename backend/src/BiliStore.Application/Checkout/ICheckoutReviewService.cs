using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Application.Checkout;

public interface ICheckoutReviewService
{
    Task<CheckoutReviewResponseDto> ReviewAsync(
        CheckoutReviewRequestDto request,
        CancellationToken cancellationToken);
}
