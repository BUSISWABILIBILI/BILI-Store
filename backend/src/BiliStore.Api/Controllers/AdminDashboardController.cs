using BiliStore.Application.DTOs.Admin;
using BiliStore.Domain.Enums;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        var orderCount = await dbContext.Orders.CountAsync(cancellationToken);
        var pendingOrderCount = await dbContext.Orders
            .CountAsync(order => order.OrderStatus == OrderStatus.PendingPayment, cancellationToken);
        var inProductionOrderCount = await dbContext.Orders
            .CountAsync(order => order.OrderStatus == OrderStatus.InProduction, cancellationToken);
        var completedOrderCount = await dbContext.Orders
            .CountAsync(order => order.OrderStatus == OrderStatus.Completed, cancellationToken);
        var paidRevenue = await dbContext.Orders
            .Where(order => order.PaymentStatus == PaymentStatus.Paid)
            .SumAsync(order => (decimal?)order.Total, cancellationToken) ?? 0m;
        var pendingRevenue = await dbContext.Orders
            .Where(order => order.PaymentStatus == PaymentStatus.Pending)
            .SumAsync(order => (decimal?)order.Total, cancellationToken) ?? 0m;
        var customerCount = await dbContext.CustomerProfiles.CountAsync(cancellationToken);
        var guestOrderCount = await dbContext.Orders
            .CountAsync(order => order.CustomerProfileId == null, cancellationToken);
        var availableProductCount = await dbContext.Products
            .CountAsync(product => product.IsAvailable, cancellationToken);
        var unavailableProductCount = await dbContext.Products
            .CountAsync(product => !product.IsAvailable, cancellationToken);
        var returnRequestCount = await dbContext.ReturnRequests.CountAsync(cancellationToken);
        var openReturnRequestCount = await dbContext.ReturnRequests
            .CountAsync(
                returnRequest => returnRequest.Status == ReturnRequestStatus.Submitted
                    || returnRequest.Status == ReturnRequestStatus.UnderReview,
                cancellationToken);

        var ordersByStatus = await dbContext.Orders
            .AsNoTracking()
            .GroupBy(order => order.OrderStatus)
            .Select(group => new AdminDashboardStatusCountDto(group.Key.ToString(), group.Count()))
            .OrderBy(status => status.Status)
            .ToListAsync(cancellationToken);

        var paymentsByStatus = await dbContext.Orders
            .AsNoTracking()
            .GroupBy(order => order.PaymentStatus)
            .Select(group => new AdminDashboardStatusCountDto(group.Key.ToString(), group.Count()))
            .OrderBy(status => status.Status)
            .ToListAsync(cancellationToken);

        var recentOrders = await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.OrderDate)
            .Take(6)
            .Select(order => new AdminDashboardRecentOrderDto(
                order.OrderNumber,
                $"{order.FirstName} {order.LastName}".Trim(),
                order.Total,
                order.OrderStatus.ToString(),
                order.PaymentStatus.ToString(),
                order.OrderDate))
            .ToListAsync(cancellationToken);

        var hiddenProductAlerts = await dbContext.Products
            .AsNoTracking()
            .Where(product => !product.IsAvailable)
            .OrderBy(product => product.Name)
            .Take(4)
            .Select(product => new AdminDashboardProductAlertDto(
                product.Id,
                product.Name,
                product.Slug,
                product.IsAvailable,
                product.IsFeatured,
                "Hidden from shop"))
            .ToListAsync(cancellationToken);

        var missingMeasurementAlerts = await dbContext.Products
            .AsNoTracking()
            .Where(product => !product.MeasurementRequirements.Any())
            .OrderBy(product => product.Name)
            .Take(4)
            .Select(product => new AdminDashboardProductAlertDto(
                product.Id,
                product.Name,
                product.Slug,
                product.IsAvailable,
                product.IsFeatured,
                "No measurement rules"))
            .ToListAsync(cancellationToken);

        var productAlerts = hiddenProductAlerts
            .Concat(missingMeasurementAlerts)
            .DistinctBy(alert => new { alert.Id, alert.Issue })
            .Take(8)
            .ToList();

        return Ok(new AdminDashboardDto(
            orderCount,
            pendingOrderCount,
            inProductionOrderCount,
            completedOrderCount,
            paidRevenue,
            pendingRevenue,
            customerCount,
            guestOrderCount,
            availableProductCount,
            unavailableProductCount,
            returnRequestCount,
            openReturnRequestCount,
            ordersByStatus,
            paymentsByStatus,
            recentOrders,
            productAlerts));
    }
}
