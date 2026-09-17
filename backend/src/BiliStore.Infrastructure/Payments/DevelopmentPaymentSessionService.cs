using BiliStore.Application.Checkout;
using BiliStore.Application.DTOs.Checkout;
using BiliStore.Application.DTOs.Payments;
using BiliStore.Application.Payments;
using BiliStore.Domain.Enums;

namespace BiliStore.Infrastructure.Payments;

internal sealed class DevelopmentPaymentSessionService(ICheckoutReviewService checkoutReviewService)
    : IPaymentSessionService
{
    private const string ProviderName = "Development";

    public async Task<CreatePaymentSessionResponseDto> CreateSessionAsync(
        CreatePaymentSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var checkoutRequest = request.Checkout ?? new CheckoutReviewRequestDto(null, null, false, null);
        var checkoutReview = await checkoutReviewService.ReviewAsync(checkoutRequest, cancellationToken);

        if (!checkoutReview.CanContinue)
        {
            return new CreatePaymentSessionResponseDto(
                false,
                checkoutReview.Issues,
                checkoutReview,
                null);
        }

        var providerReference = CreateProviderReference();
        var payment = new PaymentSessionDto(
            ProviderName,
            providerReference,
            checkoutReview.Quote.GrandTotal,
            checkoutReview.Quote.Currency,
            PaymentStatus.Pending.ToString(),
            BuildPaymentUrl(request.SuccessUrl, providerReference),
            DateTimeOffset.UtcNow.AddMinutes(30));

        return new CreatePaymentSessionResponseDto(
            true,
            Array.Empty<CheckoutValidationIssueDto>(),
            checkoutReview,
            payment);
    }

    private static string CreateProviderReference()
    {
        return $"DEV-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30].ToUpperInvariant();
    }

    private static string? BuildPaymentUrl(string? successUrl, string providerReference)
    {
        if (string.IsNullOrWhiteSpace(successUrl))
        {
            return null;
        }

        var trimmedUrl = successUrl.Trim();
        var separator = trimmedUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";

        return $"{trimmedUrl}{separator}paymentProvider=development&paymentReference={Uri.EscapeDataString(providerReference)}";
    }
}
