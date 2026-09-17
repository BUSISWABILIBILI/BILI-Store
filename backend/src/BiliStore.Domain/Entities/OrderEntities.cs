using BiliStore.Domain.Common;
using BiliStore.Domain.Enums;

namespace BiliStore.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid? CustomerProfileId { get; set; }

    public CustomerProfile? CustomerProfile { get; set; }

    public string? GuestEmail { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal Total { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.PendingPayment;

    public DateTimeOffset? ReturnPolicyAcceptedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public DeliveryDetails? DeliveryDetails { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
}

public sealed class OrderItem : Entity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = default!;

    public Guid? ProductId { get; set; }

    public Product? Product { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductDescription { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public string? SelectedColourName { get; set; }

    public string? SelectedColourHex { get; set; }

    public string? CustomColourName { get; set; }

    public string? CustomColourHex { get; set; }

    public int ProductionDaysEstimate { get; set; }

    public ICollection<OrderItemMeasurement> Measurements { get; set; } = new List<OrderItemMeasurement>();
}

public sealed class OrderItemMeasurement : Entity
{
    public Guid OrderItemId { get; set; }

    public OrderItem OrderItem { get; set; } = default!;

    public string MeasurementTypeName { get; set; } = string.Empty;

    public string MeasurementKey { get; set; } = string.Empty;

    public decimal ValueCm { get; set; }
}

public sealed class DeliveryDetails : Entity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = default!;

    public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.PaxiPep;

    public string RecipientPhone { get; set; } = string.Empty;

    public string PaxiStoreCode { get; set; } = string.Empty;

    public string CollectionPointName { get; set; } = string.Empty;

    public string TownCity { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
}

public sealed class Payment : Entity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = default!;

    public string Provider { get; set; } = "Development";

    public string? ProviderReference { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "ZAR";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? PaymentUrl { get; set; }

    public DateTimeOffset? PaidAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ReturnRequest : Entity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = default!;

    public string Reason { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public ReturnRequestStatus Status { get; set; } = ReturnRequestStatus.Submitted;

    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ResolvedAt { get; set; }
}

