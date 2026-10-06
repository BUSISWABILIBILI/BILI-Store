using BiliStore.Application.DTOs.Admin;
using BiliStore.Domain.Entities;
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
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken cancellationToken)
    {
        var ordersQuery = ApplyOrderDateRange(dbContext.Orders.AsNoTracking(), dateFrom, dateTo);
        var returnRequestsQuery = ApplyReturnRequestDateRange(dbContext.ReturnRequests.AsNoTracking(), dateFrom, dateTo);

        var orderCount = await ordersQuery.CountAsync(cancellationToken);
        var pendingOrderCount = await ordersQuery
            .CountAsync(order => order.OrderStatus == OrderStatus.PendingPayment, cancellationToken);
        var inProductionOrderCount = await ordersQuery
            .CountAsync(order => order.OrderStatus == OrderStatus.InProduction, cancellationToken);
        var completedOrderCount = await ordersQuery
            .CountAsync(order => order.OrderStatus == OrderStatus.Completed, cancellationToken);
        var paidRevenue = await ordersQuery
            .Where(order => order.PaymentStatus == PaymentStatus.Paid)
            .SumAsync(order => (decimal?)order.Total, cancellationToken) ?? 0m;
        var pendingRevenue = await ordersQuery
            .Where(order => order.PaymentStatus == PaymentStatus.Pending)
            .SumAsync(order => (decimal?)order.Total, cancellationToken) ?? 0m;
        var customerCount = await dbContext.CustomerProfiles.CountAsync(cancellationToken);
        var guestOrderCount = await ordersQuery
            .CountAsync(order => order.CustomerProfileId == null, cancellationToken);
        var availableProductCount = await dbContext.Products
            .CountAsync(product => product.IsAvailable, cancellationToken);
        var unavailableProductCount = await dbContext.Products
            .CountAsync(product => !product.IsAvailable, cancellationToken);
        var returnRequestCount = await returnRequestsQuery.CountAsync(cancellationToken);
        var openReturnRequestCount = await returnRequestsQuery
            .CountAsync(
                returnRequest => returnRequest.Status == ReturnRequestStatus.Submitted
                    || returnRequest.Status == ReturnRequestStatus.UnderReview,
                cancellationToken);

        var ordersByStatus = await ordersQuery
            .GroupBy(order => order.OrderStatus)
            .Select(group => new AdminDashboardStatusCountDto(group.Key.ToString(), group.Count()))
            .OrderBy(status => status.Status)
            .ToListAsync(cancellationToken);

        var paymentsByStatus = await ordersQuery
            .GroupBy(order => order.PaymentStatus)
            .Select(group => new AdminDashboardStatusCountDto(group.Key.ToString(), group.Count()))
            .OrderBy(status => status.Status)
            .ToListAsync(cancellationToken);

        var recentOrders = await ordersQuery
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

    private static IQueryable<Order> ApplyOrderDateRange(
        IQueryable<Order> query,
        DateOnly? dateFrom,
        DateOnly? dateTo)
    {
        if (dateFrom is not null)
        {
            var start = new DateTimeOffset(dateFrom.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(order => order.OrderDate >= start);
        }

        if (dateTo is not null)
        {
            var endExclusive = new DateTimeOffset(dateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(order => order.OrderDate < endExclusive);
        }

        return query;
    }

    private static IQueryable<ReturnRequest> ApplyReturnRequestDateRange(
        IQueryable<ReturnRequest> query,
        DateOnly? dateFrom,
        DateOnly? dateTo)
    {
        if (dateFrom is not null)
        {
            var start = new DateTimeOffset(dateFrom.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(returnRequest => returnRequest.RequestedAt >= start);
        }

        if (dateTo is not null)
        {
            var endExclusive = new DateTimeOffset(dateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(returnRequest => returnRequest.RequestedAt < endExclusive);
        }

        return query;
    }
}
