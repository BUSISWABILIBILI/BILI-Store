using System.Net.Mail;
using BiliStore.Application.Cart;
using BiliStore.Application.Checkout;
using BiliStore.Application.DTOs.Cart;
using BiliStore.Application.DTOs.Checkout;

namespace BiliStore.Infrastructure.Checkout;

internal sealed class CheckoutReviewService(ICartQuoteService cartQuoteService) : ICheckoutReviewService
{
    public async Task<CheckoutReviewResponseDto> ReviewAsync(
        CheckoutReviewRequestDto request,
        CancellationToken cancellationToken)
    {
        var issues = new List<CheckoutValidationIssueDto>();

        ValidateCustomer(request.Customer, issues);
        ValidateDelivery(request.Delivery, issues);
        ValidateReturnPolicy(request.ReturnPolicyAccepted, issues);

        var cartRequest = request.Cart ?? new CartQuoteRequestDto(Array.Empty<CartQuoteItemRequestDto>());

        if (cartRequest.Items.Count == 0)
        {
            issues.Add(new CheckoutValidationIssueDto("cart.items", "Add at least one custom garment before checkout."));
        }

        var quote = await cartQuoteService.QuoteAsync(cartRequest, cancellationToken);

        foreach (var quoteIssue in quote.Issues)
        {
            issues.Add(new CheckoutValidationIssueDto("cart", quoteIssue.Message));
        }

        var canContinue = issues.Count == 0 && quote.Items.Count > 0;

        return new CheckoutReviewResponseDto(
            canContinue,
            issues,
            quote,
            request.Customer is null ? null : new CheckoutCustomerReviewDto(
                Normalize(request.Customer.FirstName),
                Normalize(request.Customer.LastName),
                Normalize(request.Customer.Email),
                Normalize(request.Customer.Phone)),
            request.Delivery is null ? null : new CheckoutDeliveryReviewDto(
                "PAXI/PEP",
                Normalize(request.Delivery.RecipientPhone),
                Normalize(request.Delivery.PaxiStoreCode),
                Normalize(request.Delivery.CollectionPointName),
                Normalize(request.Delivery.TownCity),
                Normalize(request.Delivery.Province),
                NormalizeOptional(request.Delivery.Instructions)),
            request.ReturnPolicyAccepted ? DateTimeOffset.UtcNow : null);
    }

    private static void ValidateCustomer(
        CheckoutCustomerDetailsDto? customer,
        ICollection<CheckoutValidationIssueDto> issues)
    {
        if (customer is null)
        {
            issues.Add(new CheckoutValidationIssueDto("customer", "Customer details are required."));
            return;
        }

        AddRequiredIssue(customer.FirstName, "customer.firstName", "First name is required.", issues);
        AddRequiredIssue(customer.LastName, "customer.lastName", "Last name is required.", issues);
        AddRequiredIssue(customer.Phone, "customer.phone", "Phone number is required.", issues);

        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            issues.Add(new CheckoutValidationIssueDto("customer.email", "Email address is required."));
        }
        else if (!IsValidEmail(customer.Email))
        {
            issues.Add(new CheckoutValidationIssueDto("customer.email", "Enter a valid email address."));
        }
    }

    private static void ValidateDelivery(
        CheckoutDeliveryDetailsDto? delivery,
        ICollection<CheckoutValidationIssueDto> issues)
    {
        if (delivery is null)
        {
            issues.Add(new CheckoutValidationIssueDto("delivery", "PAXI/PEP delivery details are required."));
            return;
        }

        AddRequiredIssue(delivery.RecipientPhone, "delivery.recipientPhone", "Recipient phone number is required.", issues);
        AddRequiredIssue(delivery.PaxiStoreCode, "delivery.paxiStoreCode", "PAXI/PEP store code is required.", issues);
        AddRequiredIssue(delivery.CollectionPointName, "delivery.collectionPointName", "Collection point name is required.", issues);
        AddRequiredIssue(delivery.TownCity, "delivery.townCity", "Town or city is required.", issues);
        AddRequiredIssue(delivery.Province, "delivery.province", "Province is required.", issues);
    }

    private static void ValidateReturnPolicy(
        bool returnPolicyAccepted,
        ICollection<CheckoutValidationIssueDto> issues)
    {
        if (!returnPolicyAccepted)
        {
            issues.Add(new CheckoutValidationIssueDto("returnPolicyAccepted", "Accept the return and refund policy before checkout."));
        }
    }

    private static void AddRequiredIssue(
        string value,
        string field,
        string message,
        ICollection<CheckoutValidationIssueDto> issues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            issues.Add(new CheckoutValidationIssueDto(field, message));
        }
    }

    private static bool IsValidEmail(string value)
    {
        try
        {
            var address = new MailAddress(value);
            return string.Equals(address.Address, value.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string Normalize(string value) => value.Trim();

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
