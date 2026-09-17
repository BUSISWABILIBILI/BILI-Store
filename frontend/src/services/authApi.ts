import { apiClient } from "./apiClient";
import type {
  AuthResponse,
  CreateCustomerMeasurementProfileRequest,
  CustomerOrderSummary,
  CustomerMeasurementProfile,
  CustomerProfile,
  ForgotPasswordRequest,
  LoginCustomerRequest,
  RegisterCustomerRequest,
  UpdateCustomerProfileRequest,
} from "../types/auth";

export async function registerCustomer(request: RegisterCustomerRequest) {
  const response = await apiClient.post<AuthResponse>("/auth/register", request);

  return response.data;
}

export async function loginCustomer(request: LoginCustomerRequest) {
  const response = await apiClient.post<AuthResponse>("/auth/login", request);

  return response.data;
}

export async function getCurrentCustomer() {
  const response = await apiClient.get<CustomerProfile>("/auth/me");

  return response.data;
}

export async function updateCurrentCustomer(request: UpdateCustomerProfileRequest) {
  const response = await apiClient.put<CustomerProfile>("/auth/me", request);

  return response.data;
}

export async function getCustomerMeasurementProfiles() {
  const response = await apiClient.get<CustomerMeasurementProfile[]>("/customers/me/measurement-profiles");

  return response.data;
}

export async function createCustomerMeasurementProfile(request: CreateCustomerMeasurementProfileRequest) {
  const response = await apiClient.post<CustomerMeasurementProfile>(
    "/customers/me/measurement-profiles",
    request,
  );

  return response.data;
}

export async function requestPasswordReset(request: ForgotPasswordRequest) {
  const response = await apiClient.post<{ message: string }>("/auth/forgot-password", request);

  return response.data;
}

export async function getCustomerOrders() {
  const response = await apiClient.get<CustomerOrderSummary[]>("/customers/me/orders");

  return response.data;
}
