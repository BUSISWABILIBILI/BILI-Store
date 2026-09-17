import type { CSSProperties } from "react";
import { Clock3, Ruler, ShoppingBag } from "lucide-react";
import { Link } from "react-router-dom";
import type { ProductPreview } from "../../data/storefront";
import type { ProductSummary } from "../../types/catalogue";
import { formatCurrency } from "../../utils/formatters";

type FeaturedProductCardProps = {
  product: ProductPreview | ProductSummary;
};

export function FeaturedProductCard({ product }: FeaturedProductCardProps) {
  const isApiProduct = "slug" in product;
  const imageUrl = isApiProduct ? product.primaryImageUrl : product.image;
  const imagePosition = isApiProduct ? "center" : product.imagePosition;
  const badge = getBadge(product);
  const category = getCustomerCategory(product);
  const colours = getColours(product);
  const productionTime = isApiProduct
    ? `${product.estimatedProductionDays} sewing days`
    : product.productionTime;
  const detailTarget = isApiProduct ? `/shop?product=${product.slug}` : `/shop?preview=${product.id}`;

  const style = {
    "--product-image": `url(${imageUrl})`,
    "--product-position": imagePosition,
  } as CSSProperties;

  return (
    <article className="product-card">
      <Link className="product-visual" style={style} to={detailTarget} aria-label={product.name}>
        <span>{badge}</span>
      </Link>
      <div className="product-card-body">
        <div>
          <p className="product-meta">
            {category} / {product.clothingType}
          </p>
          <h3>{product.name}</h3>
          <p>{product.description}</p>
        </div>

        <div className="swatch-row" aria-label={`Available colours for ${product.name}`}>
          {colours.map((color) => (
            <span
              className="color-swatch"
              key={color.name}
              style={{ backgroundColor: color.hex }}
              title={color.name}
            />
          ))}
          <span className="custom-colour-note">Custom colour ready</span>
        </div>

        <div className="product-facts">
          <span>
            <Clock3 aria-hidden="true" size={16} />
            {productionTime}
          </span>
          <span>
            <Ruler aria-hidden="true" size={16} />
            {product.requiredMeasurements.slice(0, 3).join(", ")}
          </span>
        </div>

        <div className="product-card-footer">
          <strong>{formatCurrency(product.price)}</strong>
          <Link className="text-icon-link" to={detailTarget}>
            <ShoppingBag aria-hidden="true" size={17} />
            View details
          </Link>
        </div>
      </div>
    </article>
  );
}

function getBadge(product: ProductPreview | ProductSummary) {
  if (!("slug" in product)) {
    return product.badge;
  }

  if (!product.isAvailable) {
    return "Unavailable";
  }

  return product.isFeatured ? "Featured" : "Custom fit";
}

function getCustomerCategory(product: ProductPreview | ProductSummary) {
  if (!("slug" in product)) {
    return product.category;
  }

  return (
    product.categories.find((category) =>
      ["Women", "Men", "Girls", "Boys"].includes(category.name),
    )?.name ?? "Custom"
  );
}

function getColours(product: ProductPreview | ProductSummary) {
  if (!("slug" in product)) {
    return product.colors;
  }

  return product.availableColours.map((colour) => ({
    name: colour.name,
    hex: colour.hexCode,
  }));
}
