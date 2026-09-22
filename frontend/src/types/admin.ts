import type { OrderConfirmation } from "./orders";

export type AdminOrderSummary = {
  id: string;
  orderNumber: string;
  orderDate: string;
  customerName: string;
  customerEmail: string;
  total: number;
  paymentStatus: string;
  orderStatus: string;
  deliveryStatus: string;
  itemCount: number;
  latestPaymentReference: string | null;
};

export type AdminPaymentSummary = {
  id: string;
  orderNumber: string;
  customerEmail: string;
  provider: string;
  providerReference: string | null;
  amount: number;
  currency: string;
  status: string;
  createdAt: string;
};

export type AdminReturnRequestSummary = {
  id: string;
  orderNumber: string;
  customerEmail: string;
  reason: string;
  details: string;
  status: string;
  requestedAt: string;
  resolvedAt: string | null;
};

export type UpdateAdminOrderStatusRequest = {
  orderStatus: string;
  paymentStatus: string | null;
};

export type UpdateReturnRequestStatusRequest = {
  status: string;
};

export type AdminOrderDetails = OrderConfirmation;
