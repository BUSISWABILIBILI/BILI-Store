export type StoreCategory = {
  label: string;
  href: string;
  description: string;
  imagePosition: string;
  accent: "burgundy" | "charcoal" | "olive" | "indigo";
};

export type ProductPreview = {
  id: string;
  name: string;
  category: "Women" | "Men" | "Girls" | "Boys";
  clothingType: string;
  description: string;
  price: number;
  productionTime: string;
  badge: string;
  image: string;
  imagePosition: string;
  colors: Array<{
    name: string;
    hex: string;
  }>;
  requiredMeasurements: string[];
};

export type OrderingStep = {
  number: string;
  title: string;
  description: string;
};

export type MeasurementGuideItem = {
  name: string;
  summary: string;
  steps: string[];
  tip: string;
};

export const heroImage = "/images/bili-atelier-hero.png";

export const storeCategories: StoreCategory[] = [
  {
    label: "Women",
    href: "/shop?category=Women",
    description: "Dresses, skirts, blouses, jackets, jumpsuits, and statement two-piece sets.",
    imagePosition: "62% 52%",
    accent: "burgundy",
  },
  {
    label: "Men",
    href: "/shop?category=Men",
    description: "Traditional shirts, suits, jackets, trousers, and made-to-measure sets.",
    imagePosition: "72% 40%",
    accent: "charcoal",
  },
  {
    label: "Girls",
    href: "/shop?category=Girls",
    description: "Occasion-ready dresses, skirts, tops, and custom traditional outfits.",
    imagePosition: "48% 56%",
    accent: "olive",
  },
  {
    label: "Boys",
    href: "/shop?category=Boys",
    description: "Smart shirts, shorts, pants, jackets, and coordinated celebration sets.",
    imagePosition: "78% 58%",
    accent: "indigo",
  },
];

export const featuredProducts: ProductPreview[] = [
  {
    id: "burgundy-imvunulo-dress",
    name: "Burgundy Imvunulo Dress",
    category: "Women",
    clothingType: "Dress",
    description: "A fitted custom dress with structured sleeves and patterned panel detailing.",
    price: 1850,
    productionTime: "10-14 sewing days",
    badge: "Featured",
    image: heroImage,
    imagePosition: "43% 50%",
    colors: [
      { name: "Burgundy", hex: "#7a1f2b" },
      { name: "Gold", hex: "#c99a2e" },
      { name: "Black", hex: "#171412" },
    ],
    requiredMeasurements: ["Bust", "Waist", "Hips", "Shoulder", "Dress length"],
  },
  {
    id: "charcoal-traditional-suit",
    name: "Charcoal Traditional Suit",
    category: "Men",
    clothingType: "Suit",
    description: "A tailored jacket and trouser set with refined traditional front-panel detail.",
    price: 2450,
    productionTime: "12-16 sewing days",
    badge: "Custom fit",
    image: heroImage,
    imagePosition: "57% 45%",
    colors: [
      { name: "Charcoal", hex: "#262321" },
      { name: "Navy", hex: "#1d2f46" },
      { name: "Gold", hex: "#c99a2e" },
    ],
    requiredMeasurements: ["Chest", "Waist", "Shoulder", "Sleeve", "Trouser length"],
  },
  {
    id: "ivory-occasion-two-piece",
    name: "Ivory Occasion Two-Piece",
    category: "Women",
    clothingType: "Two-piece outfit",
    description: "A peplum top and skirt pairing designed for weddings, parties, and ceremonies.",
    price: 2100,
    productionTime: "10-15 sewing days",
    badge: "Occasion wear",
    image: heroImage,
    imagePosition: "70% 49%",
    colors: [
      { name: "Ivory", hex: "#f6eee4" },
      { name: "Beige", hex: "#c7a982" },
      { name: "Brown", hex: "#6d4634" },
    ],
    requiredMeasurements: ["Bust", "Waist", "Hips", "Skirt length"],
  },
  {
    id: "family-celebration-set",
    name: "Family Celebration Set",
    category: "Boys",
    clothingType: "Traditional set",
    description: "A coordinated custom set foundation for children and family event outfits.",
    price: 980,
    productionTime: "7-10 sewing days",
    badge: "Family ready",
    image: heroImage,
    imagePosition: "86% 52%",
    colors: [
      { name: "Black", hex: "#171412" },
      { name: "Burgundy", hex: "#7a1f2b" },
      { name: "White", hex: "#ffffff" },
    ],
    requiredMeasurements: ["Chest", "Waist", "Shoulder", "Height"],
  },
];

export const orderingSteps: OrderingStep[] = [
  {
    number: "01",
    title: "Choose the outfit",
    description: "Customers browse by Women, Men, Girls, Boys, clothing type, and featured designs.",
  },
  {
    number: "02",
    title: "Pick the colour",
    description: "They select a swatch or request a custom colour that stays attached to the cart item.",
  },
  {
    number: "03",
    title: "Enter measurements",
    description: "Only the measurements required for that garment are collected and validated.",
  },
  {
    number: "04",
    title: "Checkout for PAXI/PEP",
    description: "The order keeps customer details, collection point information, totals, and policy acceptance.",
  },
];

export const measurementGuideItems: MeasurementGuideItem[] = [
  {
    name: "Bust / Chest",
    summary: "Measure around the fullest part of the bust or chest while keeping the tape level.",
    steps: [
      "Stand upright with relaxed shoulders.",
      "Wrap the tape around the fullest part of the bust or chest.",
      "Keep the tape comfortable, not tight.",
    ],
    tip: "Ask someone to help if the tape keeps dropping at the back.",
  },
  {
    name: "Waist",
    summary: "Measure around the natural waistline, usually the narrowest part of the torso.",
    steps: [
      "Find the natural waist by bending gently to one side.",
      "Place the tape around that point.",
      "Leave enough room to breathe comfortably.",
    ],
    tip: "Do not pull the tape too tightly, especially for fitted garments.",
  },
  {
    name: "Hips",
    summary: "Measure around the fullest part of the hips and buttocks.",
    steps: [
      "Stand with feet close together.",
      "Wrap the tape around the widest hip point.",
      "Check that the tape is level all the way around.",
    ],
    tip: "This measurement is important for skirts, dresses, pants, and jumpsuits.",
  },
  {
    name: "Shoulder",
    summary: "Measure from one shoulder point to the other across the back.",
    steps: [
      "Locate the edge of each shoulder bone.",
      "Measure straight across the upper back.",
      "Keep the tape flat without curving it around the neck.",
    ],
    tip: "This helps jackets, shirts, and blouses sit correctly.",
  },
];
