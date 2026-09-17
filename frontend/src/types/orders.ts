import type { CheckoutReviewRequest, CheckoutValidationIssue } from "./checkout";

export type CreateOrderPayment = {
  provider: string;
  providerReference: string;
  amount: number;
  currency: string;
  paymentUrl: string | null;
};

export type CreateOrderRequest = {
  checkout: CheckoutReviewRequest;
  payment: CreateOrderPayment;
};

export type OrderCustomer = {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
};

export type OrderDelivery = {
  deliveryMethod: string;
  recipientPhone: string;
  paxiStoreCode: string;
  collectionPointName: string;
  townCity: string;
  province: string;
  instructions: string | null;
  deliveryStatus: string;
};

export type OrderItemMeasurement = {
  measurementKey: string;
  measurementName: string;
  valueCm: number;
};

export type OrderItem = {
  id: string;
  productName: string;
  productDescription: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  selectedColourName: string | null;
  selectedColourHex: string | null;
  customColourName: string | null;
  customColourHex: string | null;
  productionDaysEstimate: number;
  measurements: OrderItemMeasurement[];
};

export type OrderPayment = {
  provider: string;
  providerReference: string | null;
  amount: number;
  currency: string;
  status: string;
  paymentUrl: string | null;
  createdAt: string;
};

export type OrderConfirmation = {
  id: string;
  orderNumber: string;
  orderDate: string;
  subtotal: number;
  deliveryFee: number;
  total: number;
  paymentStatus: string;
  orderStatus: string;
  returnPolicyAcceptedAt: string | null;
  customer: OrderCustomer;
  delivery: OrderDelivery;
  items: OrderItem[];
  payments: OrderPayment[];
};

export type CreateOrderResponse = {
  canCreateOrder: boolean;
  issues: CheckoutValidationIssue[];
  order: OrderConfirmation | null;
};
