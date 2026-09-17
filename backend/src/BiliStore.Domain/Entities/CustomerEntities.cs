using BiliStore.Domain.Common;

namespace BiliStore.Domain.Entities;

public sealed class CustomerProfile : AuditableEntity
{
    public Guid ApplicationUserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PreferredPhone { get; set; } = string.Empty;

    public ICollection<CustomerMeasurementProfile> MeasurementProfiles { get; set; } = new List<CustomerMeasurementProfile>();
}

public sealed class CustomerMeasurementProfile : AuditableEntity
{
    public Guid CustomerProfileId { get; set; }

    public CustomerProfile CustomerProfile { get; set; } = default!;

    public string Name { get; set; } = "My Measurements";

    public bool IsDefault { get; set; }

    public string? Notes { get; set; }

    public ICollection<CustomerMeasurement> Measurements { get; set; } = new List<CustomerMeasurement>();
}

public sealed class CustomerMeasurement : Entity
{
    public Guid CustomerMeasurementProfileId { get; set; }

    public CustomerMeasurementProfile CustomerMeasurementProfile { get; set; } = default!;

    public Guid MeasurementTypeId { get; set; }

    public MeasurementType MeasurementType { get; set; } = default!;

    public decimal ValueCm { get; set; }
}

