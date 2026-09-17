import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { ArrowRight, Minus, Palette, Plus, Ruler, ShoppingBag, Trash2 } from "lucide-react";
import { quoteCart } from "../services/cartApi";
import { useCartStore } from "../stores/cartStore";
import { buildCartQuoteRequest } from "../utils/cartQuote";
import { formatCurrency } from "../utils/formatters";

const fallbackDeliveryFee = 99;

export function CartPage() {
  const items = useCartStore((state) => state.items);
  const clearCart = useCartStore((state) => state.clearCart);
  const removeItem = useCartStore((state) => state.removeItem);
  const updateQuantity = useCartStore((state) => state.updateQuantity);

  const quoteRequest = useMemo(() => buildCartQuoteRequest(items), [items]);

  const quoteQuery = useQuery({
    queryKey: ["cart-quote", quoteRequest],
    queryFn: () => quoteCart(quoteRequest),
    enabled: items.length > 0,
  });

  const localSubtotal = items.reduce((total, item) => total + item.unitPrice * item.quantity, 0);
  const quote = quoteQuery.data;
  const subtotal = quote?.subtotal ?? localSubtotal;
  const deliveryFee = quote?.deliveryFee ?? (items.length > 0 ? fallbackDeliveryFee : 0);
  const grandTotal = quote?.grandTotal ?? subtotal + deliveryFee;

  if (items.length === 0) {
    return (
      <section className="empty-state">
        <ShoppingBag aria-hidden="true" size={42} />
        <p className="eyebrow">Cart</p>
        <h1>Your custom cart is empty.</h1>
        <p>
          Add a product after choosing a colour and validating the measurements required for that
          garment.
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

  return (
    <div className="cart-page">
      <section className="cart-heading">
        <div>
          <p className="eyebrow">Cart</p>
          <h1>Your custom garments</h1>
          <p>
            Each cart item keeps the product, selected colour, measurements, quantity, unit price,
            and production estimate together.
          </p>
        </div>
        <button className="button button-light" onClick={clearCart} type="button">
          <Trash2 aria-hidden="true" size={18} />
          Clear cart
        </button>
      </section>

      {quoteQuery.isError ? (
        <p className="inline-alert">
          The backend cart quote is not reachable, so local totals are showing for now.
        </p>
      ) : null}

      {quote?.issues.length ? (
        <div className="validation-panel" role="alert">
          <strong>Cart needs attention:</strong>
          <ul>
            {quote.issues.map((issue) => (
              <li key={`${issue.productSlug}-${issue.message}`}>{issue.message}</li>
            ))}
          </ul>
        </div>
      ) : null}

      <div className="cart-layout">
        <div className="cart-item-list">
          {items.map((item) => (
            <article className="cart-item-card" key={item.id}>
              <div
                aria-label={item.imageAltText}
                className="cart-item-image"
                style={{ backgroundImage: `url(${item.imageUrl})` }}
              />
              <div className="cart-item-body">
                <div className="cart-item-title-row">
                  <div>
                    <p className="product-meta">{item.clothingType}</p>
                    <h2>{item.productName}</h2>
                    <p>{item.productDescription}</p>
                  </div>
                  <button
                    aria-label={`Remove ${item.productName}`}
                    className="icon-button"
                    onClick={() => removeItem(item.id)}
                    type="button"
                  >
                    <Trash2 aria-hidden="true" size={18} />
                  </button>
                </div>

                <div className="cart-detail-grid">
                  <div>
                    <span>Colour</span>
                    <strong>{item.selectedColour?.name ?? item.customColourName}</strong>
                    <i
                      aria-hidden="true"
                      style={{
                        backgroundColor:
                          item.selectedColour?.hexCode ?? item.customColourHex ?? "transparent",
                      }}
                    />
                  </div>
                  <div>
                    <span>Production</span>
                    <strong>{item.productionDaysEstimate} sewing days</strong>
                  </div>
                  <div>
                    <span>Unit price</span>
                    <strong>{formatCurrency(item.unitPrice)}</strong>
                  </div>
                </div>

                <div className="cart-measurements">
                  {item.measurements.map((measurement) => (
                    <span key={measurement.measurementKey}>
                      {measurement.measurementName}: {measurement.valueCm} {measurement.unit}
                    </span>
                  ))}
                </div>

                <div className="quantity-control">
                  <button
                    aria-label={`Decrease quantity for ${item.productName}`}
                    className="icon-button"
                    onClick={() => updateQuantity(item.id, item.quantity - 1)}
                    type="button"
                  >
                    <Minus aria-hidden="true" size={17} />
                  </button>
                  <strong>{item.quantity}</strong>
                  <button
                    aria-label={`Increase quantity for ${item.productName}`}
                    className="icon-button"
                    onClick={() => updateQuantity(item.id, item.quantity + 1)}
                    type="button"
                  >
                    <Plus aria-hidden="true" size={17} />
                  </button>
                </div>
              </div>
            </article>
          ))}
        </div>

        <aside className="cart-summary" aria-label="Cart totals">
          <p className="eyebrow">Order estimate</p>
          <h2>Cart totals</h2>
          <dl>
            <div>
              <dt>Subtotal</dt>
              <dd>{formatCurrency(subtotal)}</dd>
            </div>
            <div>
              <dt>PAXI/PEP delivery estimate</dt>
              <dd>{formatCurrency(deliveryFee)}</dd>
            </div>
            <div>
              <dt>Grand total</dt>
              <dd>{formatCurrency(grandTotal)}</dd>
            </div>
          </dl>
          <Link className="button button-primary" to="/checkout">
            Continue to checkout
            <ArrowRight aria-hidden="true" size={18} />
          </Link>
          <p>
            Checkout collects customer details, PAXI/PEP collection point information, and
            return-policy acceptance before payment.
          </p>
        </aside>
      </div>
    </div>
  );
}
