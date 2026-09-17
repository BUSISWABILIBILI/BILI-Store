import type { AcceptedMeasurement } from "./measurements";

export type CartColourSelection = {
  id: string;
  name: string;
  hexCode: string;
};

export type CartItem = {
  id: string;
  productId: string;
  productSlug: string;
  productName: string;
  productDescription: string;
  clothingType: string;
  imageUrl: string;
  imageAltText: string;
  unitPrice: number;
  productionDaysEstimate: number;
  quantity: number;
  selectedColour: CartColourSelection | null;
  customColourName: string | null;
  customColourHex: string | null;
  measurements: AcceptedMeasurement[];
  addedAt: string;
};

export type AddCartItemInput = Omit<CartItem, "id" | "addedAt">;

export type CartMeasurementInput = {
  measurementKey: string;
  measurementName: string;
  valueCm: number;
  unit: string;
};

export type CartQuoteItemRequest = {
  productSlug: string;
  selectedColourId: string | null;
  customColourName: string | null;
  customColourHex: string | null;
  quantity: number;
  measurements: CartMeasurementInput[];
};

export type CartQuoteRequest = {
  items: CartQuoteItemRequest[];
};

export type CartQuoteItemResponse = {
  productSlug: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  selectedColourName: string | null;
  selectedColourHex: string | null;
  customColourName: string | null;
  customColourHex: string | null;
  measurements: CartMeasurementInput[];
};

export type CartQuoteIssue = {
  productSlug: string;
  message: string;
};

export type CartQuoteResponse = {
  items: CartQuoteItemResponse[];
  issues: CartQuoteIssue[];
  subtotal: number;
  deliveryFee: number;
  grandTotal: number;
  currency: string;
};
