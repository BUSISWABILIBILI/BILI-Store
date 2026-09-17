import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { ArrowRight, CheckCircle2, MapPin, PackageCheck, ReceiptText, UserRound } from "lucide-react";
import { getOrder } from "../services/ordersApi";
import { formatCurrency } from "../utils/formatters";

export function OrderConfirmationPage() {
  const { orderNumber = "" } = useParams();

  const orderQuery = useQuery({
    queryKey: ["order", orderNumber],
    queryFn: () => getOrder(orderNumber),
    enabled: Boolean(orderNumber),
  });

  if (orderQuery.isLoading) {
    return (
      <section className="content-panel">
        <p className="eyebrow">Order confirmation</p>
        <h1>Loading order</h1>
      </section>
    );
  }

  if (orderQuery.isError || !orderQuery.data) {
    return (
      <section className="empty-state">
        <ReceiptText aria-hidden="true" size={42} />
        <p className="eyebrow">Order confirmation</p>
        <h1>Order not found.</h1>
        <p>Check the order number or return to the shop to start another custom order.</p>
        <Link className="button button-primary" to="/shop">
          Browse designs
          <ArrowRight aria-hidden="true" size={18} />
        </Link>
      </section>
    );
  }

  const order = orderQuery.data;
  const payment = order.payments[0];

  return (
    <div className="order-confirmation-page">
      <section className="order-confirmation-hero">
        <CheckCircle2 aria-hidden="true" size={36} />
        <div>
          <p className="eyebrow">Order confirmation</p>
          <h1>{order.orderNumber}</h1>
          <p>
            Your custom clothing order has been saved with delivery, measurement, and payment
            reference details attached.
          </p>
        </div>
      </section>

      <div className="order-detail-grid">
        <section className="order-info-card">
          <ReceiptText aria-hidden="true" size={24} />
          <h2>Totals</h2>
          <dl>
            <div>
              <dt>Subtotal</dt>
              <dd>{formatCurrency(order.subtotal)}</dd>
            </div>
            <div>
              <dt>PAXI/PEP delivery</dt>
              <dd>{formatCurrency(order.deliveryFee)}</dd>
            </div>
            <div>
              <dt>Total</dt>
              <dd>{formatCurrency(order.total)}</dd>
            </div>
            <div>
              <dt>Order status</dt>
              <dd>{order.orderStatus}</dd>
            </div>
            <div>
              <dt>Payment status</dt>
              <dd>{order.paymentStatus}</dd>
            </div>
          </dl>
        </section>

        <section className="order-info-card">
          <UserRound aria-hidden="true" size={24} />
          <h2>Customer</h2>
          <p>
            {order.customer.firstName} {order.customer.lastName}
          </p>
          <p>{order.customer.email}</p>
          <p>{order.customer.phone}</p>
        </section>

        <section className="order-info-card">
          <MapPin aria-hidden="true" size={24} />
          <h2>Delivery</h2>
          <p>{order.delivery.collectionPointName}</p>
          <p>
            {order.delivery.townCity}, {order.delivery.province}
          </p>
          <p>PAXI/PEP code: {order.delivery.paxiStoreCode}</p>
          <p>Status: {order.delivery.deliveryStatus}</p>
        </section>

        <section className="order-info-card">
          <PackageCheck aria-hidden="true" size={24} />
          <h2>Payment reference</h2>
          {payment ? (
            <>
              <p>{payment.provider}</p>
              <p>{payment.providerReference}</p>
              <p>{formatCurrency(payment.amount)}</p>
            </>
          ) : (
            <p>No payment reference was attached.</p>
          )}
        </section>
      </div>

      <section className="order-line-list">
        <h2>Garments</h2>
        {order.items.map((item) => (
          <article className="order-item-card" key={item.id}>
            <div>
              <h3>{item.productName}</h3>
              <strong>{formatCurrency(item.subtotal)}</strong>
            </div>
            <p>{item.productDescription}</p>
            <div className="cart-detail-grid">
              <div>
                <span>Colour</span>
                <strong>{item.selectedColourName ?? item.customColourName}</strong>
                <i
                  aria-hidden="true"
                  style={{
                    backgroundColor:
                      item.selectedColourHex ?? item.customColourHex ?? "transparent",
                  }}
                />
              </div>
              <div>
                <span>Quantity</span>
                <strong>{item.quantity}</strong>
              </div>
              <div>
                <span>Production</span>
                <strong>{item.productionDaysEstimate} sewing days</strong>
              </div>
            </div>
            <div className="cart-measurements">
              {item.measurements.map((measurement) => (
                <span key={measurement.measurementKey}>
                  {measurement.measurementName}: {measurement.valueCm} cm
                </span>
              ))}
            </div>
          </article>
        ))}
      </section>

      <Link className="button button-primary" to="/account">
        View account orders
        <ArrowRight aria-hidden="true" size={18} />
      </Link>
    </div>
  );
}
