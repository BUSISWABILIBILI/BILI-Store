using System.Globalization;
using System.Text;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/reports")]
public sealed class AdminReportsController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet("orders.csv")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportOrders(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken cancellationToken)
    {
        var orders = await ApplyOrderDateRange(dbContext.Orders.AsNoTracking(), dateFrom, dateTo)
            .Include(order => order.Items)
            .Include(order => order.DeliveryDetails)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        AppendCsvRow(
            csv,
            "OrderNumber",
            "OrderDate",
            "CustomerName",
            "CustomerEmail",
            "Phone",
            "OrderStatus",
            "PaymentStatus",
            "DeliveryStatus",
            "ItemCount",
            "Subtotal",
            "DeliveryFee",
            "Total");

        foreach (var order in orders)
        {
            AppendCsvRow(
                csv,
                order.OrderNumber,
                FormatDateTime(order.OrderDate),
                $"{order.FirstName} {order.LastName}".Trim(),
                order.Email,
                order.Phone,
                order.OrderStatus,
                order.PaymentStatus,
                order.DeliveryDetails?.DeliveryStatus,
                order.Items.Sum(item => item.Quantity),
                FormatMoney(order.Subtotal),
                FormatMoney(order.DeliveryFee),
                FormatMoney(order.Total));
        }

        return CsvFile(csv, BuildFileName("bili-orders-report", dateFrom, dateTo));
    }

    [HttpGet("customers.csv")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportCustomers(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken cancellationToken)
    {
        var ordersQuery = ApplyOrderDateRange(dbContext.Orders.AsNoTracking(), dateFrom, dateTo);

        var customers = await dbContext.CustomerProfiles
            .AsNoTracking()
            .Join(
                dbContext.Users.AsNoTracking(),
                profile => profile.ApplicationUserId,
                user => user.Id,
                (profile, user) => new
                {
                    profile.Id,
                    profile.ApplicationUserId,
                    profile.FirstName,
                    profile.LastName,
                    Email = user.Email ?? string.Empty,
                    profile.PreferredPhone,
                    user.IsActive,
                    user.CreatedAt,
                    MeasurementProfileCount = dbContext.CustomerMeasurementProfiles
                        .Count(measurementProfile => measurementProfile.CustomerProfileId == profile.Id),
                    OrderCount = ordersQuery.Count(order => order.CustomerProfileId == profile.Id),
                    TotalSpent = ordersQuery
                        .Where(order => order.CustomerProfileId == profile.Id)
                        .Sum(order => (decimal?)order.Total) ?? 0m,
                    LatestOrderDate = ordersQuery
                        .Where(order => order.CustomerProfileId == profile.Id)
                        .OrderByDescending(order => order.OrderDate)
                        .Select(order => (DateTimeOffset?)order.OrderDate)
                        .FirstOrDefault()
                })
            .OrderByDescending(customer => customer.LatestOrderDate ?? customer.CreatedAt)
            .ThenBy(customer => customer.Email)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        AppendCsvRow(
            csv,
            "CustomerId",
            "CreatedAt",
            "FirstName",
            "LastName",
            "Email",
            "Phone",
            "IsActive",
            "OrderCount",
            "TotalSpent",
            "LatestOrderDate",
            "MeasurementProfileCount");

        foreach (var customer in customers)
        {
            AppendCsvRow(
                csv,
                customer.Id,
                FormatDateTime(customer.CreatedAt),
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PreferredPhone,
                customer.IsActive,
                customer.OrderCount,
                FormatMoney(customer.TotalSpent),
                customer.LatestOrderDate is null ? string.Empty : FormatDateTime(customer.LatestOrderDate.Value),
                customer.MeasurementProfileCount);
        }

        return CsvFile(csv, BuildFileName("bili-customers-report", dateFrom, dateTo));
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

    private static FileContentResult CsvFile(StringBuilder csv, string fileName)
    {
        return new FileContentResult(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv")
        {
            FileDownloadName = fileName
        };
    }

    private static void AppendCsvRow(StringBuilder csv, params object?[] values)
    {
        csv.AppendLine(string.Join(",", values.Select(value => EscapeCsv(Convert.ToString(value, CultureInfo.InvariantCulture)))));
    }

    private static string EscapeCsv(string? value)
    {
        var text = value ?? string.Empty;

        return text.Contains('"') || text.Contains(',') || text.Contains('\n') || text.Contains('\r')
            ? $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : text;
    }

    private static string FormatMoney(decimal value)
    {
        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string FormatDateTime(DateTimeOffset value)
    {
        return value.ToString("u", CultureInfo.InvariantCulture);
    }

    private static string BuildFileName(string reportName, DateOnly? dateFrom, DateOnly? dateTo)
    {
        if (dateFrom is null && dateTo is null)
        {
            return $"{reportName}.csv";
        }

        var from = dateFrom?.ToString("yyyyMMdd", CultureInfo.InvariantCulture) ?? "start";
        var to = dateTo?.ToString("yyyyMMdd", CultureInfo.InvariantCulture) ?? "today";

        return $"{reportName}-{from}-{to}.csv";
    }
}
