using BiliStore.Application.Cart;
using BiliStore.Application.DTOs.Cart;
using BiliStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Cart;

internal sealed class CartQuoteService(BiliStoreDbContext dbContext) : ICartQuoteService
{
    private const decimal DeliveryFee = 99m;

    public async Task<CartQuoteResponseDto> QuoteAsync(CartQuoteRequestDto request, CancellationToken cancellationToken)
    {
        var responseItems = new List<CartQuoteItemResponseDto>();
        var issues = new List<CartQuoteIssueDto>();

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ProductSlug))
            {
                issues.Add(new CartQuoteIssueDto("", "A cart item is missing a product slug."));
                continue;
            }

            var product = await dbContext.Products
                .AsNoTracking()
                .Include(currentProduct => currentProduct.AvailableColours)
                    .ThenInclude(productColour => productColour.Colour)
                .Include(currentProduct => currentProduct.MeasurementRequirements)
                    .ThenInclude(requirement => requirement.MeasurementType)
                .SingleOrDefaultAsync(
                    currentProduct => currentProduct.Slug == item.ProductSlug && currentProduct.IsAvailable,
                    cancellationToken);

            if (product is null)
            {
                issues.Add(new CartQuoteIssueDto(item.ProductSlug, "This product is no longer available."));
                continue;
            }

            if (item.Quantity < 1)
            {
                issues.Add(new CartQuoteIssueDto(item.ProductSlug, "Quantity must be at least 1."));
                continue;
            }

            var colour = item.SelectedColourId is null
                ? null
                : product.AvailableColours
                    .Select(productColour => productColour.Colour)
                    .SingleOrDefault(currentColour => currentColour.Id == item.SelectedColourId);

            if (item.SelectedColourId is not null && colour is null)
            {
                issues.Add(new CartQuoteIssueDto(item.ProductSlug, "The selected colour is not available for this product."));
                continue;
            }

            if (!product.AllowsCustomColor && !string.IsNullOrWhiteSpace(item.CustomColourName))
            {
                issues.Add(new CartQuoteIssueDto(item.ProductSlug, "This product does not support custom colours."));
                continue;
            }

            foreach (var requiredMeasurement in product.MeasurementRequirements.Where(requirement => requirement.IsRequired))
            {
                var hasMeasurement = item.Measurements.Any(measurement =>
                    string.Equals(
                        measurement.MeasurementKey,
                        requiredMeasurement.MeasurementType.Key,
                        StringComparison.OrdinalIgnoreCase) &&
                    measurement.ValueCm > 0);

                if (!hasMeasurement)
                {
                    issues.Add(new CartQuoteIssueDto(
                        item.ProductSlug,
                        $"{requiredMeasurement.MeasurementType.Name} is required for {product.Name}."));
                }
            }

            var subtotal = product.Price * item.Quantity;

            responseItems.Add(new CartQuoteItemResponseDto(
                product.Slug,
                product.Name,
                product.Price,
                item.Quantity,
                subtotal,
                colour?.Name,
                colour?.HexCode,
                item.CustomColourName,
                item.CustomColourHex,
                item.Measurements));
        }

        var subtotalAmount = responseItems.Sum(item => item.Subtotal);
        var deliveryFee = responseItems.Count > 0 ? DeliveryFee : 0m;

        return new CartQuoteResponseDto(
            responseItems,
            issues,
            subtotalAmount,
            deliveryFee,
            subtotalAmount + deliveryFee,
            "ZAR");
    }
}
