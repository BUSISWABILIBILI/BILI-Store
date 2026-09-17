import type { CartQuoteRequest, CartQuoteResponse } from "./cart";

export type CheckoutCustomerDetails = {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
};

export type CheckoutDeliveryDetails = {
  recipientPhone: string;
  paxiStoreCode: string;
  collectionPointName: string;
  townCity: string;
  province: string;
  instructions: string;
};

export type CheckoutReviewRequest = {
  customer: CheckoutCustomerDetails;
  delivery: CheckoutDeliveryDetails;
  returnPolicyAccepted: boolean;
  cart: CartQuoteRequest;
};

export type CheckoutValidationIssue = {
  field: string;
  message: string;
};

export type CheckoutCustomerReview = CheckoutCustomerDetails;

export type CheckoutDeliveryReview = CheckoutDeliveryDetails & {
  deliveryMethod: string;
};

export type CheckoutReviewResponse = {
  canContinue: boolean;
  issues: CheckoutValidationIssue[];
  quote: CartQuoteResponse;
  customer: CheckoutCustomerReview | null;
  delivery: CheckoutDeliveryReview | null;
  returnPolicyAcceptedAt: string | null;
};
