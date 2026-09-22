using BiliStore.Application.DTOs.Admin;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/customers")]
public sealed class AdminCustomersController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminCustomerSummaryDto>>> GetCustomers(CancellationToken cancellationToken)
    {
        var customers = await dbContext.CustomerProfiles
            .AsNoTracking()
            .Join(
                dbContext.Users.AsNoTracking(),
                profile => profile.ApplicationUserId,
                user => user.Id,
                (profile, user) => new { Profile = profile, User = user })
            .Select(customer => new AdminCustomerSummaryDto(
                customer.Profile.Id,
                customer.Profile.ApplicationUserId,
                customer.Profile.FirstName,
                customer.Profile.LastName,
                customer.User.Email ?? string.Empty,
                customer.Profile.PreferredPhone,
                customer.User.IsActive,
                customer.User.CreatedAt,
                dbContext.CustomerMeasurementProfiles.Count(profile => profile.CustomerProfileId == customer.Profile.Id),
                dbContext.Orders.Count(order => order.CustomerProfileId == customer.Profile.Id),
                dbContext.Orders
                    .Where(order => order.CustomerProfileId == customer.Profile.Id)
                    .Sum(order => (decimal?)order.Total) ?? 0m,
                dbContext.Orders
                    .Where(order => order.CustomerProfileId == customer.Profile.Id)
                    .OrderByDescending(order => order.OrderDate)
                    .Select(order => (DateTimeOffset?)order.OrderDate)
                    .FirstOrDefault()))
            .OrderByDescending(customer => customer.LatestOrderDate ?? customer.CreatedAt)
            .ThenBy(customer => customer.Email)
            .ToListAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCustomerDetailsDto>> GetCustomer(
        Guid id,
        CancellationToken cancellationToken)
    {
        var summary = await dbContext.CustomerProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == id)
            .Join(
                dbContext.Users.AsNoTracking(),
                profile => profile.ApplicationUserId,
                user => user.Id,
                (profile, user) => new AdminCustomerSummaryDto(
                    profile.Id,
                    profile.ApplicationUserId,
                    profile.FirstName,
                    profile.LastName,
                    user.Email ?? string.Empty,
                    profile.PreferredPhone,
                    user.IsActive,
                    user.CreatedAt,
                    dbContext.CustomerMeasurementProfiles.Count(measurementProfile => measurementProfile.CustomerProfileId == profile.Id),
                    dbContext.Orders.Count(order => order.CustomerProfileId == profile.Id),
                    dbContext.Orders
                        .Where(order => order.CustomerProfileId == profile.Id)
                        .Sum(order => (decimal?)order.Total) ?? 0m,
                    dbContext.Orders
                        .Where(order => order.CustomerProfileId == profile.Id)
                        .OrderByDescending(order => order.OrderDate)
                        .Select(order => (DateTimeOffset?)order.OrderDate)
                        .FirstOrDefault()))
            .SingleOrDefaultAsync(cancellationToken);

        if (summary is null)
        {
            return NotFound();
        }

        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.CustomerProfileId == id)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new AdminCustomerOrderSummaryDto(
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                order.Total,
                order.PaymentStatus.ToString(),
                order.OrderStatus.ToString(),
                order.Items.Count))
            .ToListAsync(cancellationToken);

        var measurementProfiles = await dbContext.CustomerMeasurementProfiles
            .AsNoTracking()
            .Include(profile => profile.Measurements)
                .ThenInclude(measurement => measurement.MeasurementType)
            .Where(profile => profile.CustomerProfileId == id)
            .OrderByDescending(profile => profile.IsDefault)
            .ThenBy(profile => profile.Name)
            .ToListAsync(cancellationToken);

        return Ok(new AdminCustomerDetailsDto(
            summary.Id,
            summary.ApplicationUserId,
            summary.FirstName,
            summary.LastName,
            summary.Email,
            summary.Phone,
            summary.IsActive,
            summary.CreatedAt,
            summary.MeasurementProfileCount,
            summary.OrderCount,
            summary.TotalSpent,
            summary.LatestOrderDate,
            orders,
            measurementProfiles.Select(MapMeasurementProfile).ToList()));
    }

    private static AdminCustomerMeasurementProfileDto MapMeasurementProfile(CustomerMeasurementProfile profile)
    {
        var measurements = profile.Measurements
            .OrderBy(measurement => measurement.MeasurementType.DisplayOrder)
            .Select(measurement => new AdminCustomerMeasurementValueDto(
                measurement.MeasurementType.Name,
                measurement.MeasurementType.Key,
                measurement.ValueCm,
                measurement.MeasurementType.Unit))
            .ToList();

        return new AdminCustomerMeasurementProfileDto(
            profile.Id,
            profile.Name,
            profile.IsDefault,
            profile.Notes,
            measurements.Count,
            measurements);
    }
}
