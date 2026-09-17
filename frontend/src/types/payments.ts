import type { CheckoutReviewRequest, CheckoutReviewResponse, CheckoutValidationIssue } from "./checkout";

export type CreatePaymentSessionRequest = {
  checkout: CheckoutReviewRequest;
  successUrl: string;
  cancelUrl: string;
};

export type PaymentSession = {
  provider: string;
  providerReference: string;
  amount: number;
  currency: string;
  status: string;
  paymentUrl: string | null;
  expiresAt: string;
};

export type CreatePaymentSessionResponse = {
  canStartPayment: boolean;
  issues: CheckoutValidationIssue[];
  checkoutReview: CheckoutReviewResponse;
  payment: PaymentSession | null;
};
