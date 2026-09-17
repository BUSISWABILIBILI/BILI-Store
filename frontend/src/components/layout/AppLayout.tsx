import { Menu, ShoppingBag, X } from "lucide-react";
import { useState } from "react";
import { NavLink, Outlet } from "react-router-dom";
import { routes } from "../../app/routes";

export function AppLayout() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <div className="app-shell">
      <header className="site-header">
        <div className="announcement-bar">Custom sewn clothing made to your measurements.</div>
        <nav className="nav-shell" aria-label="Main navigation">
          <NavLink to="/" className="brand-mark" onClick={() => setIsMenuOpen(false)}>
            <ShoppingBag aria-hidden="true" size={24} />
            <span>BILI Store</span>
          </NavLink>

          <button
            className="icon-button menu-toggle"
            type="button"
            aria-label="Toggle navigation"
            onClick={() => setIsMenuOpen((current) => !current)}
          >
            {isMenuOpen ? <X size={22} /> : <Menu size={22} />}
          </button>

          <div className={isMenuOpen ? "nav-links nav-links-open" : "nav-links"}>
            {routes
              .filter((route) => route.path !== "/")
              .map((route) => (
                <NavLink
                  key={route.path}
                  to={route.path}
                  className={({ isActive }) => (isActive ? "nav-link nav-link-active" : "nav-link")}
                  onClick={() => setIsMenuOpen(false)}
                >
                  {route.label}
                </NavLink>
              ))}
          </div>
        </nav>
      </header>

      <main className="page-shell">
        <Outlet />
      </main>

      <footer className="site-footer">
        <span>BILI Store</span>
        <span>Traditional clothing. Custom fit. PAXI/PEP delivery ready.</span>
      </footer>
    </div>
  );
}

