import { Search, SlidersHorizontal } from "lucide-react";
import { FeaturedProductCard } from "../components/home/FeaturedProductCard";
import { SectionHeader } from "../components/ui/SectionHeader";
import { featuredProducts, storeCategories } from "../data/storefront";

export function ShopPage() {
  return (
    <div className="stack-xl">
      <section className="shop-hero">
        <p className="eyebrow">Catalogue design foundation</p>
        <h1>Shop custom-sewn designs</h1>
        <p>
          This screen is the visual structure for product browsing. Phase 4 will connect search,
          filters, product details, images, colours, and availability to the backend.
        </p>
      </section>

      <section className="filter-shell" aria-label="Catalogue preview controls">
        <div className="search-shell">
          <Search aria-hidden="true" size={19} />
          <input type="search" placeholder="Search dresses, suits, jackets..." />
        </div>
        <button className="button button-light" type="button">
          <SlidersHorizontal aria-hidden="true" size={18} />
          Filters
        </button>
      </section>

      <section>
        <SectionHeader
          eyebrow="Customer categories"
          title="Built for Women, Men, Girls, and Boys."
          description="The backend already supports category seed data; this page is ready for real category filtering in the next phase."
        />
        <div className="pill-row">
          {storeCategories.map((category) => (
            <a href={category.href} key={category.label}>
              {category.label}
            </a>
          ))}
        </div>
      </section>

      <section>
        <SectionHeader
          eyebrow="Product card system"
          title="Each card shows the information customers need before details."
          description="Price, category, clothing type, available colours, sewing time, and required measurements are visible from the grid."
        />
        <div className="product-grid">
          {featuredProducts.map((product) => (
            <FeaturedProductCard product={product} key={product.id} />
          ))}
        </div>
      </section>
    </div>
  );
}
