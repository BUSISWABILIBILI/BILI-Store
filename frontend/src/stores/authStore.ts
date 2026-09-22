import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { AuthResponse, CustomerProfile } from "../types/auth";

type AuthStore = {
  accessToken: string | null;
  customer: CustomerProfile | null;
  expiresAt: string | null;
  roles: string[];
  clearSession: () => void;
  setCustomer: (customer: CustomerProfile) => void;
  setSession: (session: AuthResponse) => void;
};

export const useAuthStore = create<AuthStore>()(
  persist(
    (set) => ({
      accessToken: null,
      customer: null,
      expiresAt: null,
      roles: [],
      clearSession: () => set({ accessToken: null, customer: null, expiresAt: null, roles: [] }),
      setCustomer: (customer) => set({ customer }),
      setSession: (session) =>
        set({
          accessToken: session.accessToken,
          customer: session.customer,
          expiresAt: session.expiresAt,
          roles: session.roles,
        }),
    }),
    {
      name: "bili-store.auth",
      partialize: (state) => ({
        accessToken: state.accessToken,
        customer: state.customer,
        expiresAt: state.expiresAt,
        roles: state.roles,
      }),
    },
  ),
);
