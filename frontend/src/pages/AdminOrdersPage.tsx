import { useEffect, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ClipboardList, CreditCard, RotateCcw, Save, ShieldCheck } from "lucide-react";
import {
  getAdminOrder,
  getAdminOrders,
  getAdminPayments,
  getAdminReturnRequests,
  updateAdminOrderStatus,
  updateAdminReturnRequestStatus,
} from "../services/adminApi";
import { useAuthStore } from "../stores/authStore";
import { formatCurrency } from "../utils/formatters";

const orderStatusOptions = [
  "PendingPayment",
  "Paid",
  "MeasurementsConfirmed",
  "InProduction",
  "ReadyForDelivery",
  "SentViaPaxi",
  "ReadyForCollection",
  "Completed",
  "Cancelled",
];

const paymentStatusOptions = ["Pending", "Paid", "Failed", "Cancelled", "Refunded"];
const returnStatusOptions = [
  "Submitted",
  "UnderReview",
  "ApprovedForRemake",
  "ApprovedForRefund",
  "Rejected",
  "Closed",
];

export function AdminOrdersPage() {
  const queryClient = useQueryClient();
  const accessToken = useAuthStore((state) => state.accessToken);
  const roles = useAuthStore((state) => state.roles);
  const isAdmin = roles.includes("Admin");
  const [selectedOrderNumber, setSelectedOrderNumber] = useState("");
  const [orderStatus, setOrderStatus] = useState(orderStatusOptions[0]);
  const [paymentStatus, setPaymentStatus] = useState(paymentStatusOptions[0]);
  const [returnStatusDrafts, setReturnStatusDrafts] = useState<Record<string, string>>({});

  const ordersQuery = useQuery({
    queryKey: ["admin", "orders"],
    queryFn: getAdminOrders,
    enabled: Boolean(accessToken && isAdmin),
  });

  const selectedOrderQuery = useQuery({
    queryKey: ["admin", "orders", selectedOrderNumber],
    queryFn: () => getAdminOrder(selectedOrderNumber),
    enabled: Boolean(accessToken && isAdmin && selectedOrderNumber),
  });

  const paymentsQuery = useQuery({
    queryKey: ["admin", "payments"],
    queryFn: getAdminPayments,
    enabled: Boolean(accessToken && isAdmin),
  });

  const returnRequestsQuery = useQuery({
    queryKey: ["admin", "return-requests"],
    queryFn: getAdminReturnRequests,
    enabled: Boolean(accessToken && isAdmin),
  });

  const updateStatusMutation = useMutation({
    mutationFn: () =>
      updateAdminOrderStatus(selectedOrderNumber, {
        orderStatus,
        paymentStatus,
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["admin", "orders"] });
      await queryClient.invalidateQueries({ queryKey: ["admin", "orders", selectedOrderNumber] });
    },
  });

  const updateReturnStatusMutation = useMutation({
    mutationFn: ({ id, status }: { id: string; status: string }) =>
      updateAdminReturnRequestStatus(id, { status }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["admin", "return-requests"] });
    },
  });

  useEffect(() => {
    if (!selectedOrderNumber && ordersQuery.data?.length) {
      setSelectedOrderNumber(ordersQuery.data[0].orderNumber);
    }
  }, [ordersQuery.data, selectedOrderNumber]);

  useEffect(() => {
    if (!selectedOrderQuery.data) {
      return;
    }

    setOrderStatus(selectedOrderQuery.data.orderStatus);
    setPaymentStatus(selectedOrderQuery.data.paymentStatus);
  }, [selectedOrderQuery.data]);

  if (!accessToken || !isAdmin) {
    return (
      <section className="content-panel">
        <p className="eyebrow">Admin</p>
        <h1>Admin access required</h1>
        <p>Log in with an account assigned to the Admin role to manage orders.</p>
      </section>
    );
  }

  return (
    <div className="admin-page">
      <section className="admin-hero">
        <ShieldCheck aria-hidden="true" size={30} />
        <div>
          <p className="eyebrow">Admin orders</p>
          <h1>Order management</h1>
          <p>Review orders, payment references, delivery status, and return request workflow.</p>
        </div>
      </section>

      {ordersQuery.isError ? (
        <p className="inline-alert">Admin orders could not be loaded. Check admin permissions.</p>
      ) : null}

      <div className="admin-layout">
        <section className="admin-panel">
          <ClipboardList aria-hidden="true" size={24} />
          <h2>Order queue</h2>
          <div className="admin-order-list">
            {ordersQuery.data?.length ? (
              ordersQuery.data.map((order) => (
                <button
                  className={selectedOrderNumber === order.orderNumber ? "admin-order-row active" : "admin-order-row"}
                  key={order.id}
                  onClick={() => setSelectedOrderNumber(order.orderNumber)}
                  type="button"
                >
                  <span>
                    <strong>{order.orderNumber}</strong>
                    <small>{order.customerEmail}</small>
                  </span>
                  <span>{order.orderStatus}</span>
                  <span>{formatCurrency(order.total)}</span>
                </button>
              ))
            ) : (
              <p>No orders saved yet.</p>
            )}
          </div>
        </section>

        <section className="admin-panel">
          <ShieldCheck aria-hidden="true" size={24} />
          <h2>Selected order</h2>
          {selectedOrderQuery.data ? (
            <>
              <div className="admin-detail-grid">
                <div>
                  <span>Order</span>
                  <strong>{selectedOrderQuery.data.orderNumber}</strong>
                </div>
                <div>
                  <span>Customer</span>
                  <strong>{selectedOrderQuery.data.customer.email}</strong>
                </div>
                <div>
                  <span>Total</span>
                  <strong>{formatCurrency(selectedOrderQuery.data.total)}</strong>
                </div>
                <div>
                  <span>Delivery</span>
                  <strong>{selectedOrderQuery.data.delivery.deliveryStatus}</strong>
                </div>
              </div>

              <div className="admin-status-form">
                <label>
                  <span>Order status</span>
                  <select value={orderStatus} onChange={(event) => setOrderStatus(event.target.value)}>
                    {orderStatusOptions.map((status) => (
                      <option key={status} value={status}>
                        {status}
                      </option>
                    ))}
                  </select>
                </label>
                <label>
                  <span>Payment status</span>
                  <select value={paymentStatus} onChange={(event) => setPaymentStatus(event.target.value)}>
                    {paymentStatusOptions.map((status) => (
                      <option key={status} value={status}>
                        {status}
                      </option>
                    ))}
                  </select>
                </label>
                <button
                  className="button button-primary"
                  disabled={updateStatusMutation.isPending}
                  onClick={() => updateStatusMutation.mutate()}
                  type="button"
                >
                  <Save aria-hidden="true" size={18} />
                  {updateStatusMutation.isPending ? "Saving..." : "Save status"}
                </button>
              </div>

              <div className="admin-item-list">
                {selectedOrderQuery.data.items.map((item) => (
                  <article key={item.id}>
                    <strong>{item.productName}</strong>
                    <span>{item.quantity} item(s)</span>
                    <span>{formatCurrency(item.subtotal)}</span>
                  </article>
                ))}
              </div>
            </>
          ) : (
            <p>Select an order to review its details.</p>
          )}
        </section>
      </div>

      <div className="admin-layout">
        <section className="admin-panel">
          <CreditCard aria-hidden="true" size={24} />
          <h2>Payments</h2>
          <div className="admin-compact-list">
            {paymentsQuery.data?.length ? (
              paymentsQuery.data.map((payment) => (
                <article key={payment.id}>
                  <strong>{payment.providerReference}</strong>
                  <span>{payment.orderNumber}</span>
                  <span>
                    {payment.status} / {formatCurrency(payment.amount)}
                  </span>
                </article>
              ))
            ) : (
              <p>No payment references yet.</p>
            )}
          </div>
        </section>

        <section className="admin-panel">
          <RotateCcw aria-hidden="true" size={24} />
          <h2>Return requests</h2>
          <div className="admin-compact-list">
            {returnRequestsQuery.data?.length ? (
              returnRequestsQuery.data.map((request) => {
                const draftStatus = returnStatusDrafts[request.id] ?? request.status;

                return (
                  <article key={request.id}>
                    <strong>{request.orderNumber}</strong>
                    <span>{request.reason}</span>
                    <label className="admin-inline-select">
                      <span>Status</span>
                      <select
                        value={draftStatus}
                        onChange={(event) =>
                          setReturnStatusDrafts((current) => ({
                            ...current,
                            [request.id]: event.target.value,
                          }))
                        }
                      >
                        {returnStatusOptions.map((status) => (
                          <option key={status} value={status}>
                            {status}
                          </option>
                        ))}
                      </select>
                    </label>
                    <button
                      className="button button-light"
                      disabled={updateReturnStatusMutation.isPending}
                      onClick={() => updateReturnStatusMutation.mutate({ id: request.id, status: draftStatus })}
                      type="button"
                    >
                      Save
                    </button>
                  </article>
                );
              })
            ) : (
              <p>No return requests submitted yet.</p>
            )}
          </div>
        </section>
      </div>
    </div>
  );
}
