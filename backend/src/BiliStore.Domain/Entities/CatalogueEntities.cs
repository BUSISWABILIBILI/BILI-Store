using BiliStore.Domain.Common;

namespace BiliStore.Domain.Entities;

public sealed class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}

public sealed class Product : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ClothingType { get; set; } = string.Empty;

    public int EstimatedProductionDays { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsFeatured { get; set; }

    public bool AllowsCustomColor { get; set; }

    public string? FabricDescription { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<ProductColour> AvailableColours { get; set; } = new List<ProductColour>();

    public ICollection<ProductMeasurementRequirement> MeasurementRequirements { get; set; } = new List<ProductMeasurementRequirement>();
}

public sealed class ProductCategory
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = default!;
}

public sealed class ProductImage : Entity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;

    public string Url { get; set; } = string.Empty;

    public string AltText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }
}

public sealed class Colour : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string HexCode { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<ProductColour> Products { get; set; } = new List<ProductColour>();
}

public sealed class ProductColour
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;

    public Guid ColourId { get; set; }

    public Colour Colour { get; set; } = default!;
}

public sealed class MeasurementType : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string Unit { get; set; } = "cm";

    public string Description { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public string Tip { get; set; } = string.Empty;

    public string? IllustrationUrl { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<ProductMeasurementRequirement> ProductRequirements { get; set; } = new List<ProductMeasurementRequirement>();
}

public sealed class ProductMeasurementRequirement : Entity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;

    public Guid MeasurementTypeId { get; set; }

    public MeasurementType MeasurementType { get; set; } = default!;

    public bool IsRequired { get; set; } = true;

    public decimal? MinValueCm { get; set; }

    public decimal? MaxValueCm { get; set; }

    public string? HelpText { get; set; }
}

