using BiliStore.Application.Checkout;
using BiliStore.Application.DTOs.Cart;
using BiliStore.Application.DTOs.Checkout;
using BiliStore.Application.DTOs.Orders;
using BiliStore.Application.Orders;
using BiliStore.Domain.Entities;
using BiliStore.Domain.Enums;
using BiliStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Orders;

internal sealed class OrderService(
    BiliStoreDbContext dbContext,
    ICheckoutReviewService checkoutReviewService) : IOrderService
{
    public async Task<CreateOrderResponseDto> CreateAsync(
        CreateOrderRequestDto request,
        Guid? applicationUserId,
        CancellationToken cancellationToken)
    {
        var checkoutRequest = request.Checkout ?? new CheckoutReviewRequestDto(null, null, false, null);
        var checkoutReview = await checkoutReviewService.ReviewAsync(checkoutRequest, cancellationToken);
        var issues = new List<CheckoutValidationIssueDto>(checkoutReview.Issues);

        ValidatePayment(request.Payment, checkoutReview, issues);

        var products = await LoadProductsAsync(checkoutRequest, cancellationToken);

        foreach (var cartItem in checkoutRequest.Cart?.Items ?? Array.Empty<CartQuoteItemRequestDto>())
        {
            if (!products.ContainsKey(cartItem.ProductSlug))
            {
                issues.Add(new CheckoutValidationIssueDto("cart", $"Product '{cartItem.ProductSlug}' could not be found for order creation."));
            }
        }

        if (!checkoutReview.CanContinue || issues.Count > 0)
        {
            return new CreateOrderResponseDto(false, issues, null);
        }

        var customerProfileId = await GetCustomerProfileIdAsync(applicationUserId, cancellationToken);
        var order = await BuildOrderAsync(
            checkoutRequest,
            checkoutReview,
            request.Payment!,
            products,
            customerProfileId,
            cancellationToken);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        var savedOrder = await GetByOrderNumberAsync(order.OrderNumber, cancellationToken);

        return new CreateOrderResponseDto(true, Array.Empty<CheckoutValidationIssueDto>(), savedOrder);
    }

    public async Task<OrderConfirmationDto?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken)
    {
        var normalizedOrderNumber = orderNumber.Trim();

        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(currentOrder => currentOrder.Items)
                .ThenInclude(item => item.Measurements)
            .Include(currentOrder => currentOrder.DeliveryDetails)
            .Include(currentOrder => currentOrder.Payments)
            .SingleOrDefaultAsync(
                currentOrder => currentOrder.OrderNumber == normalizedOrderNumber,
                cancellationToken);

        return order is null ? null : MapOrder(order);
    }

    private async Task<Order> BuildOrderAsync(
        CheckoutReviewRequestDto checkoutRequest,
        CheckoutReviewResponseDto checkoutReview,
        CreateOrderPaymentDto payment,
        IReadOnlyDictionary<string, Product> products,
        Guid? customerProfileId,
        CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderNumber = await CreateOrderNumberAsync(cancellationToken),
            CustomerProfileId = customerProfileId,
            GuestEmail = customerProfileId is null ? checkoutReview.Customer!.Email : null,
            FirstName = checkoutReview.Customer!.FirstName,
            LastName = checkoutReview.Customer.LastName,
            Email = checkoutReview.Customer.Email,
            Phone = checkoutReview.Customer.Phone,
            OrderDate = DateTimeOffset.UtcNow,
            Subtotal = checkoutReview.Quote.Subtotal,
            DeliveryFee = checkoutReview.Quote.DeliveryFee,
            Total = checkoutReview.Quote.GrandTotal,
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.PendingPayment,
            ReturnPolicyAcceptedAt = checkoutReview.ReturnPolicyAcceptedAt ?? DateTimeOffset.UtcNow,
            DeliveryDetails = new DeliveryDetails
            {
                RecipientPhone = checkoutReview.Delivery!.RecipientPhone,
                PaxiStoreCode = checkoutReview.Delivery.PaxiStoreCode,
                CollectionPointName = checkoutReview.Delivery.CollectionPointName,
                TownCity = checkoutReview.Delivery.TownCity,
                Province = checkoutReview.Delivery.Province,
                Instructions = checkoutReview.Delivery.Instructions,
                DeliveryStatus = DeliveryStatus.Pending
            },
            Payments =
            {
                new Payment
                {
                    Provider = payment.Provider.Trim(),
                    ProviderReference = payment.ProviderReference.Trim(),
                    Amount = checkoutReview.Quote.GrandTotal,
                    Currency = checkoutReview.Quote.Currency,
                    Status = PaymentStatus.Pending,
                    PaymentUrl = string.IsNullOrWhiteSpace(payment.PaymentUrl) ? null : payment.PaymentUrl.Trim(),
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        };

        for (var index = 0; index < checkoutRequest.Cart!.Items.Count; index++)
        {
            var cartItem = checkoutRequest.Cart.Items[index];
            var quoteItem = checkoutReview.Quote.Items[index];
            var product = products[cartItem.ProductSlug];

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = quoteItem.ProductName,
                ProductDescription = product.Description,
                UnitPrice = quoteItem.UnitPrice,
                Quantity = quoteItem.Quantity,
                SelectedColourName = quoteItem.SelectedColourName,
                SelectedColourHex = quoteItem.SelectedColourHex,
                CustomColourName = quoteItem.CustomColourName,
                CustomColourHex = quoteItem.CustomColourHex,
                ProductionDaysEstimate = product.EstimatedProductionDays,
                Measurements = quoteItem.Measurements
                    .Select(measurement => new OrderItemMeasurement
                    {
                        MeasurementKey = measurement.MeasurementKey,
                        MeasurementTypeName = measurement.MeasurementName,
                        ValueCm = measurement.ValueCm
                    })
                    .ToList()
            });
        }

        return order;
    }

    private async Task<Dictionary<string, Product>> LoadProductsAsync(
        CheckoutReviewRequestDto checkoutRequest,
        CancellationToken cancellationToken)
    {
        var productSlugs = checkoutRequest.Cart?.Items
            .Select(item => item.ProductSlug)
            .Distinct()
            .ToArray() ?? Array.Empty<string>();

        return await dbContext.Products
            .AsNoTracking()
            .Where(product => productSlugs.Contains(product.Slug))
            .ToDictionaryAsync(product => product.Slug, StringComparer.OrdinalIgnoreCase, cancellationToken);
    }

    private async Task<Guid?> GetCustomerProfileIdAsync(
        Guid? applicationUserId,
        CancellationToken cancellationToken)
    {
        if (applicationUserId is null)
        {
            return null;
        }

        var customerProfile = await dbContext.CustomerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                profile => profile.ApplicationUserId == applicationUserId,
                cancellationToken);

        return customerProfile?.Id;
    }

    private static void ValidatePayment(
        CreateOrderPaymentDto? payment,
        CheckoutReviewResponseDto checkoutReview,
        ICollection<CheckoutValidationIssueDto> issues)
    {
        if (payment is null)
        {
            issues.Add(new CheckoutValidationIssueDto("payment", "Create a payment session before creating the order."));
            return;
        }

        if (string.IsNullOrWhiteSpace(payment.Provider))
        {
            issues.Add(new CheckoutValidationIssueDto("payment.provider", "Payment provider is required."));
        }

        if (string.IsNullOrWhiteSpace(payment.ProviderReference))
        {
            issues.Add(new CheckoutValidationIssueDto("payment.providerReference", "Payment reference is required."));
        }

        if (payment.Amount != checkoutReview.Quote.GrandTotal)
        {
            issues.Add(new CheckoutValidationIssueDto("payment.amount", "Payment amount must match the checkout total."));
        }

        if (!string.Equals(payment.Currency, checkoutReview.Quote.Currency, StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new CheckoutValidationIssueDto("payment.currency", "Payment currency must match the checkout currency."));
        }
    }

    private async Task<string> CreateOrderNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = $"BILI-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..22].ToUpperInvariant();
            var exists = await dbContext.Orders.AnyAsync(order => order.OrderNumber == candidate, cancellationToken);

            if (!exists)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Could not generate a unique order number.");
    }

    private static OrderConfirmationDto MapOrder(Order order)
    {
        var delivery = order.DeliveryDetails ?? new DeliveryDetails();

        return new OrderConfirmationDto(
            order.Id,
            order.OrderNumber,
            order.OrderDate,
            order.Subtotal,
            order.DeliveryFee,
            order.Total,
            order.PaymentStatus.ToString(),
            order.OrderStatus.ToString(),
            order.ReturnPolicyAcceptedAt,
            new OrderCustomerDto(order.FirstName, order.LastName, order.Email, order.Phone),
            new OrderDeliveryDto(
                delivery.DeliveryMethod.ToString(),
                delivery.RecipientPhone,
                delivery.PaxiStoreCode,
                delivery.CollectionPointName,
                delivery.TownCity,
                delivery.Province,
                delivery.Instructions,
                delivery.DeliveryStatus.ToString()),
            order.Items
                .Select(item => new OrderItemDto(
                    item.Id,
                    item.ProductName,
                    item.ProductDescription,
                    item.UnitPrice,
                    item.Quantity,
                    item.UnitPrice * item.Quantity,
                    item.SelectedColourName,
                    item.SelectedColourHex,
                    item.CustomColourName,
                    item.CustomColourHex,
                    item.ProductionDaysEstimate,
                    item.Measurements
                        .Select(measurement => new OrderItemMeasurementDto(
                            measurement.MeasurementKey,
                            measurement.MeasurementTypeName,
                            measurement.ValueCm))
                        .ToList()))
                .ToList(),
            order.Payments
                .Select(payment => new OrderPaymentDto(
                    payment.Provider,
                    payment.ProviderReference,
                    payment.Amount,
                    payment.Currency,
                    payment.Status.ToString(),
                    payment.PaymentUrl,
                    payment.CreatedAt))
                .ToList());
    }
}
