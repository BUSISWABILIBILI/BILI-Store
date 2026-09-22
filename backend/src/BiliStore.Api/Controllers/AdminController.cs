using BiliStore.Application.DTOs.Admin;
using BiliStore.Application.DTOs.Orders;
using BiliStore.Application.Orders;
using BiliStore.Domain.Enums;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public sealed class AdminController(
    BiliStoreDbContext dbContext,
    IOrderService orderService) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<AdminOrderSummaryDto>>> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.DeliveryDetails)
            .Include(order => order.Payments)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new AdminOrderSummaryDto(
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                $"{order.FirstName} {order.LastName}".Trim(),
                order.Email,
                order.Total,
                order.PaymentStatus.ToString(),
                order.OrderStatus.ToString(),
                order.DeliveryDetails == null ? "Missing" : order.DeliveryDetails.DeliveryStatus.ToString(),
                order.Items.Count,
                order.Payments
                    .OrderByDescending(payment => payment.CreatedAt)
                    .Select(payment => payment.ProviderReference)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("orders/{orderNumber}")]
    public async Task<ActionResult<OrderConfirmationDto>> GetOrder(
        string orderNumber,
        CancellationToken cancellationToken)
    {
        var order = await orderService.GetByOrderNumberAsync(orderNumber, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    [HttpPut("orders/{orderNumber}/status")]
    public async Task<ActionResult<OrderConfirmationDto>> UpdateOrderStatus(
        string orderNumber,
        UpdateAdminOrderStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrderStatus>(request.OrderStatus, true, out var orderStatus))
        {
            return BadRequest(new { errors = new[] { "Unknown order status." } });
        }

        PaymentStatus? paymentStatus = null;

        if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
        {
            if (!Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var parsedPaymentStatus))
            {
                return BadRequest(new { errors = new[] { "Unknown payment status." } });
            }

            paymentStatus = parsedPaymentStatus;
        }

        var order = await dbContext.Orders
            .Include(currentOrder => currentOrder.Payments)
            .SingleOrDefaultAsync(
                currentOrder => currentOrder.OrderNumber == orderNumber.Trim(),
                cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        order.OrderStatus = orderStatus;
        order.UpdatedAt = DateTimeOffset.UtcNow;

        if (paymentStatus is not null)
        {
            order.PaymentStatus = paymentStatus.Value;

            foreach (var payment in order.Payments)
            {
                payment.Status = paymentStatus.Value;
                payment.PaidAt = paymentStatus.Value == PaymentStatus.Paid
                    ? DateTimeOffset.UtcNow
                    : payment.PaidAt;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var updatedOrder = await orderService.GetByOrderNumberAsync(order.OrderNumber, cancellationToken);

        return updatedOrder is null ? NotFound() : Ok(updatedOrder);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<IReadOnlyList<AdminPaymentSummaryDto>>> GetPayments(CancellationToken cancellationToken)
    {
        var payments = await dbContext.Payments
            .AsNoTracking()
            .Include(payment => payment.Order)
            .OrderByDescending(payment => payment.CreatedAt)
            .Select(payment => new AdminPaymentSummaryDto(
                payment.Id,
                payment.Order.OrderNumber,
                payment.Order.Email,
                payment.Provider,
                payment.ProviderReference,
                payment.Amount,
                payment.Currency,
                payment.Status.ToString(),
                payment.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(payments);
    }

    [HttpGet("return-requests")]
    public async Task<ActionResult<IReadOnlyList<AdminReturnRequestSummaryDto>>> GetReturnRequests(CancellationToken cancellationToken)
    {
        var requests = await dbContext.ReturnRequests
            .AsNoTracking()
            .Include(returnRequest => returnRequest.Order)
            .OrderByDescending(returnRequest => returnRequest.RequestedAt)
            .Select(returnRequest => new AdminReturnRequestSummaryDto(
                returnRequest.Id,
                returnRequest.Order.OrderNumber,
                returnRequest.Order.Email,
                returnRequest.Reason,
                returnRequest.Details,
                returnRequest.Status.ToString(),
                returnRequest.RequestedAt,
                returnRequest.ResolvedAt))
            .ToListAsync(cancellationToken);

        return Ok(requests);
    }

    [HttpPut("return-requests/{id:guid}/status")]
    public async Task<ActionResult<AdminReturnRequestSummaryDto>> UpdateReturnRequestStatus(
        Guid id,
        UpdateReturnRequestStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReturnRequestStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { errors = new[] { "Unknown return request status." } });
        }

        var returnRequest = await dbContext.ReturnRequests
            .Include(currentRequest => currentRequest.Order)
            .SingleOrDefaultAsync(currentRequest => currentRequest.Id == id, cancellationToken);

        if (returnRequest is null)
        {
            return NotFound();
        }

        returnRequest.Status = status;
        returnRequest.ResolvedAt = status is ReturnRequestStatus.ApprovedForRemake
            or ReturnRequestStatus.ApprovedForRefund
            or ReturnRequestStatus.Rejected
            or ReturnRequestStatus.Closed
            ? DateTimeOffset.UtcNow
            : null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new AdminReturnRequestSummaryDto(
            returnRequest.Id,
            returnRequest.Order.OrderNumber,
            returnRequest.Order.Email,
            returnRequest.Reason,
            returnRequest.Details,
            returnRequest.Status.ToString(),
            returnRequest.RequestedAt,
            returnRequest.ResolvedAt));
    }
}
