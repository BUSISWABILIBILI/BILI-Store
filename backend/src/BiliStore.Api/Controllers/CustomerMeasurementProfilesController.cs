using System.Security.Claims;
using BiliStore.Application.DTOs.Customers;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/customers/me/measurement-profiles")]
public sealed class CustomerMeasurementProfilesController(BiliStoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerMeasurementProfileDto>>> GetProfiles(CancellationToken cancellationToken)
    {
        var customerProfile = await GetCurrentCustomerProfileAsync(cancellationToken);

        if (customerProfile is null)
        {
            return Unauthorized();
        }

        var profiles = await dbContext.CustomerMeasurementProfiles
            .AsNoTracking()
            .Include(profile => profile.Measurements)
                .ThenInclude(measurement => measurement.MeasurementType)
            .Where(profile => profile.CustomerProfileId == customerProfile.Id)
            .OrderByDescending(profile => profile.IsDefault)
            .ThenBy(profile => profile.Name)
            .ToListAsync(cancellationToken);

        return Ok(profiles.Select(MapProfile).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<CustomerMeasurementProfileDto>> CreateProfile(
        CreateMeasurementProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var customerProfile = await GetCurrentCustomerProfileAsync(cancellationToken);

        if (customerProfile is null)
        {
            return Unauthorized();
        }

        if (request.Measurements.Count == 0)
        {
            return BadRequest(new { errors = new[] { "At least one measurement is required." } });
        }

        var requestedKeys = request.Measurements
            .Select(measurement => measurement.MeasurementKey.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray();
        var measurementTypes = await dbContext.MeasurementTypes
            .Where(measurementType => requestedKeys.Contains(measurementType.Key))
            .ToDictionaryAsync(measurementType => measurementType.Key, cancellationToken);
        var missingKeys = requestedKeys.Where(key => !measurementTypes.ContainsKey(key)).ToArray();

        if (missingKeys.Length > 0)
        {
            return BadRequest(new { errors = missingKeys.Select(key => $"Unknown measurement key: {key}").ToArray() });
        }

        if (request.IsDefault)
        {
            await dbContext.CustomerMeasurementProfiles
                .Where(profile => profile.CustomerProfileId == customerProfile.Id)
                .ExecuteUpdateAsync(
                    updates => updates.SetProperty(profile => profile.IsDefault, false),
                    cancellationToken);
        }

        var profile = new CustomerMeasurementProfile
        {
            CustomerProfileId = customerProfile.Id,
            Name = string.IsNullOrWhiteSpace(request.Name) ? "My Measurements" : request.Name.Trim(),
            Notes = request.Notes?.Trim(),
            IsDefault = request.IsDefault
        };

        foreach (var measurement in request.Measurements)
        {
            var key = measurement.MeasurementKey.Trim().ToLowerInvariant();
            profile.Measurements.Add(new CustomerMeasurement
            {
                MeasurementTypeId = measurementTypes[key].Id,
                ValueCm = measurement.ValueCm
            });
        }

        dbContext.CustomerMeasurementProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdProfile = await dbContext.CustomerMeasurementProfiles
            .AsNoTracking()
            .Include(currentProfile => currentProfile.Measurements)
                .ThenInclude(measurement => measurement.MeasurementType)
            .SingleAsync(currentProfile => currentProfile.Id == profile.Id, cancellationToken);

        return CreatedAtAction(nameof(GetProfiles), null, MapProfile(createdProfile));
    }

    private async Task<CustomerProfile?> GetCurrentCustomerProfileAsync(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        return await dbContext.CustomerProfiles
            .SingleOrDefaultAsync(profile => profile.ApplicationUserId == userId, cancellationToken);
    }

    private static CustomerMeasurementProfileDto MapProfile(CustomerMeasurementProfile profile)
    {
        return new CustomerMeasurementProfileDto(
            profile.Id,
            profile.Name,
            profile.IsDefault,
            profile.Notes,
            profile.Measurements
                .OrderBy(measurement => measurement.MeasurementType.DisplayOrder)
                .Select(measurement => new CustomerMeasurementValueDto(
                    measurement.MeasurementTypeId,
                    measurement.MeasurementType.Key,
                    measurement.MeasurementType.Name,
                    measurement.ValueCm,
                    measurement.MeasurementType.Unit))
                .ToList());
    }
}
