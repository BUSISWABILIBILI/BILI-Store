import type {
  AdminCategory,
  AdminColour,
  AdminCustomerDetails,
  AdminCustomerSummary,
  AdminDashboard,
  AdminOrderDetails,
  AdminOrderSummary,
  AdminPaymentSummary,
  AdminProductDetails,
  AdminProductSummary,
  AdminReportRange,
  AdminReturnRequestSummary,
  UpdateAdminDeliveryStatusRequest,
  UpdateAdminOrderStatusRequest,
  UpdateReturnRequestStatusRequest,
  UpsertAdminCategoryRequest,
  UpsertAdminColourRequest,
  UpsertAdminProductRequest,
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

export async function updateAdminDeliveryStatus(
  orderNumber: string,
  request: UpdateAdminDeliveryStatusRequest,
) {
  const response = await apiClient.put<AdminOrderDetails>(
    `/admin/orders/${orderNumber}/delivery-status`,
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

export async function getAdminProducts() {
  const response = await apiClient.get<AdminProductSummary[]>("/admin/products");
  return response.data;
}

export async function getAdminProduct(id: string) {
  const response = await apiClient.get<AdminProductDetails>(`/admin/products/${id}`);
  return response.data;
}

export async function createAdminProduct(request: UpsertAdminProductRequest) {
  const response = await apiClient.post<AdminProductDetails>("/admin/products", request);
  return response.data;
}

export async function updateAdminProduct(id: string, request: UpsertAdminProductRequest) {
  const response = await apiClient.put<AdminProductDetails>(`/admin/products/${id}`, request);
  return response.data;
}

export async function deactivateAdminProduct(id: string) {
  const response = await apiClient.delete<AdminProductDetails>(`/admin/products/${id}`);
  return response.data;
}

export async function getAdminCategories() {
  const response = await apiClient.get<AdminCategory[]>("/admin/categories");
  return response.data;
}

export async function createAdminCategory(request: UpsertAdminCategoryRequest) {
  const response = await apiClient.post<AdminCategory>("/admin/categories", request);
  return response.data;
}

export async function updateAdminCategory(id: string, request: UpsertAdminCategoryRequest) {
  const response = await apiClient.put<AdminCategory>(`/admin/categories/${id}`, request);
  return response.data;
}

export async function getAdminColours() {
  const response = await apiClient.get<AdminColour[]>("/admin/colours");
  return response.data;
}

export async function createAdminColour(request: UpsertAdminColourRequest) {
  const response = await apiClient.post<AdminColour>("/admin/colours", request);
  return response.data;
}

export async function updateAdminColour(id: string, request: UpsertAdminColourRequest) {
  const response = await apiClient.put<AdminColour>(`/admin/colours/${id}`, request);
  return response.data;
}

export async function getAdminDashboard(range?: Partial<AdminReportRange>) {
  const response = await apiClient.get<AdminDashboard>("/admin/dashboard", {
    params: cleanReportRange(range),
  });
  return response.data;
}

export async function getAdminOrdersReport(range?: Partial<AdminReportRange>) {
  const response = await apiClient.get<Blob>("/admin/reports/orders.csv", {
    params: cleanReportRange(range),
    responseType: "blob",
  });

  return response.data;
}

export async function getAdminCustomersReport(range?: Partial<AdminReportRange>) {
  const response = await apiClient.get<Blob>("/admin/reports/customers.csv", {
    params: cleanReportRange(range),
    responseType: "blob",
  });

  return response.data;
}

export async function getAdminCustomers() {
  const response = await apiClient.get<AdminCustomerSummary[]>("/admin/customers");
  return response.data;
}

export async function getAdminCustomer(id: string) {
  const response = await apiClient.get<AdminCustomerDetails>(`/admin/customers/${id}`);
  return response.data;
}

function cleanReportRange(range?: Partial<AdminReportRange>) {
  return {
    ...(range?.dateFrom ? { dateFrom: range.dateFrom } : {}),
    ...(range?.dateTo ? { dateTo: range.dateTo } : {}),
  };
}
