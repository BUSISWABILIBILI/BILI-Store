import type { CreatePaymentSessionRequest, CreatePaymentSessionResponse } from "../types/payments";
import { apiClient } from "./apiClient";

export async function createPaymentSession(request: CreatePaymentSessionRequest) {
  const response = await apiClient.post<CreatePaymentSessionResponse>(
    "/payments/create-session",
    request,
  );

  return response.data;
}
