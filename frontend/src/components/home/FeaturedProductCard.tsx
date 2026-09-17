import type { CSSProperties } from "react";
import { Clock3, Ruler, ShoppingBag } from "lucide-react";
import { Link } from "react-router-dom";
import type { ProductPreview } from "../../data/storefront";
import { formatCurrency } from "../../utils/formatters";

type FeaturedProductCardProps = {
  product: ProductPreview;
};

export function FeaturedProductCard({ product }: FeaturedProductCardProps) {
  const style = {
    "--product-image": `url(${product.image})`,
    "--product-position": product.imagePosition,
  } as CSSProperties;

  return (
    <article className="product-card">
      <Link className="product-visual" style={style} to={`/shop?preview=${product.id}`} aria-label={product.name}>
        <span>{product.badge}</span>
      </Link>
      <div className="product-card-body">
        <div>
          <p className="product-meta">
            {product.category} / {product.clothingType}
          </p>
          <h3>{product.name}</h3>
          <p>{product.description}</p>
        </div>

        <div className="swatch-row" aria-label={`Available colours for ${product.name}`}>
          {product.colors.map((color) => (
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
            {product.productionTime}
          </span>
          <span>
            <Ruler aria-hidden="true" size={16} />
            {product.requiredMeasurements.slice(0, 3).join(", ")}
          </span>
        </div>

        <div className="product-card-footer">
          <strong>{formatCurrency(product.price)}</strong>
          <Link className="text-icon-link" to={`/shop?preview=${product.id}`}>
            <ShoppingBag aria-hidden="true" size={17} />
            View details
          </Link>
        </div>
      </div>
    </article>
  );
}
