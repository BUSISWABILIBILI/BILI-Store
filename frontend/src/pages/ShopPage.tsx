import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Search, SlidersHorizontal } from "lucide-react";
import { useSearchParams } from "react-router-dom";
import { FeaturedProductCard } from "../components/home/FeaturedProductCard";
import { SectionHeader } from "../components/ui/SectionHeader";
import { featuredProducts, storeCategories } from "../data/storefront";
import { getCategories, getProductBySlug, getProducts } from "../services/catalogueApi";

const customerCategoryNames = ["Women", "Men", "Girls", "Boys"];

export function ShopPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const selectedCategory = searchParams.get("category") ?? "";
  const selectedClothingType = searchParams.get("clothingType") ?? "";
  const selectedProductSlug = searchParams.get("product") ?? "";
  const submittedSearch = searchParams.get("search") ?? "";
  const [searchInput, setSearchInput] = useState(submittedSearch);

  useEffect(() => {
    setSearchInput(submittedSearch);
  }, [submittedSearch]);

  const categoriesQuery = useQuery({
    queryKey: ["categories"],
    queryFn: getCategories,
  });

  const productsQuery = useQuery({
    queryKey: ["products", { selectedCategory, selectedClothingType, submittedSearch }],
    queryFn: () =>
      getProducts({
        category: selectedCategory || undefined,
        clothingType: selectedClothingType || undefined,
        search: submittedSearch || undefined,
      }),
  });

  const productDetailsQuery = useQuery({
    queryKey: ["product", selectedProductSlug],
    queryFn: () => getProductBySlug(selectedProductSlug),
    enabled: Boolean(selectedProductSlug),
  });

  const categories = categoriesQuery.data ?? [];
  const customerCategories = categories.filter((category) =>
    customerCategoryNames.includes(category.name),
  );
  const clothingTypeCategories = categories.filter(
    (category) => !customerCategoryNames.includes(category.name),
  );
  const visibleProducts =
    productsQuery.data && productsQuery.data.length > 0 ? productsQuery.data : featuredProducts;

  function updateFilter(key: string, value: string) {
    const nextParams = new URLSearchParams(searchParams);

    if (value) {
      nextParams.set(key, value);
    } else {
      nextParams.delete(key);
    }

    if (key !== "product") {
      nextParams.delete("product");
    }

    setSearchParams(nextParams);
  }

  function clearFilters() {
    setSearchInput("");
    setSearchParams({});
  }

  return (
    <div className="stack-xl">
      <section className="shop-hero">
        <p className="eyebrow">Catalogue design foundation</p>
        <h1>Shop custom-sewn designs</h1>
        <p>
          This screen now calls the backend catalogue API for products, categories, colours, and
          product detail data while keeping the boutique shopping layout from Phase 3.
        </p>
      </section>

      <form
        className="filter-shell"
        aria-label="Catalogue controls"
        onSubmit={(event) => {
          event.preventDefault();
          updateFilter("search", searchInput.trim());
        }}
      >
        <div className="search-shell">
          <Search aria-hidden="true" size={19} />
          <input
            type="search"
            placeholder="Search dresses, suits, jackets..."
            value={searchInput}
            onChange={(event) => setSearchInput(event.target.value)}
          />
        </div>
        <button className="button button-light" type="button">
          <SlidersHorizontal aria-hidden="true" size={18} />
          API filters
        </button>
        <button className="button button-primary" type="submit">
          Search
        </button>
      </form>

      <section>
        <SectionHeader
          eyebrow="Customer categories"
          title="Built for Women, Men, Girls, and Boys."
          description="These buttons are loaded from the backend category endpoint and filter the product API."
        />
        <div className="pill-row">
          {(customerCategories.length > 0 ? customerCategories : storeCategories).map((category) => (
            <button
              className={
                selectedCategory === ("slug" in category ? category.slug : category.label)
                  ? "pill-button pill-button-active"
                  : "pill-button"
              }
              key={"slug" in category ? category.slug : category.label}
              type="button"
              onClick={() => updateFilter("category", "slug" in category ? category.slug : category.label)}
            >
              {"label" in category ? category.label : category.name}
            </button>
          ))}
          <button className="pill-button" type="button" onClick={clearFilters}>
            Clear
          </button>
        </div>
      </section>

      {clothingTypeCategories.length > 0 ? (
        <section>
          <SectionHeader
            eyebrow="Clothing types"
            title="Filter by garment type."
            description="This prepares the catalogue for the clothing-type filtering required in the brief."
          />
          <div className="pill-row">
            {clothingTypeCategories.map((category) => (
              <button
                className={
                  selectedClothingType === category.name ? "pill-button pill-button-active" : "pill-button"
                }
                key={category.slug}
                type="button"
                onClick={() => updateFilter("clothingType", category.name)}
              >
                {category.name}
              </button>
            ))}
          </div>
        </section>
      ) : null}

      {selectedProductSlug ? (
        <section className="product-detail-panel">
          {productDetailsQuery.isLoading ? <p>Loading product details...</p> : null}
          {productDetailsQuery.isError ? (
            <p className="inline-alert">Could not load this product from the API.</p>
          ) : null}
          {productDetailsQuery.data ? (
            <>
              <div>
                <p className="eyebrow">Product details from API</p>
                <h2>{productDetailsQuery.data.name}</h2>
                <p>{productDetailsQuery.data.description}</p>
              </div>
              <div className="detail-facts">
                <span>{productDetailsQuery.data.clothingType}</span>
                <span>{productDetailsQuery.data.estimatedProductionDays} sewing days</span>
                <span>{productDetailsQuery.data.allowsCustomColor ? "Custom colour allowed" : "Swatches only"}</span>
              </div>
              <div className="measurement-board">
                {productDetailsQuery.data.measurementRequirements.map((measurement) => (
                  <span key={measurement.id}>{measurement.name}</span>
                ))}
              </div>
            </>
          ) : null}
        </section>
      ) : null}

      <section>
        <SectionHeader
          eyebrow="Product catalogue"
          title="Each card shows the information customers need before details."
          description="Products now come from the backend API. If the API is offline, the page keeps local preview cards visible for design work."
        />
        {productsQuery.isLoading ? <p>Loading products from the API...</p> : null}
        {productsQuery.isError ? (
          <p className="inline-alert">The product API is not reachable, so local preview products are showing.</p>
        ) : null}
        <div className="product-grid">
          {visibleProducts.map((product) => (
            <FeaturedProductCard product={product} key={product.id} />
          ))}
        </div>
      </section>
    </div>
  );
}
