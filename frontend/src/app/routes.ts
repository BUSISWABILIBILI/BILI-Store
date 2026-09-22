import { AccountPage } from "../pages/AccountPage";
import { AdminCataloguePage } from "../pages/AdminCataloguePage";
import { AdminCustomersPage } from "../pages/AdminCustomersPage";
import { AdminDashboardPage } from "../pages/AdminDashboardPage";
import { AdminOrdersPage } from "../pages/AdminOrdersPage";
import { CartPage } from "../pages/CartPage";
import { CheckoutPage } from "../pages/CheckoutPage";
import { HomePage } from "../pages/HomePage";
import { MeasurementGuidePage } from "../pages/MeasurementGuidePage";
import { OrderConfirmationPage } from "../pages/OrderConfirmationPage";
import { ReturnPolicyPage } from "../pages/ReturnPolicyPage";
import { ShopPage } from "../pages/ShopPage";

export const routes = [
  { path: "/", label: "Home", element: HomePage },
  { path: "/shop", label: "Shop", element: ShopPage },
  { path: "/measurements", label: "Measurements", element: MeasurementGuidePage },
  { path: "/cart", label: "Cart", element: CartPage },
  { path: "/checkout", label: "Checkout", element: CheckoutPage },
  { path: "/orders/:orderNumber", label: "Order Confirmation", element: OrderConfirmationPage },
  { path: "/admin", label: "Admin Dashboard", element: AdminDashboardPage },
  { path: "/admin/orders", label: "Admin Orders", element: AdminOrdersPage },
  { path: "/admin/catalogue", label: "Admin Catalogue", element: AdminCataloguePage },
  { path: "/admin/customers", label: "Admin Customers", element: AdminCustomersPage },
  { path: "/account", label: "Account", element: AccountPage },
  { path: "/return-policy", label: "Return Policy", element: ReturnPolicyPage },
] as const;
