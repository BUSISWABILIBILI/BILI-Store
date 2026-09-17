using System.Security.Claims;
using BiliStore.Application.DTOs.Customers;
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
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var customerProfile = await dbContext.CustomerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(profile => profile.ApplicationUserId == userId, cancellationToken);

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
}
