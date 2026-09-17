import { Link } from "react-router-dom";
import { ArrowRight, Palette, Ruler, ShoppingBag } from "lucide-react";

export function CartPage() {
  return (
    <section className="empty-state">
      <ShoppingBag aria-hidden="true" size={42} />
      <p className="eyebrow">Cart foundation</p>
      <h1>Your custom cart will live here.</h1>
      <p>
        Phase 6 will preserve each selected garment, colour, custom colour note, measurement set,
        quantity, unit price, delivery fee, and order total.
      </p>
      <div className="cart-preview-grid" aria-label="Cart data that will be preserved">
        <span>
          <Palette aria-hidden="true" size={18} />
          Selected colour
        </span>
        <span>
          <Ruler aria-hidden="true" size={18} />
          Required measurements
        </span>
        <span>
          <ShoppingBag aria-hidden="true" size={18} />
          Garment and quantity
        </span>
      </div>
      <Link className="button button-primary" to="/shop">
        Browse designs
        <ArrowRight aria-hidden="true" size={18} />
      </Link>
    </section>
  );
}
