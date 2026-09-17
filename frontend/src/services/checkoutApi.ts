import type { CheckoutReviewRequest, CheckoutReviewResponse } from "../types/checkout";
import { apiClient } from "./apiClient";

export async function reviewCheckout(request: CheckoutReviewRequest) {
  const response = await apiClient.post<CheckoutReviewResponse>("/checkout/review", request);
  return response.data;
}
