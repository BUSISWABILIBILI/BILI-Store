import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { AddCartItemInput, CartItem } from "../types/cart";

type CartStore = {
  items: CartItem[];
  addItem: (item: AddCartItemInput) => void;
  clearCart: () => void;
  removeItem: (id: string) => void;
  updateQuantity: (id: string, quantity: number) => void;
};

export const useCartStore = create<CartStore>()(
  persist(
    (set) => ({
      items: [],
      addItem: (item) =>
        set((state) => ({
          items: [
            ...state.items,
            {
              ...item,
              id: crypto.randomUUID(),
              addedAt: new Date().toISOString(),
            },
          ],
        })),
      clearCart: () => set({ items: [] }),
      removeItem: (id) =>
        set((state) => ({
          items: state.items.filter((item) => item.id !== id),
        })),
      updateQuantity: (id, quantity) =>
        set((state) => ({
          items: state.items.map((item) =>
            item.id === id ? { ...item, quantity: Math.max(1, Math.min(20, quantity)) } : item,
          ),
        })),
    }),
    {
      name: "bili-store.cart",
      partialize: (state) => ({ items: state.items }),
    },
  ),
);
