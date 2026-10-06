import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  AlertTriangle,
  Boxes,
  CalendarDays,
  ClipboardList,
  CreditCard,
  Download,
  Filter,
  LayoutDashboard,
  RotateCcw,
  UsersRound,
} from "lucide-react";
import { Link } from "react-router-dom";
import { getAdminCustomersReport, getAdminDashboard, getAdminOrdersReport } from "../services/adminApi";
import { useAuthStore } from "../stores/authStore";
import { formatCurrency } from "../utils/formatters";

function formatDate(value: string) {
  return new Intl.DateTimeFormat("en-ZA", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(new Date(value));
}

function buildReportFileName(reportName: "orders" | "customers", dateFrom: string, dateTo: string) {
  const range = dateFrom || dateTo ? `-${dateFrom || "start"}-${dateTo || "today"}` : "";

  return `bili-${reportName}-report${range}.csv`;
}

function downloadBlob(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = fileName;
  document.body.appendChild(anchor);
  anchor.click();
  anchor.remove();
  URL.revokeObjectURL(url);
}

export function AdminDashboardPage() {
  const accessToken = useAuthStore((state) => state.accessToken);
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [reportMessage, setReportMessage] = useState<string | null>(null);
  const [isExporting, setIsExporting] = useState(false);
  const isDateRangeInvalid = Boolean(dateFrom && dateTo && dateFrom > dateTo);

  const dashboardQuery = useQuery({
    queryKey: ["admin", "dashboard", dateFrom, dateTo],
    queryFn: () => getAdminDashboard({ dateFrom, dateTo }),
    enabled: Boolean(accessToken && isAdmin && !isDateRangeInvalid),
  });

  async function handleReportDownload(reportName: "orders" | "customers") {
    if (isDateRangeInvalid) {
      setReportMessage("Choose a start date that is before the end date.");
      return;
    }

    setIsExporting(true);
    setReportMessage(null);

    try {
      const blob =
        reportName === "orders"
          ? await getAdminOrdersReport({ dateFrom, dateTo })
          : await getAdminCustomersReport({ dateFrom, dateTo });

      downloadBlob(blob, buildReportFileName(reportName, dateFrom, dateTo));
      setReportMessage(`${reportName === "orders" ? "Orders" : "Customers"} report downloaded.`);
    } catch {
      setReportMessage("Report download failed. Check admin permissions and try again.");
    } finally {
      setIsExporting(false);
    }
  }

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

      <section className="admin-report-panel">
        <div className="admin-report-heading">
          <Filter aria-hidden="true" size={24} />
          <div>
            <p className="eyebrow">Reports</p>
            <h2>Reporting range</h2>
            <p>Filter dashboard activity and download CSV reports for orders or customers.</p>
          </div>
        </div>

        <div className="admin-report-grid">
          <label className="admin-field">
            <span>From</span>
            <input type="date" value={dateFrom} onChange={(event) => setDateFrom(event.target.value)} />
          </label>
          <label className="admin-field">
            <span>To</span>
            <input type="date" value={dateTo} onChange={(event) => setDateTo(event.target.value)} />
          </label>
          <div className="admin-report-actions">
            <button
              className="button button-light"
              disabled={!dateFrom && !dateTo}
              type="button"
              onClick={() => {
                setDateFrom("");
                setDateTo("");
                setReportMessage(null);
              }}
            >
              <CalendarDays aria-hidden="true" size={18} />
              Clear
            </button>
            <button
              className="button button-light"
              disabled={isExporting || isDateRangeInvalid}
              type="button"
              onClick={() => void handleReportDownload("customers")}
            >
              <Download aria-hidden="true" size={18} />
              Customers CSV
            </button>
            <button
              className="button button-primary"
              disabled={isExporting || isDateRangeInvalid}
              type="button"
              onClick={() => void handleReportDownload("orders")}
            >
              <Download aria-hidden="true" size={18} />
              Orders CSV
            </button>
          </div>
        </div>

        {isDateRangeInvalid ? <p className="inline-alert">Start date must be before the end date.</p> : null}
        {reportMessage ? <p className="inline-alert">{reportMessage}</p> : null}
      </section>

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
