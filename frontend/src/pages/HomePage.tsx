import { Link } from "react-router-dom";
import { ApiStatusPanel } from "../components/ui/ApiStatusPanel";

export function HomePage() {
  return (
    <div className="stack-xl">
      <section className="foundation-hero">
        <div className="hero-copy">
          <p className="eyebrow">Phase 2 foundation</p>
          <h1>BILI Store full-stack setup is underway.</h1>
          <p>
            The React frontend and ASP.NET Core backend are being prepared for a premium
            custom-sewn traditional clothing store where garments, colors, measurements,
            checkout, payments, and PAXI/PEP delivery stay connected.
          </p>
          <div className="action-row">
            <Link className="button button-primary" to="/shop">
              View shop shell
            </Link>
            <Link className="button button-secondary" to="/measurements">
              Measurement guide shell
            </Link>
          </div>
        </div>
      </section>

      <ApiStatusPanel />
    </div>
  );
}

