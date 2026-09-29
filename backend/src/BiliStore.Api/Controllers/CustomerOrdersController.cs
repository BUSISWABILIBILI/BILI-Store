using System.Security.Claims;
using BiliStore.Application.DTOs.Customers;
using BiliStore.Domain.Entities;
using BiliStore.Domain.Enums;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/customers/me/orders")]
public sealed class CustomerOrdersController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerOrderSummaryDto>>> GetOrders(CancellationToken cancellationToken)
    {
        var customerProfile = await GetCurrentCustomerProfileAsync(cancellationToken);

        if (customerProfile is null)
        {
            return Unauthorized();
        }

        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.CustomerProfileId == customerProfile.Id)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new CustomerOrderSummaryDto(
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                order.Total,
                order.PaymentStatus.ToString(),
                order.OrderStatus.ToString(),
                order.Items.Count))
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("return-requests")]
    public async Task<ActionResult<IReadOnlyList<CustomerReturnRequestDto>>> GetReturnRequests(CancellationToken cancellationToken)
    {
        var customerProfile = await GetCurrentCustomerProfileAsync(cancellationToken);

        if (customerProfile is null)
        {
            return Unauthorized();
        }

        var requests = await dbContext.ReturnRequests
            .AsNoTracking()
            .Include(returnRequest => returnRequest.Order)
            .Where(returnRequest => returnRequest.Order.CustomerProfileId == customerProfile.Id)
            .OrderByDescending(returnRequest => returnRequest.RequestedAt)
            .Select(returnRequest => new CustomerReturnRequestDto(
                returnRequest.Id,
                returnRequest.Order.OrderNumber,
                returnRequest.Reason,
                returnRequest.Details,
                returnRequest.Status.ToString(),
                returnRequest.RequestedAt,
                returnRequest.ResolvedAt))
            .ToListAsync(cancellationToken);

        return Ok(requests);
    }

    [HttpPost("{orderNumber}/return-requests")]
    public async Task<ActionResult<CustomerReturnRequestDto>> CreateReturnRequest(
        string orderNumber,
        CreateReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        var customerProfile = await GetCurrentCustomerProfileAsync(cancellationToken);

        if (customerProfile is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Reason) || string.IsNullOrWhiteSpace(request.Details))
        {
            return BadRequest(new { errors = new[] { "Return reason and details are required." } });
        }

        var order = await dbContext.Orders
            .Include(currentOrder => currentOrder.ReturnRequests)
            .SingleOrDefaultAsync(
                currentOrder => currentOrder.CustomerProfileId == customerProfile.Id
                    && currentOrder.OrderNumber == orderNumber.Trim(),
                cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        var hasOpenReturn = order.ReturnRequests.Any(returnRequest =>
            returnRequest.Status == ReturnRequestStatus.Submitted
            || returnRequest.Status == ReturnRequestStatus.UnderReview);

        if (hasOpenReturn)
        {
            return BadRequest(new { errors = new[] { "This order already has an open return request." } });
        }

        var returnRequest = new ReturnRequest
        {
            OrderId = order.Id,
            Reason = request.Reason.Trim(),
            Details = request.Details.Trim(),
            Status = ReturnRequestStatus.Submitted,
            RequestedAt = DateTimeOffset.UtcNow
        };

        dbContext.ReturnRequests.Add(returnRequest);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CustomerReturnRequestDto(
            returnRequest.Id,
            order.OrderNumber,
            returnRequest.Reason,
            returnRequest.Details,
            returnRequest.Status.ToString(),
            returnRequest.RequestedAt,
            returnRequest.ResolvedAt);

        return CreatedAtAction(nameof(GetReturnRequests), null, response);
    }

    private async Task<CustomerProfile?> GetCurrentCustomerProfileAsync(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        return await dbContext.CustomerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(profile => profile.ApplicationUserId == userId, cancellationToken);
    }
}
