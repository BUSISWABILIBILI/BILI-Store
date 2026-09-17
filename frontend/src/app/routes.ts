import { AccountPage } from "../pages/AccountPage";
import { CartPage } from "../pages/CartPage";
import { CheckoutPage } from "../pages/CheckoutPage";
import { HomePage } from "../pages/HomePage";
import { MeasurementGuidePage } from "../pages/MeasurementGuidePage";
import { ReturnPolicyPage } from "../pages/ReturnPolicyPage";
import { ShopPage } from "../pages/ShopPage";

export const routes = [
  { path: "/", label: "Home", element: HomePage },
  { path: "/shop", label: "Shop", element: ShopPage },
  { path: "/measurements", label: "Measurements", element: MeasurementGuidePage },
  { path: "/cart", label: "Cart", element: CartPage },
  { path: "/checkout", label: "Checkout", element: CheckoutPage },
  { path: "/account", label: "Account", element: AccountPage },
  { path: "/return-policy", label: "Return Policy", element: ReturnPolicyPage },
] as const;

