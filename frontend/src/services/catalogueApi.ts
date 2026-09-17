import { apiClient } from "./apiClient";
import type {
  CatalogueCategory,
  CatalogueColour,
  MeasurementType,
  ProductDetails,
  ProductQuery,
  ProductSummary,
} from "../types/catalogue";

export async function getProducts(query: ProductQuery = {}) {
  const response = await apiClient.get<ProductSummary[]>("/products", {
    params: cleanQuery(query),
  });

  return response.data;
}

export async function getProductBySlug(slug: string) {
  const response = await apiClient.get<ProductDetails>(`/products/${slug}`);

  return response.data;
}

export async function getCategories() {
  const response = await apiClient.get<CatalogueCategory[]>("/categories");

  return response.data;
}

export async function getColours() {
  const response = await apiClient.get<CatalogueColour[]>("/colours");

  return response.data;
}

export async function getMeasurements() {
  const response = await apiClient.get<MeasurementType[]>("/measurements");

  return response.data;
}

function cleanQuery(query: ProductQuery) {
  return Object.fromEntries(
    Object.entries(query).filter(([, value]) => value !== undefined && value !== null && value !== ""),
  );
}
