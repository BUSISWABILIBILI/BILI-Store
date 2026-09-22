import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { ClipboardList, Ruler, Search, ShieldCheck, UsersRound } from "lucide-react";
import { getAdminCustomer, getAdminCustomers } from "../services/adminApi";
import { useAuthStore } from "../stores/authStore";
import { formatCurrency } from "../utils/formatters";

function formatDate(value: string | null) {
  if (!value) {
    return "None yet";
  }

  return new Intl.DateTimeFormat("en-ZA", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(new Date(value));
}

export function AdminCustomersPage() {
  const accessToken = useAuthStore((state) => state.accessToken);
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");
  const [selectedCustomerId, setSelectedCustomerId] = useState("");
  const [searchTerm, setSearchTerm] = useState("");

  const customersQuery = useQuery({
    queryKey: ["admin", "customers"],
    queryFn: getAdminCustomers,
    enabled: Boolean(accessToken && isAdmin),
  });

  const selectedCustomerQuery = useQuery({
    queryKey: ["admin", "customers", selectedCustomerId],
    queryFn: () => getAdminCustomer(selectedCustomerId),
    enabled: Boolean(accessToken && isAdmin && selectedCustomerId),
  });

  const filteredCustomers = useMemo(() => {
    const search = searchTerm.trim().toLowerCase();

    if (!search) {
      return customersQuery.data ?? [];
    }

    return (customersQuery.data ?? []).filter((customer) => {
      const fullName = `${customer.firstName} ${customer.lastName}`.toLowerCase();
      return fullName.includes(search) || customer.email.toLowerCase().includes(search) || customer.phone.includes(search);
    });
  }, [customersQuery.data, searchTerm]);

  useEffect(() => {
    if (!selectedCustomerId && filteredCustomers.length) {
      setSelectedCustomerId(filteredCustomers[0].id);
    }
  }, [filteredCustomers, selectedCustomerId]);

  if (!accessToken || !isAdmin) {
    return (
      <section className="content-panel">
        <p className="eyebrow">Admin</p>
        <h1>Admin access required</h1>
        <p>Log in with an account assigned to the Admin role to review customer records.</p>
      </section>
    );
  }

  return (
    <div className="admin-page">
      <section className="admin-hero">
        <UsersRound aria-hidden="true" size={30} />
        <div>
          <p className="eyebrow">Admin customers</p>
          <h1>Customer lookup</h1>
          <p>Review registered customers, order history, and saved measurement profile coverage.</p>
        </div>
      </section>

      {customersQuery.isError ? (
        <p className="inline-alert">Customers could not be loaded. Check admin permissions.</p>
      ) : null}

      <div className="admin-layout">
        <section className="admin-panel">
          <UsersRound aria-hidden="true" size={24} />
          <h2>Customers</h2>
          <label className="search-shell admin-search-shell">
            <Search aria-hidden="true" size={20} />
            <input
              placeholder="Search customer"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
            />
          </label>
          <div className="admin-customer-list">
            {filteredCustomers.length ? (
              filteredCustomers.map((customer) => (
                <button
                  className={selectedCustomerId === customer.id ? "admin-customer-row active" : "admin-customer-row"}
                  key={customer.id}
                  onClick={() => setSelectedCustomerId(customer.id)}
                  type="button"
                >
                  <span>
                    <strong>{customer.firstName} {customer.lastName}</strong>
                    <small>{customer.email}</small>
                  </span>
                  <span>{customer.orderCount} orders</span>
                  <span>{formatCurrency(customer.totalSpent)}</span>
                </button>
              ))
            ) : (
              <p>No matching customers.</p>
            )}
          </div>
        </section>

        <section className="admin-panel">
          <ShieldCheck aria-hidden="true" size={24} />
          <h2>Selected customer</h2>
          {selectedCustomerQuery.data ? (
            <>
              <div className="admin-detail-grid">
                <div>
                  <span>Name</span>
                  <strong>{selectedCustomerQuery.data.firstName} {selectedCustomerQuery.data.lastName}</strong>
                </div>
                <div>
                  <span>Email</span>
                  <strong>{selectedCustomerQuery.data.email}</strong>
                </div>
                <div>
                  <span>Phone</span>
                  <strong>{selectedCustomerQuery.data.phone}</strong>
                </div>
                <div>
                  <span>Status</span>
                  <strong>{selectedCustomerQuery.data.isActive ? "Active" : "Inactive"}</strong>
                </div>
                <div>
                  <span>Orders</span>
                  <strong>{selectedCustomerQuery.data.orderCount}</strong>
                </div>
                <div>
                  <span>Total spent</span>
                  <strong>{formatCurrency(selectedCustomerQuery.data.totalSpent)}</strong>
                </div>
                <div>
                  <span>Measurements</span>
                  <strong>{selectedCustomerQuery.data.measurementProfileCount}</strong>
                </div>
                <div>
                  <span>Latest order</span>
                  <strong>{formatDate(selectedCustomerQuery.data.latestOrderDate)}</strong>
                </div>
              </div>

              <section className="admin-subsection">
                <ClipboardList aria-hidden="true" size={22} />
                <h3>Orders</h3>
                <div className="admin-compact-list">
                  {selectedCustomerQuery.data.orders.length ? (
                    selectedCustomerQuery.data.orders.map((order) => (
                      <article key={order.id}>
                        <strong>{order.orderNumber}</strong>
                        <span>{formatDate(order.orderDate)}</span>
                        <span>
                          {order.orderStatus} / {order.paymentStatus} / {formatCurrency(order.total)}
                        </span>
                      </article>
                    ))
                  ) : (
                    <p>No registered-customer orders yet.</p>
                  )}
                </div>
              </section>

              <section className="admin-subsection">
                <Ruler aria-hidden="true" size={22} />
                <h3>Measurement profiles</h3>
                <div className="admin-measurement-profile-list">
                  {selectedCustomerQuery.data.measurementProfiles.length ? (
                    selectedCustomerQuery.data.measurementProfiles.map((profile) => (
                      <article key={profile.id}>
                        <div>
                          <strong>{profile.name}</strong>
                          <span>{profile.isDefault ? "Default" : `${profile.measurementCount} measurements`}</span>
                        </div>
                        {profile.notes ? <p>{profile.notes}</p> : null}
                        <div className="admin-measurement-chip-row">
                          {profile.measurements.map((measurement) => (
                            <span key={measurement.measurementKey}>
                              {measurement.measurementName}: {measurement.valueCm} {measurement.unit}
                            </span>
                          ))}
                        </div>
                      </article>
                    ))
                  ) : (
                    <p>No saved measurement profiles.</p>
                  )}
                </div>
              </section>
            </>
          ) : (
            <p>Select a customer to review account details.</p>
          )}
        </section>
      </div>
    </div>
  );
}
