import type { CSSProperties } from "react";
import { ArrowRight } from "lucide-react";
import { Link } from "react-router-dom";
import type { StoreCategory } from "../../data/storefront";
import { heroImage } from "../../data/storefront";

type CategoryTileProps = {
  category: StoreCategory;
};

export function CategoryTile({ category }: CategoryTileProps) {
  const style = {
    "--category-image": `url(${heroImage})`,
    "--category-position": category.imagePosition,
  } as CSSProperties;

  return (
    <Link className={`category-tile category-tile-${category.accent}`} style={style} to={category.href}>
      <span className="category-image" aria-hidden="true" />
      <span className="category-copy">
        <span className="category-label">{category.label}</span>
        <span>{category.description}</span>
      </span>
      <ArrowRight aria-hidden="true" size={20} />
    </Link>
  );
}
