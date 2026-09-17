import type { CSSProperties } from "react";
import { useQuery } from "@tanstack/react-query";
import { ArrowRight, CheckCircle2, MapPin, Ruler, ShieldCheck, Sparkles } from "lucide-react";
import { Link } from "react-router-dom";
import { CategoryTile } from "../components/home/CategoryTile";
import { FeaturedProductCard } from "../components/home/FeaturedProductCard";
import { OrderingStepCard } from "../components/home/OrderingStepCard";
import { ApiStatusPanel } from "../components/ui/ApiStatusPanel";
import { SectionHeader } from "../components/ui/SectionHeader";
import { featuredProducts, heroImage, orderingSteps, storeCategories } from "../data/storefront";
import { getProducts } from "../services/catalogueApi";

export function HomePage() {
  const featuredQuery = useQuery({
    queryKey: ["products", "featured-home"],
    queryFn: () => getProducts({ featured: true }),
  });
  const visibleFeaturedProducts =
    featuredQuery.data && featuredQuery.data.length > 0 ? featuredQuery.data : featuredProducts;

  const heroStyle = {
    "--hero-image": `url(${heroImage})`,
  } as CSSProperties;

  return (
    <div className="homepage stack-xl">
      <section className="storefront-hero" style={heroStyle}>
        <div className="storefront-hero-content">
          <p className="eyebrow">Custom-sewn traditional fashion</p>
          <h1>BILI Store</h1>
          <p>
            Premium traditional outfits made around the customer's chosen design, colour,
            measurements, and PAXI/PEP delivery details.
          </p>
          <div className="action-row">
            <Link className="button button-primary" to="/shop">
              Shop designs
              <ArrowRight aria-hidden="true" size={18} />
            </Link>
            <Link className="button button-secondary" to="/measurements">
              How to measure
              <Ruler aria-hidden="true" size={18} />
            </Link>
          </div>
        </div>
      </section>

      <section className="category-section">
        <SectionHeader
          eyebrow="Shop by customer"
          title="Start with the person, then tailor the garment."
          description="The brief requires customer categories that can grow later, so these tiles already point customers toward Women, Men, Girls, and Boys shopping journeys."
        />
        <div className="category-grid">
          {storeCategories.map((category) => (
            <CategoryTile category={category} key={category.label} />
          ))}
        </div>
      </section>

      <section className="split-section split-section-elevated">
        <div>
          <p className="eyebrow">Custom fit is the core feature</p>
          <h2>Every order keeps the outfit, colour, and measurements together.</h2>
        </div>
        <div className="feature-list">
          <span>
            <CheckCircle2 aria-hidden="true" size={20} />
            Predefined colour swatches plus custom colour requests.
          </span>
          <span>
            <CheckCircle2 aria-hidden="true" size={20} />
            Required measurements change by clothing type.
          </span>
          <span>
            <CheckCircle2 aria-hidden="true" size={20} />
            PAXI/PEP collection details stay attached through checkout.
          </span>
        </div>
      </section>

      <section>
        <SectionHeader
          eyebrow="Featured outfit previews"
          title="A catalogue foundation that already feels shoppable."
          description="These cards now ask the backend for featured products. Preview data only appears if the API is offline during local design work."
        />
        {featuredQuery.isError ? (
          <p className="inline-alert">
            Featured products are using local preview data because the API is not reachable.
          </p>
        ) : null}
        <div className="product-grid">
          {visibleFeaturedProducts.map((product) => (
            <FeaturedProductCard product={product} key={product.id} />
          ))}
        </div>
      </section>

      <section className="process-section">
        <SectionHeader
          eyebrow="How custom ordering works"
          title="The customer journey is designed before the checkout is built."
          description="This prepares the frontend for the later phases: catalogue, measurements, cart, checkout, payment, and orders."
        />
        <div className="step-grid">
          {orderingSteps.map((step) => (
            <OrderingStepCard step={step} key={step.number} />
          ))}
        </div>
      </section>

      <section className="split-section measurement-callout">
        <div>
          <p className="eyebrow">Measurement confidence</p>
          <h2>Customers need clear guidance before they buy custom clothing.</h2>
          <p>
            The measurement page now introduces a professional guide pattern with step-by-step
            cards. Later, these same measurements will drive validation on product details and
            checkout.
          </p>
          <Link className="text-icon-link" to="/measurements">
            Open measurement guide
            <ArrowRight aria-hidden="true" size={17} />
          </Link>
        </div>
        <div className="measurement-board" aria-hidden="true">
          <span>Bust / Chest</span>
          <span>Waist</span>
          <span>Hips</span>
          <span>Shoulder</span>
          <span>Dress length</span>
          <span>Trouser length</span>
        </div>
      </section>

      <section className="trust-grid" aria-label="BILI Store service highlights">
        <article>
          <Sparkles aria-hidden="true" size={26} />
          <h3>Premium boutique feel</h3>
          <p>Strong typography, warm editorial imagery, and focused shopping sections.</p>
        </article>
        <article>
          <ShieldCheck aria-hidden="true" size={26} />
          <h3>Policy-aware checkout</h3>
          <p>The return policy is visible now and will be required before final checkout later.</p>
        </article>
        <article>
          <MapPin aria-hidden="true" size={26} />
          <h3>PAXI/PEP ready</h3>
          <p>Delivery copy prepares customers for collection-point details during checkout.</p>
        </article>
      </section>

      <section className="newsletter-band">
        <div>
          <p className="eyebrow">New drops and custom slots</p>
          <h2>Stay close to the next collection.</h2>
        </div>
        <form className="newsletter-form" onSubmit={(event) => event.preventDefault()}>
          <label className="sr-only" htmlFor="newsletter-email">
            Email address
          </label>
          <input id="newsletter-email" type="email" placeholder="Email address" />
          <button className="button button-primary" type="submit">
            Join list
          </button>
        </form>
      </section>

      <ApiStatusPanel />
    </div>
  );
}
