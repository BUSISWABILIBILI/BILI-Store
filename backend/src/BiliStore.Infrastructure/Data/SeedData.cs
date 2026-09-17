using BiliStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiliStore.Infrastructure.Data;

internal static class SeedData
{
    private static readonly DateTimeOffset SeededAt = new(2026, 9, 15, 0, 0, 0, TimeSpan.Zero);

    public static void Apply(ModelBuilder builder)
    {
        builder.Entity<Category>().HasData(
            Category(CategoryIds.Women, "Women", "women", 1),
            Category(CategoryIds.Men, "Men", "men", 2),
            Category(CategoryIds.Girls, "Girls", "girls", 3),
            Category(CategoryIds.Boys, "Boys", "boys", 4),
            Category(CategoryIds.Dresses, "Dresses", "dresses", 10),
            Category(CategoryIds.Skirts, "Skirts", "skirts", 11),
            Category(CategoryIds.Shorts, "Shorts", "shorts", 12),
            Category(CategoryIds.Pants, "Pants/Trousers", "pants-trousers", 13),
            Category(CategoryIds.Shirts, "Shirts", "shirts", 14),
            Category(CategoryIds.Blouses, "Blouses", "blouses", 15),
            Category(CategoryIds.Blazers, "Blazers", "blazers", 16),
            Category(CategoryIds.Jackets, "Jackets", "jackets", 17),
            Category(CategoryIds.Jumpsuits, "Jumpsuits", "jumpsuits", 18),
            Category(CategoryIds.TwoPiece, "Two-piece outfits", "two-piece-outfits", 19),
            Category(CategoryIds.TraditionalSets, "Traditional sets", "traditional-sets", 20),
            Category(CategoryIds.Suits, "Suits", "suits", 21),
            Category(CategoryIds.CustomOutfits, "Custom outfits", "custom-outfits", 22));

        builder.Entity<Colour>().HasData(
            Colour(ColourIds.Black, "Black", "#111111"),
            Colour(ColourIds.White, "White", "#ffffff"),
            Colour(ColourIds.Red, "Red", "#b91c1c"),
            Colour(ColourIds.Blue, "Blue", "#1d4ed8"),
            Colour(ColourIds.Green, "Green", "#15803d"),
            Colour(ColourIds.Yellow, "Yellow", "#facc15"),
            Colour(ColourIds.Burgundy, "Burgundy", "#7f1d1d"),
            Colour(ColourIds.Navy, "Navy", "#172554"),
            Colour(ColourIds.Beige, "Beige", "#d6c3a3"),
            Colour(ColourIds.Brown, "Brown", "#6f4e37"),
            Colour(ColourIds.Gold, "Gold", "#d4af37"));

        builder.Entity<MeasurementType>().HasData(
            Measurement(MeasurementTypeIds.BustChest, "Bust/Chest", "bust_chest", 1, "Measure around the fullest part of the bust or chest.", "Wrap the tape around the fullest part while keeping it level and comfortable.", "Do not pull the tape tight; leave natural breathing room."),
            Measurement(MeasurementTypeIds.Waist, "Waist", "waist", 2, "Measure around the natural waistline.", "Place the tape around the narrowest part of the torso, usually above the belly button.", "Stand naturally and avoid holding your stomach in."),
            Measurement(MeasurementTypeIds.Hips, "Hips", "hips", 3, "Measure around the fullest part of the hips and buttocks.", "Keep the tape parallel to the floor and measure around the widest point.", "Check the tape in a mirror or ask someone to help keep it level."),
            Measurement(MeasurementTypeIds.ShoulderWidth, "Shoulder width", "shoulder_width", 4, "Measure across the back from shoulder point to shoulder point.", "Start at the edge of one shoulder and measure straight across to the other.", "Keep shoulders relaxed."),
            Measurement(MeasurementTypeIds.Neck, "Neck", "neck", 5, "Measure around the base of the neck.", "Wrap the tape around where a collar naturally sits.", "Leave enough comfort for one finger under the tape."),
            Measurement(MeasurementTypeIds.SleeveLength, "Sleeve length", "sleeve_length", 6, "Measure from shoulder point to wrist.", "Bend the arm slightly and follow the outside of the arm down to the wrist.", "Slightly bent arms prevent sleeves from being too short."),
            Measurement(MeasurementTypeIds.Inseam, "Inseam", "inseam", 7, "Measure from the crotch seam down the inside leg.", "Measure to the desired trouser hem length.", "Wear shoes similar to what you plan to wear with the garment."),
            Measurement(MeasurementTypeIds.TrouserLength, "Trouser length", "trouser_length", 8, "Measure from the waist down to the desired trouser hem.", "Start at the waistline and measure down the outside of the leg.", "Stand straight for the most accurate length."),
            Measurement(MeasurementTypeIds.SkirtLength, "Skirt length", "skirt_length", 9, "Measure from the waist to the preferred skirt hem.", "Start at the natural waist and measure down to the desired finish point.", "Decide on the length before measuring."),
            Measurement(MeasurementTypeIds.DressLength, "Dress length", "dress_length", 10, "Measure from the shoulder or waist depending on dress style.", "For shoulder-to-hem dresses, start at the shoulder and measure down to the desired hem.", "Confirm whether the design uses shoulder or waist length."),
            Measurement(MeasurementTypeIds.Height, "Height", "height", 11, "Measure full body height.", "Stand straight against a wall without shoes and measure from floor to top of head.", "Keep heels, back, and head aligned against the wall."));

        builder.Entity<Product>().HasData(
            Product(ProductIds.BurgundyDress, "Burgundy Imvunulo Dress", "burgundy-imvunulo-dress", "A fitted custom dress with structured sleeves and patterned panel detailing for ceremonies, weddings, and formal celebrations.", 1850m, "Dress", 14, true, "Structured satin blend with traditional patterned contrast panels."),
            Product(ProductIds.CharcoalSuit, "Charcoal Traditional Suit", "charcoal-traditional-suit", "A tailored jacket and trouser set with refined traditional front-panel detail and a clean modern fit.", 2450m, "Suit", 16, true, "Suiting fabric with embroidered or printed traditional trim."),
            Product(ProductIds.IvoryTwoPiece, "Ivory Occasion Two-Piece", "ivory-occasion-two-piece", "A peplum top and skirt pairing designed for weddings, parties, and premium occasion wear.", 2100m, "Two-piece outfit", 15, true, "Textured ivory fabric with soft structure and custom trim options."),
            Product(ProductIds.FamilySet, "Family Celebration Set", "family-celebration-set", "A coordinated traditional set foundation for children and family event outfits.", 980m, "Traditional set", 10, true, "Cotton blend with optional matching patterned inserts."),
            Product(ProductIds.GirlsSkirtSet, "Girls Heritage Skirt Set", "girls-heritage-skirt-set", "A bright custom skirt and top set for birthdays, school celebrations, and cultural events.", 860m, "Skirt set", 8, false, "Soft cotton blend with decorative traditional fabric panels."),
            Product(ProductIds.NavyShirt, "Navy Panel Shirt", "navy-panel-shirt", "A smart custom shirt with contrast front panel detail for events and everyday traditional styling.", 720m, "Shirt", 7, false, "Breathable cotton shirting with contrast detail."),
            Product(ProductIds.GoldJumpsuit, "Gold Detail Jumpsuit", "gold-detail-jumpsuit", "A modern custom jumpsuit with a fitted bodice, wide-leg silhouette, and statement detailing.", 1950m, "Jumpsuit", 13, false, "Crepe blend with decorative gold-inspired detailing."),
            Product(ProductIds.BoysShirtShorts, "Boys Shirt and Shorts Set", "boys-shirt-shorts-set", "A comfortable custom shirt and shorts set for young boys attending family ceremonies.", 690m, "Shorts set", 7, false, "Cotton blend with contrast trim."));

        builder.Entity<ProductImage>().HasData(
            ProductImage(ProductImageIds.BurgundyDressPrimary, ProductIds.BurgundyDress, "/images/bili-atelier-hero.png", "Burgundy custom traditional dress displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.CharcoalSuitPrimary, ProductIds.CharcoalSuit, "/images/bili-atelier-hero.png", "Charcoal traditional suit displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.IvoryTwoPiecePrimary, ProductIds.IvoryTwoPiece, "/images/bili-atelier-hero.png", "Ivory occasion two-piece displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.FamilySetPrimary, ProductIds.FamilySet, "/images/bili-atelier-hero.png", "Family traditional celebration set displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.GirlsSkirtSetPrimary, ProductIds.GirlsSkirtSet, "/images/bili-atelier-hero.png", "Girls heritage skirt set displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.NavyShirtPrimary, ProductIds.NavyShirt, "/images/bili-atelier-hero.png", "Navy panel shirt displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.GoldJumpsuitPrimary, ProductIds.GoldJumpsuit, "/images/bili-atelier-hero.png", "Gold detail jumpsuit displayed in the BILI Store atelier", 1, true),
            ProductImage(ProductImageIds.BoysShirtShortsPrimary, ProductIds.BoysShirtShorts, "/images/bili-atelier-hero.png", "Boys shirt and shorts set displayed in the BILI Store atelier", 1, true));

        builder.Entity<ProductCategory>().HasData(
            ProductCategory(ProductIds.BurgundyDress, CategoryIds.Women),
            ProductCategory(ProductIds.BurgundyDress, CategoryIds.Dresses),
            ProductCategory(ProductIds.CharcoalSuit, CategoryIds.Men),
            ProductCategory(ProductIds.CharcoalSuit, CategoryIds.Suits),
            ProductCategory(ProductIds.IvoryTwoPiece, CategoryIds.Women),
            ProductCategory(ProductIds.IvoryTwoPiece, CategoryIds.TwoPiece),
            ProductCategory(ProductIds.FamilySet, CategoryIds.Boys),
            ProductCategory(ProductIds.FamilySet, CategoryIds.TraditionalSets),
            ProductCategory(ProductIds.GirlsSkirtSet, CategoryIds.Girls),
            ProductCategory(ProductIds.GirlsSkirtSet, CategoryIds.Skirts),
            ProductCategory(ProductIds.NavyShirt, CategoryIds.Men),
            ProductCategory(ProductIds.NavyShirt, CategoryIds.Shirts),
            ProductCategory(ProductIds.GoldJumpsuit, CategoryIds.Women),
            ProductCategory(ProductIds.GoldJumpsuit, CategoryIds.Jumpsuits),
            ProductCategory(ProductIds.BoysShirtShorts, CategoryIds.Boys),
            ProductCategory(ProductIds.BoysShirtShorts, CategoryIds.Shorts),
            ProductCategory(ProductIds.BoysShirtShorts, CategoryIds.Shirts));

        builder.Entity<ProductColour>().HasData(
            ProductColour(ProductIds.BurgundyDress, ColourIds.Burgundy),
            ProductColour(ProductIds.BurgundyDress, ColourIds.Gold),
            ProductColour(ProductIds.BurgundyDress, ColourIds.Black),
            ProductColour(ProductIds.CharcoalSuit, ColourIds.Black),
            ProductColour(ProductIds.CharcoalSuit, ColourIds.Navy),
            ProductColour(ProductIds.CharcoalSuit, ColourIds.Gold),
            ProductColour(ProductIds.IvoryTwoPiece, ColourIds.White),
            ProductColour(ProductIds.IvoryTwoPiece, ColourIds.Beige),
            ProductColour(ProductIds.IvoryTwoPiece, ColourIds.Brown),
            ProductColour(ProductIds.FamilySet, ColourIds.Black),
            ProductColour(ProductIds.FamilySet, ColourIds.Burgundy),
            ProductColour(ProductIds.FamilySet, ColourIds.White),
            ProductColour(ProductIds.GirlsSkirtSet, ColourIds.Red),
            ProductColour(ProductIds.GirlsSkirtSet, ColourIds.Yellow),
            ProductColour(ProductIds.GirlsSkirtSet, ColourIds.Green),
            ProductColour(ProductIds.NavyShirt, ColourIds.Navy),
            ProductColour(ProductIds.NavyShirt, ColourIds.White),
            ProductColour(ProductIds.NavyShirt, ColourIds.Gold),
            ProductColour(ProductIds.GoldJumpsuit, ColourIds.Gold),
            ProductColour(ProductIds.GoldJumpsuit, ColourIds.Black),
            ProductColour(ProductIds.GoldJumpsuit, ColourIds.Brown),
            ProductColour(ProductIds.BoysShirtShorts, ColourIds.Blue),
            ProductColour(ProductIds.BoysShirtShorts, ColourIds.White),
            ProductColour(ProductIds.BoysShirtShorts, ColourIds.Burgundy));

        builder.Entity<ProductMeasurementRequirement>().HasData(
            Requirement(RequirementIds.BurgundyDressBust, ProductIds.BurgundyDress, MeasurementTypeIds.BustChest, "Needed so the bodice fits comfortably."),
            Requirement(RequirementIds.BurgundyDressWaist, ProductIds.BurgundyDress, MeasurementTypeIds.Waist, "Needed for the fitted waistline."),
            Requirement(RequirementIds.BurgundyDressHips, ProductIds.BurgundyDress, MeasurementTypeIds.Hips, "Needed for skirt shaping."),
            Requirement(RequirementIds.BurgundyDressShoulder, ProductIds.BurgundyDress, MeasurementTypeIds.ShoulderWidth, "Needed for sleeve and neckline balance."),
            Requirement(RequirementIds.BurgundyDressLength, ProductIds.BurgundyDress, MeasurementTypeIds.DressLength, "Choose the final dress length."),
            Requirement(RequirementIds.CharcoalSuitChest, ProductIds.CharcoalSuit, MeasurementTypeIds.BustChest, "Used as the chest measurement for the jacket."),
            Requirement(RequirementIds.CharcoalSuitWaist, ProductIds.CharcoalSuit, MeasurementTypeIds.Waist, "Needed for both jacket and trouser fit."),
            Requirement(RequirementIds.CharcoalSuitShoulder, ProductIds.CharcoalSuit, MeasurementTypeIds.ShoulderWidth, "Needed for jacket structure."),
            Requirement(RequirementIds.CharcoalSuitSleeve, ProductIds.CharcoalSuit, MeasurementTypeIds.SleeveLength, "Needed for jacket sleeve length."),
            Requirement(RequirementIds.CharcoalSuitTrouserLength, ProductIds.CharcoalSuit, MeasurementTypeIds.TrouserLength, "Choose the preferred trouser finish."),
            Requirement(RequirementIds.IvoryTwoPieceBust, ProductIds.IvoryTwoPiece, MeasurementTypeIds.BustChest, "Needed for the top fit."),
            Requirement(RequirementIds.IvoryTwoPieceWaist, ProductIds.IvoryTwoPiece, MeasurementTypeIds.Waist, "Needed for top and skirt fit."),
            Requirement(RequirementIds.IvoryTwoPieceHips, ProductIds.IvoryTwoPiece, MeasurementTypeIds.Hips, "Needed for skirt shaping."),
            Requirement(RequirementIds.IvoryTwoPieceSkirtLength, ProductIds.IvoryTwoPiece, MeasurementTypeIds.SkirtLength, "Choose the preferred skirt length."),
            Requirement(RequirementIds.FamilySetChest, ProductIds.FamilySet, MeasurementTypeIds.BustChest, "Used as the chest measurement for the top."),
            Requirement(RequirementIds.FamilySetWaist, ProductIds.FamilySet, MeasurementTypeIds.Waist, "Needed for trousers or bottoms."),
            Requirement(RequirementIds.FamilySetShoulder, ProductIds.FamilySet, MeasurementTypeIds.ShoulderWidth, "Needed for top fit."),
            Requirement(RequirementIds.FamilySetHeight, ProductIds.FamilySet, MeasurementTypeIds.Height, "Helps scale the child garment correctly."),
            Requirement(RequirementIds.GirlsSkirtSetWaist, ProductIds.GirlsSkirtSet, MeasurementTypeIds.Waist, "Needed for skirt waistband."),
            Requirement(RequirementIds.GirlsSkirtSetHips, ProductIds.GirlsSkirtSet, MeasurementTypeIds.Hips, "Needed for skirt ease."),
            Requirement(RequirementIds.GirlsSkirtSetSkirtLength, ProductIds.GirlsSkirtSet, MeasurementTypeIds.SkirtLength, "Choose the preferred skirt length."),
            Requirement(RequirementIds.NavyShirtChest, ProductIds.NavyShirt, MeasurementTypeIds.BustChest, "Used as the chest measurement for the shirt."),
            Requirement(RequirementIds.NavyShirtNeck, ProductIds.NavyShirt, MeasurementTypeIds.Neck, "Needed for collar comfort."),
            Requirement(RequirementIds.NavyShirtShoulder, ProductIds.NavyShirt, MeasurementTypeIds.ShoulderWidth, "Needed for shirt shoulder fit."),
            Requirement(RequirementIds.NavyShirtSleeve, ProductIds.NavyShirt, MeasurementTypeIds.SleeveLength, "Choose the preferred sleeve length."),
            Requirement(RequirementIds.GoldJumpsuitBust, ProductIds.GoldJumpsuit, MeasurementTypeIds.BustChest, "Needed for bodice fit."),
            Requirement(RequirementIds.GoldJumpsuitWaist, ProductIds.GoldJumpsuit, MeasurementTypeIds.Waist, "Needed for fitted waist shaping."),
            Requirement(RequirementIds.GoldJumpsuitHips, ProductIds.GoldJumpsuit, MeasurementTypeIds.Hips, "Needed for lower-body ease."),
            Requirement(RequirementIds.GoldJumpsuitInseam, ProductIds.GoldJumpsuit, MeasurementTypeIds.Inseam, "Needed for jumpsuit trouser fit."),
            Requirement(RequirementIds.BoysSetChest, ProductIds.BoysShirtShorts, MeasurementTypeIds.BustChest, "Used as the chest measurement for the shirt."),
            Requirement(RequirementIds.BoysSetWaist, ProductIds.BoysShirtShorts, MeasurementTypeIds.Waist, "Needed for shorts fit."),
            Requirement(RequirementIds.BoysSetHeight, ProductIds.BoysShirtShorts, MeasurementTypeIds.Height, "Helps scale the child garment correctly."));
    }

    private static Category Category(Guid id, string name, string slug, int displayOrder)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Slug = slug,
            DisplayOrder = displayOrder,
            IsActive = true,
            CreatedAt = SeededAt
        };
    }

    private static Colour Colour(Guid id, string name, string hexCode)
    {
        return new Colour
        {
            Id = id,
            Name = name,
            HexCode = hexCode,
            IsActive = true,
            CreatedAt = SeededAt
        };
    }

    private static MeasurementType Measurement(Guid id, string name, string key, int displayOrder, string description, string instructions, string tip)
    {
        return new MeasurementType
        {
            Id = id,
            Name = name,
            Key = key,
            Unit = "cm",
            DisplayOrder = displayOrder,
            Description = description,
            Instructions = instructions,
            Tip = tip,
            CreatedAt = SeededAt
        };
    }

    private static Product Product(
        Guid id,
        string name,
        string slug,
        string description,
        decimal price,
        string clothingType,
        int estimatedProductionDays,
        bool isFeatured,
        string fabricDescription)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Slug = slug,
            Description = description,
            Price = price,
            ClothingType = clothingType,
            EstimatedProductionDays = estimatedProductionDays,
            IsAvailable = true,
            IsFeatured = isFeatured,
            AllowsCustomColor = true,
            FabricDescription = fabricDescription,
            CreatedAt = SeededAt
        };
    }

    private static ProductImage ProductImage(Guid id, Guid productId, string url, string altText, int displayOrder, bool isPrimary)
    {
        return new ProductImage
        {
            Id = id,
            ProductId = productId,
            Url = url,
            AltText = altText,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary
        };
    }

    private static ProductCategory ProductCategory(Guid productId, Guid categoryId)
    {
        return new ProductCategory
        {
            ProductId = productId,
            CategoryId = categoryId
        };
    }

    private static ProductColour ProductColour(Guid productId, Guid colourId)
    {
        return new ProductColour
        {
            ProductId = productId,
            ColourId = colourId
        };
    }

    private static ProductMeasurementRequirement Requirement(Guid id, Guid productId, Guid measurementTypeId, string helpText)
    {
        return new ProductMeasurementRequirement
        {
            Id = id,
            ProductId = productId,
            MeasurementTypeId = measurementTypeId,
            IsRequired = true,
            MinValueCm = 10,
            MaxValueCm = 250,
            HelpText = helpText
        };
    }

    private static class CategoryIds
    {
        public static readonly Guid Women = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Men = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Girls = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid Boys = Guid.Parse("10000000-0000-0000-0000-000000000004");
        public static readonly Guid Dresses = Guid.Parse("10000000-0000-0000-0000-000000000010");
        public static readonly Guid Skirts = Guid.Parse("10000000-0000-0000-0000-000000000011");
        public static readonly Guid Shorts = Guid.Parse("10000000-0000-0000-0000-000000000012");
        public static readonly Guid Pants = Guid.Parse("10000000-0000-0000-0000-000000000013");
        public static readonly Guid Shirts = Guid.Parse("10000000-0000-0000-0000-000000000014");
        public static readonly Guid Blouses = Guid.Parse("10000000-0000-0000-0000-000000000015");
        public static readonly Guid Blazers = Guid.Parse("10000000-0000-0000-0000-000000000016");
        public static readonly Guid Jackets = Guid.Parse("10000000-0000-0000-0000-000000000017");
        public static readonly Guid Jumpsuits = Guid.Parse("10000000-0000-0000-0000-000000000018");
        public static readonly Guid TwoPiece = Guid.Parse("10000000-0000-0000-0000-000000000019");
        public static readonly Guid TraditionalSets = Guid.Parse("10000000-0000-0000-0000-000000000020");
        public static readonly Guid Suits = Guid.Parse("10000000-0000-0000-0000-000000000021");
        public static readonly Guid CustomOutfits = Guid.Parse("10000000-0000-0000-0000-000000000022");
    }

    private static class ColourIds
    {
        public static readonly Guid Black = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid White = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Red = Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Blue = Guid.Parse("20000000-0000-0000-0000-000000000004");
        public static readonly Guid Green = Guid.Parse("20000000-0000-0000-0000-000000000005");
        public static readonly Guid Yellow = Guid.Parse("20000000-0000-0000-0000-000000000006");
        public static readonly Guid Burgundy = Guid.Parse("20000000-0000-0000-0000-000000000007");
        public static readonly Guid Navy = Guid.Parse("20000000-0000-0000-0000-000000000008");
        public static readonly Guid Beige = Guid.Parse("20000000-0000-0000-0000-000000000009");
        public static readonly Guid Brown = Guid.Parse("20000000-0000-0000-0000-000000000010");
        public static readonly Guid Gold = Guid.Parse("20000000-0000-0000-0000-000000000011");
    }

    private static class MeasurementTypeIds
    {
        public static readonly Guid BustChest = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid Waist = Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid Hips = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid ShoulderWidth = Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid Neck = Guid.Parse("30000000-0000-0000-0000-000000000005");
        public static readonly Guid SleeveLength = Guid.Parse("30000000-0000-0000-0000-000000000006");
        public static readonly Guid Inseam = Guid.Parse("30000000-0000-0000-0000-000000000007");
        public static readonly Guid TrouserLength = Guid.Parse("30000000-0000-0000-0000-000000000008");
        public static readonly Guid SkirtLength = Guid.Parse("30000000-0000-0000-0000-000000000009");
        public static readonly Guid DressLength = Guid.Parse("30000000-0000-0000-0000-000000000010");
        public static readonly Guid Height = Guid.Parse("30000000-0000-0000-0000-000000000011");
    }

    private static class ProductIds
    {
        public static readonly Guid BurgundyDress = Guid.Parse("40000000-0000-0000-0000-000000000001");
        public static readonly Guid CharcoalSuit = Guid.Parse("40000000-0000-0000-0000-000000000002");
        public static readonly Guid IvoryTwoPiece = Guid.Parse("40000000-0000-0000-0000-000000000003");
        public static readonly Guid FamilySet = Guid.Parse("40000000-0000-0000-0000-000000000004");
        public static readonly Guid GirlsSkirtSet = Guid.Parse("40000000-0000-0000-0000-000000000005");
        public static readonly Guid NavyShirt = Guid.Parse("40000000-0000-0000-0000-000000000006");
        public static readonly Guid GoldJumpsuit = Guid.Parse("40000000-0000-0000-0000-000000000007");
        public static readonly Guid BoysShirtShorts = Guid.Parse("40000000-0000-0000-0000-000000000008");
    }

    private static class ProductImageIds
    {
        public static readonly Guid BurgundyDressPrimary = Guid.Parse("50000000-0000-0000-0000-000000000001");
        public static readonly Guid CharcoalSuitPrimary = Guid.Parse("50000000-0000-0000-0000-000000000002");
        public static readonly Guid IvoryTwoPiecePrimary = Guid.Parse("50000000-0000-0000-0000-000000000003");
        public static readonly Guid FamilySetPrimary = Guid.Parse("50000000-0000-0000-0000-000000000004");
        public static readonly Guid GirlsSkirtSetPrimary = Guid.Parse("50000000-0000-0000-0000-000000000005");
        public static readonly Guid NavyShirtPrimary = Guid.Parse("50000000-0000-0000-0000-000000000006");
        public static readonly Guid GoldJumpsuitPrimary = Guid.Parse("50000000-0000-0000-0000-000000000007");
        public static readonly Guid BoysShirtShortsPrimary = Guid.Parse("50000000-0000-0000-0000-000000000008");
    }

    private static class RequirementIds
    {
        public static readonly Guid BurgundyDressBust = Guid.Parse("60000000-0000-0000-0000-000000000001");
        public static readonly Guid BurgundyDressWaist = Guid.Parse("60000000-0000-0000-0000-000000000002");
        public static readonly Guid BurgundyDressHips = Guid.Parse("60000000-0000-0000-0000-000000000003");
        public static readonly Guid BurgundyDressShoulder = Guid.Parse("60000000-0000-0000-0000-000000000004");
        public static readonly Guid BurgundyDressLength = Guid.Parse("60000000-0000-0000-0000-000000000005");
        public static readonly Guid CharcoalSuitChest = Guid.Parse("60000000-0000-0000-0000-000000000006");
        public static readonly Guid CharcoalSuitWaist = Guid.Parse("60000000-0000-0000-0000-000000000007");
        public static readonly Guid CharcoalSuitShoulder = Guid.Parse("60000000-0000-0000-0000-000000000008");
        public static readonly Guid CharcoalSuitSleeve = Guid.Parse("60000000-0000-0000-0000-000000000009");
        public static readonly Guid CharcoalSuitTrouserLength = Guid.Parse("60000000-0000-0000-0000-000000000010");
        public static readonly Guid IvoryTwoPieceBust = Guid.Parse("60000000-0000-0000-0000-000000000011");
        public static readonly Guid IvoryTwoPieceWaist = Guid.Parse("60000000-0000-0000-0000-000000000012");
        public static readonly Guid IvoryTwoPieceHips = Guid.Parse("60000000-0000-0000-0000-000000000013");
        public static readonly Guid IvoryTwoPieceSkirtLength = Guid.Parse("60000000-0000-0000-0000-000000000014");
        public static readonly Guid FamilySetChest = Guid.Parse("60000000-0000-0000-0000-000000000015");
        public static readonly Guid FamilySetWaist = Guid.Parse("60000000-0000-0000-0000-000000000016");
        public static readonly Guid FamilySetShoulder = Guid.Parse("60000000-0000-0000-0000-000000000017");
        public static readonly Guid FamilySetHeight = Guid.Parse("60000000-0000-0000-0000-000000000018");
        public static readonly Guid GirlsSkirtSetWaist = Guid.Parse("60000000-0000-0000-0000-000000000019");
        public static readonly Guid GirlsSkirtSetHips = Guid.Parse("60000000-0000-0000-0000-000000000020");
        public static readonly Guid GirlsSkirtSetSkirtLength = Guid.Parse("60000000-0000-0000-0000-000000000021");
        public static readonly Guid NavyShirtChest = Guid.Parse("60000000-0000-0000-0000-000000000022");
        public static readonly Guid NavyShirtNeck = Guid.Parse("60000000-0000-0000-0000-000000000023");
        public static readonly Guid NavyShirtShoulder = Guid.Parse("60000000-0000-0000-0000-000000000024");
        public static readonly Guid NavyShirtSleeve = Guid.Parse("60000000-0000-0000-0000-000000000025");
        public static readonly Guid GoldJumpsuitBust = Guid.Parse("60000000-0000-0000-0000-000000000026");
        public static readonly Guid GoldJumpsuitWaist = Guid.Parse("60000000-0000-0000-0000-000000000027");
        public static readonly Guid GoldJumpsuitHips = Guid.Parse("60000000-0000-0000-0000-000000000028");
        public static readonly Guid GoldJumpsuitInseam = Guid.Parse("60000000-0000-0000-0000-000000000029");
        public static readonly Guid BoysSetChest = Guid.Parse("60000000-0000-0000-0000-000000000030");
        public static readonly Guid BoysSetWaist = Guid.Parse("60000000-0000-0000-0000-000000000031");
        public static readonly Guid BoysSetHeight = Guid.Parse("60000000-0000-0000-0000-000000000032");
    }
}
