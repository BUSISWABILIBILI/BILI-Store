import type { CSSProperties, FormEvent } from "react";
import { useEffect, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Boxes, Palette, Plus, Ruler, Save, ShieldCheck, Trash2 } from "lucide-react";
import {
  createAdminCategory,
  createAdminColour,
  createAdminProduct,
  deactivateAdminProduct,
  getAdminCategories,
  getAdminColours,
  getAdminProduct,
  getAdminProducts,
  updateAdminCategory,
  updateAdminColour,
  updateAdminProduct,
} from "../services/adminApi";
import { getMeasurements } from "../services/catalogueApi";
import { useAuthStore } from "../stores/authStore";
import type {
  AdminCategory,
  AdminColour,
  AdminProductDetails,
  UpsertAdminCategoryRequest,
  UpsertAdminColourRequest,
  UpsertAdminProductRequest,
} from "../types/admin";
import { formatCurrency } from "../utils/formatters";

type MeasurementDraft = {
  isRequired: boolean;
  minValueCm: string;
  maxValueCm: string;
  helpText: string;
};

type ProductFormState = {
  name: string;
  slug: string;
  description: string;
  price: string;
  clothingType: string;
  estimatedProductionDays: string;
  isAvailable: boolean;
  isFeatured: boolean;
  allowsCustomColor: boolean;
  fabricDescription: string;
  primaryImageUrl: string;
  primaryImageAltText: string;
  categoryIds: string[];
  colourIds: string[];
  measurementRequirements: Record<string, MeasurementDraft>;
};

type CategoryFormState = {
  name: string;
  slug: string;
  parentCategoryId: string;
  displayOrder: string;
  isActive: boolean;
};

type ColourFormState = {
  name: string;
  hexCode: string;
  isActive: boolean;
};

const defaultImageUrl = "/images/bili-atelier-hero.png";

function emptyProductForm(): ProductFormState {
  return {
    name: "",
    slug: "",
    description: "",
    price: "0",
    clothingType: "Custom Clothing",
    estimatedProductionDays: "14",
    isAvailable: true,
    isFeatured: false,
    allowsCustomColor: true,
    fabricDescription: "",
    primaryImageUrl: defaultImageUrl,
    primaryImageAltText: "",
    categoryIds: [],
    colourIds: [],
    measurementRequirements: {},
  };
}

function emptyCategoryForm(): CategoryFormState {
  return {
    name: "",
    slug: "",
    parentCategoryId: "",
    displayOrder: "0",
    isActive: true,
  };
}

function emptyColourForm(): ColourFormState {
  return {
    name: "",
    hexCode: "#7a2d2a",
    isActive: true,
  };
}

function optionalText(value: string) {
  const trimmed = value.trim();
  return trimmed.length ? trimmed : null;
}

function optionalNumber(value: string) {
  const parsed = Number.parseFloat(value);
  return Number.isFinite(parsed) ? parsed : null;
}

function requiredNumber(value: string, fallback: number) {
  const parsed = Number.parseFloat(value);
  return Number.isFinite(parsed) ? parsed : fallback;
}

function requiredInteger(value: string, fallback: number) {
  const parsed = Number.parseInt(value, 10);
  return Number.isFinite(parsed) ? parsed : fallback;
}

function toggleId(values: string[], id: string) {
  return values.includes(id) ? values.filter((value) => value !== id) : [...values, id];
}

function toProductForm(product: AdminProductDetails): ProductFormState {
  return {
    name: product.name,
    slug: product.slug,
    description: product.description,
    price: String(product.price),
    clothingType: product.clothingType,
    estimatedProductionDays: String(product.estimatedProductionDays),
    isAvailable: product.isAvailable,
    isFeatured: product.isFeatured,
    allowsCustomColor: product.allowsCustomColor,
    fabricDescription: product.fabricDescription ?? "",
    primaryImageUrl: product.primaryImageUrl,
    primaryImageAltText: product.primaryImageAltText,
    categoryIds: product.categoryIds,
    colourIds: product.colourIds,
    measurementRequirements: Object.fromEntries(
      product.measurementRequirements.map((requirement) => [
        requirement.measurementTypeId,
        {
          isRequired: requirement.isRequired,
          minValueCm: requirement.minValueCm === null ? "" : String(requirement.minValueCm),
          maxValueCm: requirement.maxValueCm === null ? "" : String(requirement.maxValueCm),
          helpText: requirement.helpText ?? "",
        },
      ]),
    ),
  };
}

function toCategoryForm(category: AdminCategory): CategoryFormState {
  return {
    name: category.name,
    slug: category.slug,
    parentCategoryId: category.parentCategoryId ?? "",
    displayOrder: String(category.displayOrder),
    isActive: category.isActive,
  };
}

function toColourForm(colour: AdminColour): ColourFormState {
  return {
    name: colour.name,
    hexCode: colour.hexCode,
    isActive: colour.isActive,
  };
}

function buildProductRequest(form: ProductFormState): UpsertAdminProductRequest {
  return {
    name: form.name.trim(),
    slug: optionalText(form.slug),
    description: form.description.trim(),
    price: requiredNumber(form.price, 0),
    clothingType: form.clothingType.trim(),
    estimatedProductionDays: Math.max(1, requiredInteger(form.estimatedProductionDays, 14)),
    isAvailable: form.isAvailable,
    isFeatured: form.isFeatured,
    allowsCustomColor: form.allowsCustomColor,
    fabricDescription: optionalText(form.fabricDescription),
    primaryImageUrl: optionalText(form.primaryImageUrl),
    primaryImageAltText: optionalText(form.primaryImageAltText),
    categoryIds: form.categoryIds,
    colourIds: form.colourIds,
    measurementRequirements: Object.entries(form.measurementRequirements).map(([measurementTypeId, requirement]) => ({
      measurementTypeId,
      isRequired: requirement.isRequired,
      minValueCm: optionalNumber(requirement.minValueCm),
      maxValueCm: optionalNumber(requirement.maxValueCm),
      helpText: optionalText(requirement.helpText),
    })),
  };
}

function buildCategoryRequest(form: CategoryFormState): UpsertAdminCategoryRequest {
  return {
    name: form.name.trim(),
    slug: optionalText(form.slug),
    parentCategoryId: form.parentCategoryId || null,
    displayOrder: requiredInteger(form.displayOrder, 0),
    isActive: form.isActive,
  };
}

function buildColourRequest(form: ColourFormState): UpsertAdminColourRequest {
  return {
    name: form.name.trim(),
    hexCode: form.hexCode.trim(),
    isActive: form.isActive,
  };
}

export function AdminCataloguePage() {
  const queryClient = useQueryClient();
  const accessToken = useAuthStore((state) => state.accessToken);
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");
  const [selectedProductId, setSelectedProductId] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState("");
  const [selectedColourId, setSelectedColourId] = useState("");
  const [productForm, setProductForm] = useState<ProductFormState>(() => emptyProductForm());
  const [categoryForm, setCategoryForm] = useState<CategoryFormState>(() => emptyCategoryForm());
  const [colourForm, setColourForm] = useState<ColourFormState>(() => emptyColourForm());

  const productsQuery = useQuery({
    queryKey: ["admin", "catalogue", "products"],
    queryFn: getAdminProducts,
    enabled: Boolean(accessToken && isAdmin),
  });

  const selectedProductQuery = useQuery({
    queryKey: ["admin", "catalogue", "products", selectedProductId],
    queryFn: () => getAdminProduct(selectedProductId),
    enabled: Boolean(accessToken && isAdmin && selectedProductId),
  });

  const categoriesQuery = useQuery({
    queryKey: ["admin", "catalogue", "categories"],
    queryFn: getAdminCategories,
    enabled: Boolean(accessToken && isAdmin),
  });

  const coloursQuery = useQuery({
    queryKey: ["admin", "catalogue", "colours"],
    queryFn: getAdminColours,
    enabled: Boolean(accessToken && isAdmin),
  });

  const measurementsQuery = useQuery({
    queryKey: ["catalogue", "measurements"],
    queryFn: getMeasurements,
    enabled: Boolean(accessToken && isAdmin),
  });

  const saveProductMutation = useMutation({
    mutationFn: () => {
      const request = buildProductRequest(productForm);
      return selectedProductId ? updateAdminProduct(selectedProductId, request) : createAdminProduct(request);
    },
    onSuccess: async (product) => {
      setSelectedProductId(product.id);
      setProductForm(toProductForm(product));
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue", "products"] });
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue", "products", product.id] });
    },
  });

  const deactivateProductMutation = useMutation({
    mutationFn: () => deactivateAdminProduct(selectedProductId),
    onSuccess: async (product) => {
      setProductForm(toProductForm(product));
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue", "products"] });
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue", "products", product.id] });
    },
  });

  const saveCategoryMutation = useMutation({
    mutationFn: () => {
      const request = buildCategoryRequest(categoryForm);
      return selectedCategoryId ? updateAdminCategory(selectedCategoryId, request) : createAdminCategory(request);
    },
    onSuccess: async (category) => {
      setSelectedCategoryId(category.id);
      setCategoryForm(toCategoryForm(category));
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue"] });
    },
  });

  const saveColourMutation = useMutation({
    mutationFn: () => {
      const request = buildColourRequest(colourForm);
      return selectedColourId ? updateAdminColour(selectedColourId, request) : createAdminColour(request);
    },
    onSuccess: async (colour) => {
      setSelectedColourId(colour.id);
      setColourForm(toColourForm(colour));
      await queryClient.invalidateQueries({ queryKey: ["admin", "catalogue"] });
    },
  });

  const selectedCategory = useMemo(
    () => categoriesQuery.data?.find((category) => category.id === selectedCategoryId),
    [categoriesQuery.data, selectedCategoryId],
  );

  const selectedColour = useMemo(
    () => coloursQuery.data?.find((colour) => colour.id === selectedColourId),
    [coloursQuery.data, selectedColourId],
  );

  useEffect(() => {
    if (!selectedProductId && productsQuery.data?.length) {
      setSelectedProductId(productsQuery.data[0].id);
    }
  }, [productsQuery.data, selectedProductId]);

  useEffect(() => {
    if (selectedProductQuery.data) {
      setProductForm(toProductForm(selectedProductQuery.data));
    }
  }, [selectedProductQuery.data]);

  useEffect(() => {
    if (selectedCategory) {
      setCategoryForm(toCategoryForm(selectedCategory));
    }
  }, [selectedCategory]);

  useEffect(() => {
    if (selectedColour) {
      setColourForm(toColourForm(selectedColour));
    }
  }, [selectedColour]);

  function handleProductSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveProductMutation.mutate();
  }

  function handleCategorySubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveCategoryMutation.mutate();
  }

  function handleColourSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveColourMutation.mutate();
  }

  function startNewProduct() {
    setSelectedProductId("");
    setProductForm(emptyProductForm());
  }

  function startNewCategory() {
    setSelectedCategoryId("");
    setCategoryForm(emptyCategoryForm());
  }

  function startNewColour() {
    setSelectedColourId("");
    setColourForm(emptyColourForm());
  }

  function toggleMeasurement(measurementTypeId: string) {
    setProductForm((current) => {
      const nextRequirements = { ...current.measurementRequirements };

      if (nextRequirements[measurementTypeId]) {
        delete nextRequirements[measurementTypeId];
      } else {
        nextRequirements[measurementTypeId] = {
          isRequired: true,
          minValueCm: "",
          maxValueCm: "",
          helpText: "",
        };
      }

      return { ...current, measurementRequirements: nextRequirements };
    });
  }

  function updateMeasurement(measurementTypeId: string, patch: Partial<MeasurementDraft>) {
    const emptyDraft: MeasurementDraft = {
      isRequired: true,
      minValueCm: "",
      maxValueCm: "",
      helpText: "",
    };

    setProductForm((current) => ({
      ...current,
      measurementRequirements: {
        ...current.measurementRequirements,
        [measurementTypeId]: { ...(current.measurementRequirements[measurementTypeId] ?? emptyDraft), ...patch },
      },
    }));
  }

  if (!accessToken || !isAdmin) {
    return (
      <section className="content-panel">
        <p className="eyebrow">Admin</p>
        <h1>Admin access required</h1>
        <p>Log in with an account assigned to the Admin role to manage catalogue records.</p>
      </section>
    );
  }

  const hasCatalogueError =
    productsQuery.isError || categoriesQuery.isError || coloursQuery.isError || measurementsQuery.isError;

  return (
    <div className="admin-page">
      <section className="admin-hero">
        <ShieldCheck aria-hidden="true" size={30} />
        <div>
          <p className="eyebrow">Admin catalogue</p>
          <h1>Catalogue management</h1>
          <p>Maintain products, categories, colour swatches, and measurement rules.</p>
        </div>
      </section>

      {hasCatalogueError ? (
        <p className="inline-alert">Catalogue data could not be loaded. Check admin permissions.</p>
      ) : null}

      <div className="admin-layout admin-catalogue-layout">
        <section className="admin-panel">
          <Boxes aria-hidden="true" size={24} />
          <div className="admin-panel-heading">
            <h2>Products</h2>
            <button className="button button-light" onClick={startNewProduct} type="button">
              <Plus aria-hidden="true" size={18} />
              New
            </button>
          </div>
          <div className="admin-product-list">
            {productsQuery.data?.length ? (
              productsQuery.data.map((product) => (
                <button
                  className={selectedProductId === product.id ? "admin-product-row active" : "admin-product-row"}
                  key={product.id}
                  onClick={() => setSelectedProductId(product.id)}
                  type="button"
                >
                  <span>
                    <strong>{product.name}</strong>
                    <small>{product.slug}</small>
                  </span>
                  <span>{formatCurrency(product.price)}</span>
                  <span>{product.isAvailable ? "Available" : "Hidden"}</span>
                </button>
              ))
            ) : (
              <p>No products saved yet.</p>
            )}
          </div>
        </section>

        <form className="admin-panel admin-catalogue-form" onSubmit={handleProductSubmit}>
          <Boxes aria-hidden="true" size={24} />
          <h2>{selectedProductId ? "Edit product" : "New product"}</h2>

          <div className="admin-form-grid">
            <label className="admin-field">
              <span>Name</span>
              <input
                required
                value={productForm.name}
                onChange={(event) => setProductForm((current) => ({ ...current, name: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Slug</span>
              <input
                value={productForm.slug}
                onChange={(event) => setProductForm((current) => ({ ...current, slug: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Price</span>
              <input
                min="0"
                step="0.01"
                type="number"
                value={productForm.price}
                onChange={(event) => setProductForm((current) => ({ ...current, price: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Clothing type</span>
              <input
                required
                value={productForm.clothingType}
                onChange={(event) => setProductForm((current) => ({ ...current, clothingType: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Production days</span>
              <input
                min="1"
                type="number"
                value={productForm.estimatedProductionDays}
                onChange={(event) =>
                  setProductForm((current) => ({ ...current, estimatedProductionDays: event.target.value }))
                }
              />
            </label>
            <label className="admin-field">
              <span>Image URL</span>
              <input
                value={productForm.primaryImageUrl}
                onChange={(event) =>
                  setProductForm((current) => ({ ...current, primaryImageUrl: event.target.value }))
                }
              />
            </label>
            <label className="admin-field admin-field-wide">
              <span>Image alt text</span>
              <input
                value={productForm.primaryImageAltText}
                onChange={(event) =>
                  setProductForm((current) => ({ ...current, primaryImageAltText: event.target.value }))
                }
              />
            </label>
            <label className="admin-field admin-field-wide">
              <span>Description</span>
              <textarea
                required
                rows={4}
                value={productForm.description}
                onChange={(event) => setProductForm((current) => ({ ...current, description: event.target.value }))}
              />
            </label>
            <label className="admin-field admin-field-wide">
              <span>Fabric</span>
              <textarea
                rows={3}
                value={productForm.fabricDescription}
                onChange={(event) =>
                  setProductForm((current) => ({ ...current, fabricDescription: event.target.value }))
                }
              />
            </label>
          </div>

          <div className="admin-toggle-grid">
            <label className="admin-check-row">
              <input
                checked={productForm.isAvailable}
                type="checkbox"
                onChange={(event) => setProductForm((current) => ({ ...current, isAvailable: event.target.checked }))}
              />
              <span>Available</span>
            </label>
            <label className="admin-check-row">
              <input
                checked={productForm.isFeatured}
                type="checkbox"
                onChange={(event) => setProductForm((current) => ({ ...current, isFeatured: event.target.checked }))}
              />
              <span>Featured</span>
            </label>
            <label className="admin-check-row">
              <input
                checked={productForm.allowsCustomColor}
                type="checkbox"
                onChange={(event) =>
                  setProductForm((current) => ({ ...current, allowsCustomColor: event.target.checked }))
                }
              />
              <span>Custom colour</span>
            </label>
          </div>

          <div className="admin-relation-grid">
            <section>
              <h3>Categories</h3>
              <div className="admin-checkbox-grid">
                {categoriesQuery.data?.map((category) => (
                  <label className="admin-check-row" key={category.id}>
                    <input
                      checked={productForm.categoryIds.includes(category.id)}
                      type="checkbox"
                      onChange={() =>
                        setProductForm((current) => ({
                          ...current,
                          categoryIds: toggleId(current.categoryIds, category.id),
                        }))
                      }
                    />
                    <span>{category.name}</span>
                  </label>
                ))}
              </div>
            </section>

            <section>
              <h3>Colours</h3>
              <div className="admin-checkbox-grid">
                {coloursQuery.data?.map((colour) => (
                  <label className="admin-check-row" key={colour.id}>
                    <input
                      checked={productForm.colourIds.includes(colour.id)}
                      type="checkbox"
                      onChange={() =>
                        setProductForm((current) => ({
                          ...current,
                          colourIds: toggleId(current.colourIds, colour.id),
                        }))
                      }
                    />
                    <span
                      className="admin-swatch"
                      style={{ "--admin-swatch-color": colour.hexCode } as CSSProperties}
                    />
                    <span>{colour.name}</span>
                  </label>
                ))}
              </div>
            </section>
          </div>

          <section className="admin-measurement-section">
            <Ruler aria-hidden="true" size={22} />
            <h3>Measurement rules</h3>
            <div className="admin-measurement-grid">
              {measurementsQuery.data?.map((measurement) => {
                const requirement = productForm.measurementRequirements[measurement.id];
                const isSelected = Boolean(requirement);

                return (
                  <article className={isSelected ? "admin-measurement-card active" : "admin-measurement-card"} key={measurement.id}>
                    <label className="admin-check-row">
                      <input checked={isSelected} type="checkbox" onChange={() => toggleMeasurement(measurement.id)} />
                      <span>{measurement.name}</span>
                    </label>
                    {isSelected ? (
                      <div className="admin-measurement-fields">
                        <label className="admin-check-row">
                          <input
                            checked={requirement.isRequired}
                            type="checkbox"
                            onChange={(event) =>
                              updateMeasurement(measurement.id, { isRequired: event.target.checked })
                            }
                          />
                          <span>Required</span>
                        </label>
                        <label className="admin-field">
                          <span>Min cm</span>
                          <input
                            type="number"
                            value={requirement.minValueCm}
                            onChange={(event) =>
                              updateMeasurement(measurement.id, { minValueCm: event.target.value })
                            }
                          />
                        </label>
                        <label className="admin-field">
                          <span>Max cm</span>
                          <input
                            type="number"
                            value={requirement.maxValueCm}
                            onChange={(event) =>
                              updateMeasurement(measurement.id, { maxValueCm: event.target.value })
                            }
                          />
                        </label>
                        <label className="admin-field admin-field-wide">
                          <span>Help text</span>
                          <input
                            value={requirement.helpText}
                            onChange={(event) => updateMeasurement(measurement.id, { helpText: event.target.value })}
                          />
                        </label>
                      </div>
                    ) : null}
                  </article>
                );
              })}
            </div>
          </section>

          <div className="admin-action-row">
            <button className="button button-primary" disabled={saveProductMutation.isPending} type="submit">
              <Save aria-hidden="true" size={18} />
              {saveProductMutation.isPending ? "Saving..." : "Save product"}
            </button>
            <button
              className="button button-light"
              disabled={!selectedProductId || deactivateProductMutation.isPending}
              onClick={() => deactivateProductMutation.mutate()}
              type="button"
            >
              <Trash2 aria-hidden="true" size={18} />
              Hide product
            </button>
          </div>
        </form>
      </div>

      <div className="admin-layout">
        <form className="admin-panel" onSubmit={handleCategorySubmit}>
          <Boxes aria-hidden="true" size={24} />
          <div className="admin-panel-heading">
            <h2>Categories</h2>
            <button className="button button-light" onClick={startNewCategory} type="button">
              <Plus aria-hidden="true" size={18} />
              New
            </button>
          </div>
          <div className="admin-mini-list">
            {categoriesQuery.data?.map((category) => (
              <button
                className={selectedCategoryId === category.id ? "admin-mini-row active" : "admin-mini-row"}
                key={category.id}
                onClick={() => setSelectedCategoryId(category.id)}
                type="button"
              >
                <strong>{category.name}</strong>
                <span>{category.isActive ? "Active" : "Hidden"}</span>
              </button>
            ))}
          </div>
          <div className="admin-form-grid">
            <label className="admin-field">
              <span>Name</span>
              <input
                required
                value={categoryForm.name}
                onChange={(event) => setCategoryForm((current) => ({ ...current, name: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Slug</span>
              <input
                value={categoryForm.slug}
                onChange={(event) => setCategoryForm((current) => ({ ...current, slug: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Parent</span>
              <select
                value={categoryForm.parentCategoryId}
                onChange={(event) =>
                  setCategoryForm((current) => ({ ...current, parentCategoryId: event.target.value }))
                }
              >
                <option value="">None</option>
                {categoriesQuery.data
                  ?.filter((category) => category.id !== selectedCategoryId)
                  .map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))}
              </select>
            </label>
            <label className="admin-field">
              <span>Display order</span>
              <input
                type="number"
                value={categoryForm.displayOrder}
                onChange={(event) => setCategoryForm((current) => ({ ...current, displayOrder: event.target.value }))}
              />
            </label>
          </div>
          <label className="admin-check-row">
            <input
              checked={categoryForm.isActive}
              type="checkbox"
              onChange={(event) => setCategoryForm((current) => ({ ...current, isActive: event.target.checked }))}
            />
            <span>Active</span>
          </label>
          <button className="button button-primary" disabled={saveCategoryMutation.isPending} type="submit">
            <Save aria-hidden="true" size={18} />
            {saveCategoryMutation.isPending ? "Saving..." : "Save category"}
          </button>
        </form>

        <form className="admin-panel" onSubmit={handleColourSubmit}>
          <Palette aria-hidden="true" size={24} />
          <div className="admin-panel-heading">
            <h2>Colours</h2>
            <button className="button button-light" onClick={startNewColour} type="button">
              <Plus aria-hidden="true" size={18} />
              New
            </button>
          </div>
          <div className="admin-mini-list">
            {coloursQuery.data?.map((colour) => (
              <button
                className={selectedColourId === colour.id ? "admin-mini-row active" : "admin-mini-row"}
                key={colour.id}
                onClick={() => setSelectedColourId(colour.id)}
                type="button"
              >
                <span className="admin-swatch" style={{ "--admin-swatch-color": colour.hexCode } as CSSProperties} />
                <strong>{colour.name}</strong>
                <span>{colour.isActive ? "Active" : "Hidden"}</span>
              </button>
            ))}
          </div>
          <div className="admin-form-grid">
            <label className="admin-field">
              <span>Name</span>
              <input
                required
                value={colourForm.name}
                onChange={(event) => setColourForm((current) => ({ ...current, name: event.target.value }))}
              />
            </label>
            <label className="admin-field">
              <span>Hex code</span>
              <input
                required
                type="color"
                value={colourForm.hexCode}
                onChange={(event) => setColourForm((current) => ({ ...current, hexCode: event.target.value }))}
              />
            </label>
          </div>
          <label className="admin-check-row">
            <input
              checked={colourForm.isActive}
              type="checkbox"
              onChange={(event) => setColourForm((current) => ({ ...current, isActive: event.target.checked }))}
            />
            <span>Active</span>
          </label>
          <button className="button button-primary" disabled={saveColourMutation.isPending} type="submit">
            <Save aria-hidden="true" size={18} />
            {saveColourMutation.isPending ? "Saving..." : "Save colour"}
          </button>
        </form>
      </div>
    </div>
  );
}
