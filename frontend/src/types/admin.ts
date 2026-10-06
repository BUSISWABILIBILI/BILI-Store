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

export type UpdateAdminDeliveryStatusRequest = {
  deliveryStatus: string;
};

export type UpdateReturnRequestStatusRequest = {
  status: string;
};

export type AdminOrderDetails = OrderConfirmation;

export type AdminCategory = {
  id: string;
  name: string;
  slug: string;
  parentCategoryId: string | null;
  parentCategoryName: string | null;
  displayOrder: number;
  isActive: boolean;
};

export type UpsertAdminCategoryRequest = {
  name: string;
  slug: string | null;
  parentCategoryId: string | null;
  displayOrder: number;
  isActive: boolean;
};

export type AdminColour = {
  id: string;
  name: string;
  hexCode: string;
  isActive: boolean;
};

export type UpsertAdminColourRequest = {
  name: string;
  hexCode: string;
  isActive: boolean;
};

export type AdminProductMeasurementRequirement = {
  id: string;
  measurementTypeId: string;
  measurementName: string;
  measurementKey: string;
  isRequired: boolean;
  minValueCm: number | null;
  maxValueCm: number | null;
  helpText: string | null;
};

export type AdminProductMeasurementRequirementInput = {
  measurementTypeId: string;
  isRequired: boolean;
  minValueCm: number | null;
  maxValueCm: number | null;
  helpText: string | null;
};

export type AdminProductSummary = {
  id: string;
  name: string;
  slug: string;
  price: number;
  clothingType: string;
  estimatedProductionDays: number;
  isAvailable: boolean;
  isFeatured: boolean;
  allowsCustomColor: boolean;
  primaryImageUrl: string;
  categoryNames: string[];
  colourNames: string[];
  requiredMeasurementNames: string[];
};

export type AdminProductDetails = {
  id: string;
  name: string;
  slug: string;
  description: string;
  price: number;
  clothingType: string;
  estimatedProductionDays: number;
  isAvailable: boolean;
  isFeatured: boolean;
  allowsCustomColor: boolean;
  fabricDescription: string | null;
  primaryImageUrl: string;
  primaryImageAltText: string;
  categoryIds: string[];
  colourIds: string[];
  measurementRequirements: AdminProductMeasurementRequirement[];
};

export type UpsertAdminProductRequest = {
  name: string;
  slug: string | null;
  description: string;
  price: number;
  clothingType: string;
  estimatedProductionDays: number;
  isAvailable: boolean;
  isFeatured: boolean;
  allowsCustomColor: boolean;
  fabricDescription: string | null;
  primaryImageUrl: string | null;
  primaryImageAltText: string | null;
  categoryIds: string[];
  colourIds: string[];
  measurementRequirements: AdminProductMeasurementRequirementInput[];
};

export type AdminDashboardStatusCount = {
  status: string;
  count: number;
};

export type AdminDashboardRecentOrder = {
  orderNumber: string;
  customerName: string;
  total: number;
  orderStatus: string;
  paymentStatus: string;
  orderDate: string;
};

export type AdminDashboardProductAlert = {
  id: string;
  name: string;
  slug: string;
  isAvailable: boolean;
  isFeatured: boolean;
  issue: string;
};

export type AdminDashboard = {
  orderCount: number;
  pendingOrderCount: number;
  inProductionOrderCount: number;
  completedOrderCount: number;
  paidRevenue: number;
  pendingRevenue: number;
  customerCount: number;
  guestOrderCount: number;
  availableProductCount: number;
  unavailableProductCount: number;
  returnRequestCount: number;
  openReturnRequestCount: number;
  ordersByStatus: AdminDashboardStatusCount[];
  paymentsByStatus: AdminDashboardStatusCount[];
  recentOrders: AdminDashboardRecentOrder[];
  productAlerts: AdminDashboardProductAlert[];
};

export type AdminReportRange = {
  dateFrom: string;
  dateTo: string;
};

export type AdminCustomerSummary = {
  id: string;
  applicationUserId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  isActive: boolean;
  createdAt: string;
  measurementProfileCount: number;
  orderCount: number;
  totalSpent: number;
  latestOrderDate: string | null;
};

export type AdminCustomerOrderSummary = {
  id: string;
  orderNumber: string;
  orderDate: string;
  total: number;
  paymentStatus: string;
  orderStatus: string;
  itemCount: number;
};

export type AdminCustomerMeasurementValue = {
  measurementName: string;
  measurementKey: string;
  valueCm: number;
  unit: string;
};

export type AdminCustomerMeasurementProfile = {
  id: string;
  name: string;
  isDefault: boolean;
  notes: string | null;
  measurementCount: number;
  measurements: AdminCustomerMeasurementValue[];
};

export type AdminCustomerDetails = AdminCustomerSummary & {
  orders: AdminCustomerOrderSummary[];
  measurementProfiles: AdminCustomerMeasurementProfile[];
};
