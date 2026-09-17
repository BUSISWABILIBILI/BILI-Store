using BiliStore.Application.DTOs.Measurements;
using BiliStore.Application.Measurements;
using BiliStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Measurements;

internal sealed class MeasurementValidationService(BiliStoreDbContext dbContext) : IMeasurementValidationService
{
    public async Task<MeasurementValidationResponseDto?> ValidateProductMeasurementsAsync(
        ValidateProductMeasurementsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductSlug))
        {
            return new MeasurementValidationResponseDto(
                false,
                [
                    new MeasurementValidationIssueDto(
                        "product",
                        "Product",
                        "A product slug is required before measurements can be validated.")
                ],
                []);
        }

        var product = await dbContext.Products
            .AsNoTracking()
            .Include(currentProduct => currentProduct.MeasurementRequirements)
                .ThenInclude(requirement => requirement.MeasurementType)
            .SingleOrDefaultAsync(
                currentProduct => currentProduct.Slug == request.ProductSlug && currentProduct.IsAvailable,
                cancellationToken);

        if (product is null)
        {
            return null;
        }

        var providedValues = request.Measurements
            .Where(measurement => !string.IsNullOrWhiteSpace(measurement.MeasurementKey))
            .GroupBy(measurement => Normalize(measurement.MeasurementKey))
            .ToDictionary(group => group.Key, group => group.Last().ValueCm);

        var issues = new List<MeasurementValidationIssueDto>();
        var acceptedMeasurements = new List<AcceptedMeasurementDto>();

        foreach (var requirement in product.MeasurementRequirements.OrderBy(current => current.MeasurementType.DisplayOrder))
        {
            var measurement = requirement.MeasurementType;
            var measurementKey = Normalize(measurement.Key);
            providedValues.TryGetValue(measurementKey, out var valueCm);

            if (requirement.IsRequired && (valueCm is null || valueCm <= 0))
            {
                issues.Add(new MeasurementValidationIssueDto(
                    measurement.Key,
                    measurement.Name,
                    $"{measurement.Name} is required for {product.Name}."));

                continue;
            }

            if (valueCm is null)
            {
                continue;
            }

            if (requirement.MinValueCm is not null && valueCm < requirement.MinValueCm)
            {
                issues.Add(new MeasurementValidationIssueDto(
                    measurement.Key,
                    measurement.Name,
                    $"{measurement.Name} must be at least {requirement.MinValueCm:0.##} cm."));

                continue;
            }

            if (requirement.MaxValueCm is not null && valueCm > requirement.MaxValueCm)
            {
                issues.Add(new MeasurementValidationIssueDto(
                    measurement.Key,
                    measurement.Name,
                    $"{measurement.Name} must be no more than {requirement.MaxValueCm:0.##} cm."));

                continue;
            }

            acceptedMeasurements.Add(new AcceptedMeasurementDto(
                measurement.Key,
                measurement.Name,
                valueCm.Value,
                measurement.Unit));
        }

        return new MeasurementValidationResponseDto(
            issues.Count == 0,
            issues,
            acceptedMeasurements);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
