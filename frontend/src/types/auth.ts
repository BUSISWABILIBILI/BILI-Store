export type CustomerProfile = {
  id: string;
  applicationUserId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
};

export type AuthResponse = {
  accessToken: string;
  expiresAt: string;
  customer: CustomerProfile;
  roles: string[];
};

export type RegisterCustomerRequest = {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
};

export type LoginCustomerRequest = {
  email: string;
  password: string;
};

export type UpdateCustomerProfileRequest = {
  firstName: string;
  lastName: string;
  phone: string;
};

export type ForgotPasswordRequest = {
  email: string;
};

export type CustomerMeasurementValue = {
  measurementTypeId: string;
  measurementKey: string;
  measurementName: string;
  valueCm: number;
  unit: string;
};

export type CustomerMeasurementProfile = {
  id: string;
  name: string;
  isDefault: boolean;
  notes: string | null;
  measurements: CustomerMeasurementValue[];
};

export type CreateCustomerMeasurementProfileRequest = {
  name: string;
  isDefault: boolean;
  notes: string | null;
  measurements: Array<{
    measurementKey: string;
    valueCm: number;
  }>;
};

export type CustomerOrderSummary = {
  id: string;
  orderNumber: string;
  orderDate: string;
  total: number;
  paymentStatus: string;
  orderStatus: string;
  itemCount: number;
};
