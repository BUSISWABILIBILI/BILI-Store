import { useEffect, useMemo, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import {
  AlertCircle,
  ArrowLeft,
  ArrowRight,
  CheckCircle2,
  ClipboardCheck,
  MapPin,
  ShieldCheck,
  ShoppingBag,
  UserRound,
} from "lucide-react";
import { reviewCheckout } from "../services/checkoutApi";
import { useAuthStore } from "../stores/authStore";
import { useCartStore } from "../stores/cartStore";
import type { CheckoutCustomerDetails, CheckoutDeliveryDetails } from "../types/checkout";
import { buildCartQuoteRequest } from "../utils/cartQuote";
import { formatCurrency } from "../utils/formatters";

const fallbackDeliveryFee = 99;

const checkoutSteps = [
  { title: "Customer", icon: UserRound },
  { title: "Delivery", icon: MapPin },
  { title: "Policy", icon: ShieldCheck },
  { title: "Review", icon: ClipboardCheck },
];

const emptyCustomerForm: CheckoutCustomerDetails = {
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
};

const emptyDeliveryForm: CheckoutDeliveryDetails = {
  recipientPhone: "",
  paxiStoreCode: "",
  collectionPointName: "",
  townCity: "",
  province: "",
  instructions: "",
};

export function CheckoutPage() {
  const items = useCartStore((state) => state.items);
  const customer = useAuthStore((state) => state.customer);
  const [activeStep, setActiveStep] = useState(0);
  const [customerForm, setCustomerForm] = useState<CheckoutCustomerDetails>(emptyCustomerForm);
  const [deliveryForm, setDeliveryForm] = useState<CheckoutDeliveryDetails>(emptyDeliveryForm);
  const [returnPolicyAccepted, setReturnPolicyAccepted] = useState(false);

  const quoteRequest = useMemo(() => buildCartQuoteRequest(items), [items]);
  const localSubtotal = items.reduce((total, item) => total + item.unitPrice * item.quantity, 0);

  const reviewMutation = useMutation({
    mutationFn: reviewCheckout,
  });

  useEffect(() => {
    if (!customer) {
      return;
    }

    setCustomerForm((current) => ({
      firstName: current.firstName || customer.firstName,
      lastName: current.lastName || customer.lastName,
      email: current.email || customer.email,
      phone: current.phone || customer.phone,
    }));

    setDeliveryForm((current) => ({
      ...current,
      recipientPhone: current.recipientPhone || customer.phone,
    }));
  }, [customer]);

  const review = reviewMutation.data;
  const quote = review?.quote;
  const subtotal = quote?.subtotal ?? localSubtotal;
  const deliveryFee = quote?.deliveryFee ?? (items.length > 0 ? fallbackDeliveryFee : 0);
  const grandTotal = quote?.grandTotal ?? subtotal + deliveryFee;

  function updateCustomerField(field: keyof CheckoutCustomerDetails, value: string) {
    reviewMutation.reset();
    setCustomerForm((current) => ({ ...current, [field]: value }));
  }

  function updateDeliveryField(field: keyof CheckoutDeliveryDetails, value: string) {
    reviewMutation.reset();
    setDeliveryForm((current) => ({ ...current, [field]: value }));
  }

  function submitReview() {
    reviewMutation.mutate({
      customer: customerForm,
      delivery: deliveryForm,
      returnPolicyAccepted,
      cart: quoteRequest,
    });
  }

  if (items.length === 0) {
    return (
      <section className="empty-state">
        <ShoppingBag aria-hidden="true" size={42} />
        <p className="eyebrow">Checkout</p>
        <h1>Your cart needs a custom garment first.</h1>
        <p>
          Checkout can only review an order after a product has a colour, measurements, and quantity
          attached.
        </p>
        <Link className="button button-primary" to="/shop">
          Browse designs
          <ArrowRight aria-hidden="true" size={18} />
        </Link>
      </section>
    );
  }

  return (
    <div className="checkout-page">
      <section className="checkout-hero">
        <div>
          <p className="eyebrow">Checkout</p>
          <h1>Review your custom order</h1>
          <p>
            Confirm customer details, PAXI/PEP collection information, and the return policy before
            payment is added in the next phase.
          </p>
        </div>
      </section>

      <div className="checkout-steps" aria-label="Checkout steps">
        {checkoutSteps.map((step, index) => {
          const Icon = step.icon;

          return (
            <button
              className={`checkout-step-button ${activeStep === index ? "active" : ""}`}
              key={step.title}
              onClick={() => setActiveStep(index)}
              type="button"
            >
              <span>{index + 1}</span>
              <Icon aria-hidden="true" size={18} />
              <strong>{step.title}</strong>
            </button>
          );
        })}
      </div>

      {review?.issues.length ? (
        <div className="validation-panel" role="alert">
          <strong>Checkout needs attention:</strong>
          <ul>
            {review.issues.map((issue) => (
              <li key={`${issue.field}-${issue.message}`}>{issue.message}</li>
            ))}
          </ul>
        </div>
      ) : null}

      {review?.canContinue ? (
        <div className="checkout-success" role="status">
          <CheckCircle2 aria-hidden="true" size={22} />
          <div>
            <strong>Checkout review passed.</strong>
            <p>These details are ready for order creation and payment in the next phase.</p>
          </div>
        </div>
      ) : null}

      {reviewMutation.isError ? (
        <p className="inline-alert">Checkout review could not reach the backend right now.</p>
      ) : null}

      <div className="checkout-layout">
        <section className="checkout-panel">
          {activeStep === 0 ? (
            <>
              <UserRound aria-hidden="true" size={26} />
              <h2>Customer details</h2>
              <div className="checkout-form-grid">
                <CheckoutField
                  label="First name"
                  onChange={(value) => updateCustomerField("firstName", value)}
                  value={customerForm.firstName}
                />
                <CheckoutField
                  label="Last name"
                  onChange={(value) => updateCustomerField("lastName", value)}
                  value={customerForm.lastName}
                />
                <CheckoutField
                  label="Email"
                  onChange={(value) => updateCustomerField("email", value)}
                  type="email"
                  value={customerForm.email}
                />
                <CheckoutField
                  label="Phone"
                  onChange={(value) => updateCustomerField("phone", value)}
                  value={customerForm.phone}
                />
              </div>
            </>
          ) : null}

          {activeStep === 1 ? (
            <>
              <MapPin aria-hidden="true" size={26} />
              <h2>PAXI/PEP delivery</h2>
              <div className="checkout-form-grid">
                <CheckoutField
                  label="Recipient phone"
                  onChange={(value) => updateDeliveryField("recipientPhone", value)}
                  value={deliveryForm.recipientPhone}
                />
                <CheckoutField
                  label="PAXI/PEP store code"
                  onChange={(value) => updateDeliveryField("paxiStoreCode", value)}
                  value={deliveryForm.paxiStoreCode}
                />
                <CheckoutField
                  label="Collection point name"
                  onChange={(value) => updateDeliveryField("collectionPointName", value)}
                  value={deliveryForm.collectionPointName}
                />
                <CheckoutField
                  label="Town / city"
                  onChange={(value) => updateDeliveryField("townCity", value)}
                  value={deliveryForm.townCity}
                />
                <CheckoutField
                  label="Province"
                  onChange={(value) => updateDeliveryField("province", value)}
                  value={deliveryForm.province}
                />
                <CheckoutField
                  label="Delivery notes"
                  onChange={(value) => updateDeliveryField("instructions", value)}
                  value={deliveryForm.instructions}
                />
              </div>
            </>
          ) : null}

          {activeStep === 2 ? (
            <>
              <ShieldCheck aria-hidden="true" size={26} />
              <h2>Return policy</h2>
              <p>
                Custom-sewn clothing depends on accurate measurements and confirmed order details.
                Please read and accept the policy before moving to payment.
              </p>
              <label className="policy-check">
                <input
                  checked={returnPolicyAccepted}
                  onChange={(event) => {
                    reviewMutation.reset();
                    setReturnPolicyAccepted(event.target.checked);
                  }}
                  type="checkbox"
                />
                <span>I accept the BILI Store return and refund policy for custom garments.</span>
              </label>
              <Link className="button button-light" to="/return-policy">
                Read return policy
                <ArrowRight aria-hidden="true" size={18} />
              </Link>
            </>
          ) : null}

          {activeStep === 3 ? (
            <>
              <ClipboardCheck aria-hidden="true" size={26} />
              <h2>Order review</h2>
              <div className="checkout-review-list">
                {items.map((item) => (
                  <article className="checkout-review-item" key={item.id}>
                    <div>
                      <strong>{item.productName}</strong>
                      <span>{item.quantity} item(s)</span>
                    </div>
                    <p>
                      {item.selectedColour?.name ?? item.customColourName} /{" "}
                      {formatCurrency(item.unitPrice)} each
                    </p>
                  </article>
                ))}
              </div>
              <button
                className="button button-primary"
                disabled={reviewMutation.isPending}
                onClick={submitReview}
                type="button"
              >
                <ClipboardCheck aria-hidden="true" size={18} />
                {reviewMutation.isPending ? "Reviewing..." : "Review checkout"}
              </button>
            </>
          ) : null}

          <div className="checkout-actions">
            <button
              className="button button-light"
              disabled={activeStep === 0}
              onClick={() => setActiveStep((current) => Math.max(0, current - 1))}
              type="button"
            >
              <ArrowLeft aria-hidden="true" size={18} />
              Back
            </button>
            <button
              className="button button-primary"
              disabled={activeStep === checkoutSteps.length - 1}
              onClick={() =>
                setActiveStep((current) => Math.min(checkoutSteps.length - 1, current + 1))
              }
              type="button"
            >
              Next
              <ArrowRight aria-hidden="true" size={18} />
            </button>
          </div>
        </section>

        <aside className="checkout-summary-panel" aria-label="Checkout summary">
          <p className="eyebrow">Review total</p>
          <h2>{formatCurrency(grandTotal)}</h2>
          <dl>
            <div>
              <dt>Subtotal</dt>
              <dd>{formatCurrency(subtotal)}</dd>
            </div>
            <div>
              <dt>PAXI/PEP delivery</dt>
              <dd>{formatCurrency(deliveryFee)}</dd>
            </div>
            <div>
              <dt>Items</dt>
              <dd>{items.length}</dd>
            </div>
          </dl>
          <div className="checkout-status-list">
            <span>
              <UserRound aria-hidden="true" size={17} />
              {customerForm.email || "Customer email needed"}
            </span>
            <span>
              <MapPin aria-hidden="true" size={17} />
              {deliveryForm.collectionPointName || "Collection point needed"}
            </span>
            <span>
              {returnPolicyAccepted ? (
                <CheckCircle2 aria-hidden="true" size={17} />
              ) : (
                <AlertCircle aria-hidden="true" size={17} />
              )}
              Return policy {returnPolicyAccepted ? "accepted" : "not accepted"}
            </span>
          </div>
        </aside>
      </div>
    </div>
  );
}

type CheckoutFieldProps = {
  label: string;
  onChange: (value: string) => void;
  type?: string;
  value: string;
};

function CheckoutField({ label, onChange, type = "text", value }: CheckoutFieldProps) {
  return (
    <label className="checkout-field">
      <span>{label}</span>
      <input onChange={(event) => onChange(event.target.value)} type={type} value={value} />
    </label>
  );
}
