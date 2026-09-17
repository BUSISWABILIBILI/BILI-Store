export type CatalogueCategory = {
  id: string;
  name: string;
  slug: string;
  parentSlug: string | null;
  displayOrder: number;
};

export type CatalogueColour = {
  id: string;
  name: string;
  hexCode: string;
};

export type MeasurementType = {
  id: string;
  name: string;
  key: string;
  unit: string;
  description: string;
  instructions: string;
  tip: string;
  illustrationUrl: string | null;
  displayOrder: number;
};

export type ProductImage = {
  id: string;
  url: string;
  altText: string;
  displayOrder: number;
  isPrimary: boolean;
};

export type ProductMeasurementRequirement = {
  id: string;
  measurementTypeId: string;
  name: string;
  key: string;
  unit: string;
  isRequired: boolean;
  minValueCm: number | null;
  maxValueCm: number | null;
  helpText: string | null;
};

export type ProductSummary = {
  id: string;
  name: string;
  slug: string;
  description: string;
  price: number;
  clothingType: string;
  estimatedProductionDays: number;
  isAvailable: boolean;
  isFeatured: boolean;
  allowsCustomColor: boolean;
  fabricDescription: string | null;
  primaryImageUrl: string;
  primaryImageAltText: string;
  categories: CatalogueCategory[];
  availableColours: CatalogueColour[];
  requiredMeasurements: string[];
};

export type ProductDetails = ProductSummary & {
  images: ProductImage[];
  measurementRequirements: ProductMeasurementRequirement[];
};

export type ProductQuery = {
  search?: string;
  category?: string;
  clothingType?: string;
  minPrice?: number;
  maxPrice?: number;
  featured?: boolean;
};
