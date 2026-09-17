using BiliStore.Domain.Common;

namespace BiliStore.Domain.Entities;

public sealed class Cart : AuditableEntity
{
    public Guid? CustomerProfileId { get; set; }

    public CustomerProfile? CustomerProfile { get; set; }

    public string? AnonymousCartKey { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public sealed class CartItem : AuditableEntity
{
    public Guid CartId { get; set; }

    public Cart Cart { get; set; } = default!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;

    public string ProductSnapshotName { get; set; } = string.Empty;

    public decimal ProductSnapshotPrice { get; set; }

    public Guid? SelectedColourId { get; set; }

    public Colour? SelectedColour { get; set; }

    public string? CustomColourName { get; set; }

    public string? CustomColourHex { get; set; }

    public int Quantity { get; set; } = 1;

    public ICollection<CartItemMeasurement> Measurements { get; set; } = new List<CartItemMeasurement>();
}

public sealed class CartItemMeasurement : Entity
{
    public Guid CartItemId { get; set; }

    public CartItem CartItem { get; set; } = default!;

    public Guid MeasurementTypeId { get; set; }

    public MeasurementType MeasurementType { get; set; } = default!;

    public decimal ValueCm { get; set; }
}

