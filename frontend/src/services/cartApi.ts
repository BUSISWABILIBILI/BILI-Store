import { apiClient } from "./apiClient";
import type { CartQuoteRequest, CartQuoteResponse } from "../types/cart";

export async function quoteCart(request: CartQuoteRequest) {
  const response = await apiClient.post<CartQuoteResponse>("/cart/quote", request);

  return response.data;
}
