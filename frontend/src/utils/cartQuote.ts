import type { CartItem, CartQuoteRequest } from "../types/cart";

export function buildCartQuoteRequest(items: CartItem[]): CartQuoteRequest {
  return {
    items: items.map((item) => ({
      productSlug: item.productSlug,
      selectedColourId: item.selectedColour?.id ?? null,
      customColourName: item.customColourName,
      customColourHex: item.customColourHex,
      quantity: item.quantity,
      measurements: item.measurements.map((measurement) => ({
        measurementKey: measurement.measurementKey,
        measurementName: measurement.measurementName,
        valueCm: measurement.valueCm,
        unit: measurement.unit,
      })),
    })),
  };
}
