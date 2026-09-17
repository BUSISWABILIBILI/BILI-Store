import type { CreateOrderRequest, CreateOrderResponse, OrderConfirmation } from "../types/orders";
import { apiClient } from "./apiClient";

export async function createOrder(request: CreateOrderRequest) {
  const response = await apiClient.post<CreateOrderResponse>("/orders", request);
  return response.data;
}

export async function getOrder(orderNumber: string) {
  const response = await apiClient.get<OrderConfirmation>(`/orders/${orderNumber}`);
  return response.data;
}
