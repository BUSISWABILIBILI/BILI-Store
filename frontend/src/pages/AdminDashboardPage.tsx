import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, Boxes, ClipboardList, CreditCard, LayoutDashboard, RotateCcw, UsersRound } from "lucide-react";
import { Link } from "react-router-dom";
import { getAdminDashboard } from "../services/adminApi";
import { useAuthStore } from "../stores/authStore";
import { formatCurrency } from "../utils/formatters";

function formatDate(value: string) {
  return new Intl.DateTimeFormat("en-ZA", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(new Date(value));
}

export function AdminDashboardPage() {
  const accessToken = useAuthStore((state) => state.accessToken);
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");

  const dashboardQuery = useQuery({
    queryKey: ["admin", "dashboard"],
    queryFn: getAdminDashboard,
    enabled: Boolean(accessToken && isAdmin),
  });

  if (!accessToken || !isAdmin) {
    return (
      <section className="content-panel">
        <p className="eyebrow">Admin</p>
        <h1>Admin access required</h1>
        <p>Log in with an account assigned to the Admin role to view store dashboard data.</p>
      </section>
    );
  }

  const dashboard = dashboardQuery.data;
  const stats = dashboard
    ? [
        {
          label: "Orders",
          value: dashboard.orderCount,
          detail: `${dashboard.pendingOrderCount} pending payment`,
          icon: ClipboardList,
        },
        {
          label: "Paid revenue",
          value: formatCurrency(dashboard.paidRevenue),
          detail: `${formatCurrency(dashboard.pendingRevenue)} pending`,
          icon: CreditCard,
        },
        {
          label: "Customers",
          value: dashboard.customerCount,
          detail: `${dashboard.guestOrderCount} guest orders`,
          icon: UsersRound,
        },
        {
          label: "Returns",
          value: dashboard.returnRequestCount,
          detail: `${dashboard.openReturnRequestCount} open`,
          icon: RotateCcw,
        },
      ]
    : [];

  return (
    <div className="admin-page">
      <section className="admin-hero">
        <LayoutDashboard aria-hidden="true" size={30} />
        <div>
          <p className="eyebrow">Admin dashboard</p>
          <h1>Store overview</h1>
          <p>Track order activity, revenue, customers, returns, and catalogue items that need attention.</p>
        </div>
      </section>

      {dashboardQuery.isError ? (
        <p className="inline-alert">Dashboard data could not be loaded. Check admin permissions.</p>
      ) : null}

      {dashboard ? (
        <>
          <section className="admin-stat-grid">
            {stats.map((stat) => {
              const Icon = stat.icon;

              return (
                <article className="admin-stat-card" key={stat.label}>
                  <Icon aria-hidden="true" size={24} />
                  <span>{stat.label}</span>
                  <strong>{stat.value}</strong>
                  <small>{stat.detail}</small>
                </article>
              );
            })}
          </section>

          <section className="admin-link-grid">
            <Link to="/admin/orders">
              <ClipboardList aria-hidden="true" size={22} />
              <span>Orders</span>
            </Link>
            <Link to="/admin/catalogue">
              <Boxes aria-hidden="true" size={22} />
              <span>Catalogue</span>
            </Link>
            <Link to="/admin/customers">
              <UsersRound aria-hidden="true" size={22} />
              <span>Customers</span>
            </Link>
          </section>

          <div className="admin-layout">
            <section className="admin-panel">
              <ClipboardList aria-hidden="true" size={24} />
              <h2>Recent orders</h2>
              <div className="admin-compact-list">
                {dashboard.recentOrders.length ? (
                  dashboard.recentOrders.map((order) => (
                    <article key={order.orderNumber}>
                      <strong>{order.orderNumber}</strong>
                      <span>{order.customerName || "Customer"}</span>
                      <span>
                        {order.orderStatus} / {formatCurrency(order.total)} / {formatDate(order.orderDate)}
                      </span>
                    </article>
                  ))
                ) : (
                  <p>No orders saved yet.</p>
                )}
              </div>
            </section>

            <section className="admin-panel">
              <AlertTriangle aria-hidden="true" size={24} />
              <h2>Catalogue alerts</h2>
              <div className="admin-compact-list">
                {dashboard.productAlerts.length ? (
                  dashboard.productAlerts.map((alert) => (
                    <article key={`${alert.id}-${alert.issue}`}>
                      <strong>{alert.name}</strong>
                      <span>{alert.issue}</span>
                      <span>{alert.isFeatured ? "Featured" : "Standard"}</span>
                    </article>
                  ))
                ) : (
                  <p>No catalogue alerts right now.</p>
                )}
              </div>
            </section>
          </div>

          <div className="admin-layout">
            <section className="admin-panel">
              <ClipboardList aria-hidden="true" size={24} />
              <h2>Order statuses</h2>
              <div className="admin-status-list">
                {dashboard.ordersByStatus.map((status) => (
                  <div key={status.status}>
                    <span>{status.status}</span>
                    <strong>{status.count}</strong>
                  </div>
                ))}
              </div>
            </section>

            <section className="admin-panel">
              <CreditCard aria-hidden="true" size={24} />
              <h2>Payment statuses</h2>
              <div className="admin-status-list">
                {dashboard.paymentsByStatus.map((status) => (
                  <div key={status.status}>
                    <span>{status.status}</span>
                    <strong>{status.count}</strong>
                  </div>
                ))}
              </div>
            </section>
          </div>
        </>
      ) : (
        <section className="admin-panel">
          <LayoutDashboard aria-hidden="true" size={24} />
          <h2>Loading dashboard</h2>
          <p>Preparing admin summary data.</p>
        </section>
      )}
    </div>
  );
}
