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
}

