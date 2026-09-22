import type {
  AdminOrderDetails,
  AdminOrderSummary,
  AdminPaymentSummary,
  AdminReturnRequestSummary,
  UpdateAdminOrderStatusRequest,
  UpdateReturnRequestStatusRequest,
} from "../types/admin";
import { apiClient } from "./apiClient";

export async function getAdminOrders() {
  const response = await apiClient.get<AdminOrderSummary[]>("/admin/orders");
  return response.data;
}

export async function getAdminOrder(orderNumber: string) {
  const response = await apiClient.get<AdminOrderDetails>(`/admin/orders/${orderNumber}`);
  return response.data;
}

export async function updateAdminOrderStatus(
  orderNumber: string,
  request: UpdateAdminOrderStatusRequest,
) {
  const response = await apiClient.put<AdminOrderDetails>(
    `/admin/orders/${orderNumber}/status`,
    request,
  );
  return response.data;
}

export async function getAdminPayments() {
  const response = await apiClient.get<AdminPaymentSummary[]>("/admin/payments");
  return response.data;
}

export async function getAdminReturnRequests() {
  const response = await apiClient.get<AdminReturnRequestSummary[]>("/admin/return-requests");
  return response.data;
}

export async function updateAdminReturnRequestStatus(
  id: string,
  request: UpdateReturnRequestStatusRequest,
) {
  const response = await apiClient.put<AdminReturnRequestSummary>(
    `/admin/return-requests/${id}/status`,
    request,
  );
  return response.data;
}
