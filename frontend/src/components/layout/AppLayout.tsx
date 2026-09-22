import { Menu, Scissors, ShoppingBag, UserRound, X } from "lucide-react";
import { useState } from "react";
import { Link, NavLink, Outlet } from "react-router-dom";
import { routes } from "../../app/routes";
import { useAuthStore } from "../../stores/authStore";
import { useCartStore } from "../../stores/cartStore";

export function AppLayout() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const cartItemCount = useCartStore((state) =>
    state.items.reduce((total, item) => total + item.quantity, 0),
  );
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");
  const primaryNavigation = routes.filter((route) =>
    ["/shop", "/measurements", "/return-policy"].includes(route.path),
  );

  return (
    <div className="app-shell">
      <header className="site-header">
        <div className="announcement-bar">
          Custom sewn clothing made to your measurements. PAXI/PEP delivery ready.
        </div>
        <nav className="nav-shell" aria-label="Main navigation">
          <NavLink to="/" className="brand-mark" onClick={() => setIsMenuOpen(false)}>
            <Scissors aria-hidden="true" size={24} />
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
            {primaryNavigation.map((route) => (
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

          <div className="nav-actions">
            {isAdmin ? (
              <NavLink className="nav-link admin-nav-link" to="/admin/orders">
                Admin
              </NavLink>
            ) : null}
            <NavLink className="icon-button" to="/account" aria-label="Account">
              <UserRound aria-hidden="true" size={20} />
            </NavLink>
            <NavLink className="icon-button cart-action" to="/cart" aria-label="Cart">
              <ShoppingBag aria-hidden="true" size={20} />
              <span aria-hidden="true">{cartItemCount}</span>
            </NavLink>
          </div>
        </nav>
      </header>

      <main className="page-shell">
        <Outlet />
      </main>

      <footer className="site-footer">
        <div>
          <Link to="/" className="footer-brand">
            BILI Store
          </Link>
          <p>Traditional clothing. Custom fit. Premium boutique ordering.</p>
        </div>
        <div className="footer-links">
          <Link to="/shop">Shop</Link>
          <Link to="/measurements">Measurements</Link>
          <Link to="/return-policy">Return Policy</Link>
          <Link to="/checkout">Checkout</Link>
          {isAdmin ? <Link to="/admin/orders">Admin</Link> : null}
        </div>
      </footer>
    </div>
  );
}
