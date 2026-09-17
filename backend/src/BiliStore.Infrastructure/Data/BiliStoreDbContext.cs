using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Data;

public sealed class BiliStoreDbContext(DbContextOptions<BiliStoreDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<Colour> Colours => Set<Colour>();

    public DbSet<ProductColour> ProductColours => Set<ProductColour>();

    public DbSet<MeasurementType> MeasurementTypes => Set<MeasurementType>();

    public DbSet<ProductMeasurementRequirement> ProductMeasurementRequirements => Set<ProductMeasurementRequirement>();

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    public DbSet<CustomerMeasurementProfile> CustomerMeasurementProfiles => Set<CustomerMeasurementProfile>();

    public DbSet<CustomerMeasurement> CustomerMeasurements => Set<CustomerMeasurement>();

    public DbSet<BiliStore.Domain.Entities.Cart> Carts => Set<BiliStore.Domain.Entities.Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<CartItemMeasurement> CartItemMeasurements => Set<CartItemMeasurement>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<OrderItemMeasurement> OrderItemMeasurements => Set<OrderItemMeasurement>();

    public DbSet<DeliveryDetails> DeliveryDetails => Set<DeliveryDetails>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureCatalogue(builder);
        ConfigureCustomers(builder);
        ConfigureCart(builder);
        ConfigureOrders(builder);

        SeedData.Apply(builder);
    }

    private static void ConfigureCatalogue(ModelBuilder builder)
    {
        builder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name).HasMaxLength(120).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(140).IsRequired();
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.HasOne(category => category.ParentCategory)
                .WithMany(category => category.Children)
                .HasForeignKey(category => category.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name).HasMaxLength(180).IsRequired();
            entity.Property(product => product.Slug).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(2000).IsRequired();
            entity.Property(product => product.ClothingType).HasMaxLength(120).IsRequired();
            entity.Property(product => product.FabricDescription).HasMaxLength(500);
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.HasIndex(product => product.Slug).IsUnique();
        });

        builder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(productCategory => new { productCategory.ProductId, productCategory.CategoryId });
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(image => image.Url).HasMaxLength(1000).IsRequired();
            entity.Property(image => image.AltText).HasMaxLength(250).IsRequired();
        });

        builder.Entity<Colour>(entity =>
        {
            entity.Property(colour => colour.Name).HasMaxLength(80).IsRequired();
            entity.Property(colour => colour.HexCode).HasMaxLength(20).IsRequired();
            entity.HasIndex(colour => colour.Name).IsUnique();
        });

        builder.Entity<ProductColour>(entity =>
        {
            entity.HasKey(productColour => new { productColour.ProductId, productColour.ColourId });
        });

        builder.Entity<MeasurementType>(entity =>
        {
            entity.Property(measurement => measurement.Name).HasMaxLength(120).IsRequired();
            entity.Property(measurement => measurement.Key).HasMaxLength(80).IsRequired();
            entity.Property(measurement => measurement.Unit).HasMaxLength(20).IsRequired();
            entity.Property(measurement => measurement.Description).HasMaxLength(500).IsRequired();
            entity.Property(measurement => measurement.Instructions).HasMaxLength(1200).IsRequired();
            entity.Property(measurement => measurement.Tip).HasMaxLength(500).IsRequired();
            entity.Property(measurement => measurement.IllustrationUrl).HasMaxLength(1000);
            entity.HasIndex(measurement => measurement.Key).IsUnique();
        });

        builder.Entity<ProductMeasurementRequirement>(entity =>
        {
            entity.Property(requirement => requirement.MinValueCm).HasPrecision(8, 2);
            entity.Property(requirement => requirement.MaxValueCm).HasPrecision(8, 2);
            entity.Property(requirement => requirement.HelpText).HasMaxLength(500);
            entity.HasIndex(requirement => new { requirement.ProductId, requirement.MeasurementTypeId }).IsUnique();
        });
    }

    private static void ConfigureCustomers(ModelBuilder builder)
    {
        builder.Entity<CustomerProfile>(entity =>
        {
            entity.Property(customer => customer.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(customer => customer.LastName).HasMaxLength(100).IsRequired();
            entity.Property(customer => customer.PreferredPhone).HasMaxLength(40).IsRequired();
            entity.HasIndex(customer => customer.ApplicationUserId).IsUnique();
        });

        builder.Entity<CustomerMeasurementProfile>(entity =>
        {
            entity.Property(profile => profile.Name).HasMaxLength(120).IsRequired();
            entity.Property(profile => profile.Notes).HasMaxLength(500);
        });

        builder.Entity<CustomerMeasurement>(entity =>
        {
            entity.Property(measurement => measurement.ValueCm).HasPrecision(8, 2);
            entity.HasIndex(measurement => new { measurement.CustomerMeasurementProfileId, measurement.MeasurementTypeId }).IsUnique();
        });
    }

    private static void ConfigureCart(ModelBuilder builder)
    {
        builder.Entity<BiliStore.Domain.Entities.Cart>(entity =>
        {
            entity.Property(cart => cart.AnonymousCartKey).HasMaxLength(100);
            entity.HasIndex(cart => cart.AnonymousCartKey);
        });

        builder.Entity<CartItem>(entity =>
        {
            entity.Property(item => item.ProductSnapshotName).HasMaxLength(180).IsRequired();
            entity.Property(item => item.ProductSnapshotPrice).HasPrecision(18, 2);
            entity.Property(item => item.CustomColourName).HasMaxLength(80);
            entity.Property(item => item.CustomColourHex).HasMaxLength(20);
        });

        builder.Entity<CartItemMeasurement>(entity =>
        {
            entity.Property(measurement => measurement.ValueCm).HasPrecision(8, 2);
            entity.HasIndex(measurement => new { measurement.CartItemId, measurement.MeasurementTypeId }).IsUnique();
        });
    }

    private static void ConfigureOrders(ModelBuilder builder)
    {
        builder.Entity<Order>(entity =>
        {
            entity.Property(order => order.OrderNumber).HasMaxLength(40).IsRequired();
            entity.Property(order => order.GuestEmail).HasMaxLength(254);
            entity.Property(order => order.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(order => order.LastName).HasMaxLength(100).IsRequired();
            entity.Property(order => order.Email).HasMaxLength(254).IsRequired();
            entity.Property(order => order.Phone).HasMaxLength(40).IsRequired();
            entity.Property(order => order.Subtotal).HasPrecision(18, 2);
            entity.Property(order => order.DeliveryFee).HasPrecision(18, 2);
            entity.Property(order => order.Total).HasPrecision(18, 2);
            entity.HasIndex(order => order.OrderNumber).IsUnique();
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(item => item.ProductName).HasMaxLength(180).IsRequired();
            entity.Property(item => item.ProductDescription).HasMaxLength(2000).IsRequired();
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.Property(item => item.SelectedColourName).HasMaxLength(80);
            entity.Property(item => item.SelectedColourHex).HasMaxLength(20);
            entity.Property(item => item.CustomColourName).HasMaxLength(80);
            entity.Property(item => item.CustomColourHex).HasMaxLength(20);
        });

        builder.Entity<OrderItemMeasurement>(entity =>
        {
            entity.Property(measurement => measurement.MeasurementTypeName).HasMaxLength(120).IsRequired();
            entity.Property(measurement => measurement.MeasurementKey).HasMaxLength(80).IsRequired();
            entity.Property(measurement => measurement.ValueCm).HasPrecision(8, 2);
        });

        builder.Entity<DeliveryDetails>(entity =>
        {
            entity.Property(delivery => delivery.RecipientPhone).HasMaxLength(40).IsRequired();
            entity.Property(delivery => delivery.PaxiStoreCode).HasMaxLength(80).IsRequired();
            entity.Property(delivery => delivery.CollectionPointName).HasMaxLength(180).IsRequired();
            entity.Property(delivery => delivery.TownCity).HasMaxLength(120).IsRequired();
            entity.Property(delivery => delivery.Province).HasMaxLength(120).IsRequired();
            entity.Property(delivery => delivery.Instructions).HasMaxLength(500);
            entity.HasIndex(delivery => delivery.OrderId).IsUnique();
        });

        builder.Entity<Payment>(entity =>
        {
            entity.Property(payment => payment.Provider).HasMaxLength(80).IsRequired();
            entity.Property(payment => payment.ProviderReference).HasMaxLength(180);
            entity.Property(payment => payment.Amount).HasPrecision(18, 2);
            entity.Property(payment => payment.Currency).HasMaxLength(3).IsRequired();
            entity.Property(payment => payment.PaymentUrl).HasMaxLength(1000);
        });

        builder.Entity<ReturnRequest>(entity =>
        {
            entity.Property(returnRequest => returnRequest.Reason).HasMaxLength(180).IsRequired();
            entity.Property(returnRequest => returnRequest.Details).HasMaxLength(2000).IsRequired();
        });
    }
}
