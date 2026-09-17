import { apiClient } from "./apiClient";
import type { ApiInfoResponse, HealthCheckResponse } from "../types/system";

export async function getApiHealth(): Promise<HealthCheckResponse> {
  const response = await apiClient.get<HealthCheckResponse>("/health");
  return response.data;
}

export async function getApiInfo(): Promise<ApiInfoResponse> {
  const response = await apiClient.get<ApiInfoResponse>("/system");
  return response.data;
}

