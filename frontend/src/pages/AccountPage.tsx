import { useEffect, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { LogOut, Ruler, Save, UserRound } from "lucide-react";
import {
  createCustomerMeasurementProfile,
  getCurrentCustomer,
  getCustomerMeasurementProfiles,
  getCustomerOrders,
  loginCustomer,
  registerCustomer,
  requestPasswordReset,
  updateCurrentCustomer,
} from "../services/authApi";
import { useAuthStore } from "../stores/authStore";
import type {
  LoginCustomerRequest,
  RegisterCustomerRequest,
  UpdateCustomerProfileRequest,
} from "../types/auth";

const emptyRegisterForm: RegisterCustomerRequest = {
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  password: "",
  confirmPassword: "",
};

const emptyLoginForm: LoginCustomerRequest = {
  email: "",
  password: "",
};

export function AccountPage() {
  const queryClient = useQueryClient();
  const accessToken = useAuthStore((state) => state.accessToken);
  const customer = useAuthStore((state) => state.customer);
  const setSession = useAuthStore((state) => state.setSession);
  const setCustomer = useAuthStore((state) => state.setCustomer);
  const clearSession = useAuthStore((state) => state.clearSession);
  const [registerForm, setRegisterForm] = useState(emptyRegisterForm);
  const [loginForm, setLoginForm] = useState(emptyLoginForm);
  const [resetEmail, setResetEmail] = useState("");
  const [profileForm, setProfileForm] = useState<UpdateCustomerProfileRequest>({
    firstName: customer?.firstName ?? "",
    lastName: customer?.lastName ?? "",
    phone: customer?.phone ?? "",
  });
  const [measurementProfileName, setMeasurementProfileName] = useState("My Measurements");
  const [measurementValues, setMeasurementValues] = useState({
    bust_chest: "",
    waist: "",
    hips: "",
  });
  const [accountMessage, setAccountMessage] = useState("");

  const profileQuery = useQuery({
    queryKey: ["account", "me"],
    queryFn: getCurrentCustomer,
    enabled: Boolean(accessToken),
  });

  const measurementProfilesQuery = useQuery({
    queryKey: ["account", "measurement-profiles"],
    queryFn: getCustomerMeasurementProfiles,
    enabled: Boolean(accessToken),
  });

  const ordersQuery = useQuery({
    queryKey: ["account", "orders"],
    queryFn: getCustomerOrders,
    enabled: Boolean(accessToken),
  });

  useEffect(() => {
    if (profileQuery.data) {
      setCustomer(profileQuery.data);
      setProfileForm({
        firstName: profileQuery.data.firstName,
        lastName: profileQuery.data.lastName,
        phone: profileQuery.data.phone,
      });
    }
  }, [profileQuery.data, setCustomer]);

  const registerMutation = useMutation({
    mutationFn: registerCustomer,
    onSuccess: (session) => {
      setSession(session);
      setRegisterForm(emptyRegisterForm);
      setAccountMessage("Account created and logged in.");
    },
  });

  const loginMutation = useMutation({
    mutationFn: loginCustomer,
    onSuccess: (session) => {
      setSession(session);
      setLoginForm(emptyLoginForm);
      setAccountMessage("Logged in successfully.");
    },
  });

  const forgotPasswordMutation = useMutation({
    mutationFn: requestPasswordReset,
    onSuccess: (response) => {
      setAccountMessage(response.message);
    },
  });

  const updateProfileMutation = useMutation({
    mutationFn: updateCurrentCustomer,
    onSuccess: (updatedCustomer) => {
      setCustomer(updatedCustomer);
      setAccountMessage("Profile updated.");
    },
  });

  const createMeasurementProfileMutation = useMutation({
    mutationFn: createCustomerMeasurementProfile,
    onSuccess: async () => {
      setMeasurementValues({ bust_chest: "", waist: "", hips: "" });
      setAccountMessage("Measurement profile saved to your account.");
      await queryClient.invalidateQueries({ queryKey: ["account", "measurement-profiles"] });
    },
  });

  function logout() {
    clearSession();
    setAccountMessage("Logged out.");
    queryClient.removeQueries({ queryKey: ["account"] });
  }

  if (!accessToken) {
    return (
      <div className="account-page">
        <section className="shop-hero">
          <p className="eyebrow">Customer accounts</p>
          <h1>Create or access your BILI Store account</h1>
          <p>
            Customer accounts keep profile details and saved measurements ready for future custom
            clothing orders.
          </p>
        </section>

        {accountMessage ? <p className="profile-note">{accountMessage}</p> : null}

        <div className="account-grid">
          <form
            className="account-card"
            onSubmit={(event) => {
              event.preventDefault();
              registerMutation.mutate(registerForm);
            }}
          >
            <UserRound aria-hidden="true" size={26} />
            <h2>Register</h2>
            <AccountInput label="First name" value={registerForm.firstName} onChange={(firstName) => setRegisterForm((current) => ({ ...current, firstName }))} />
            <AccountInput label="Last name" value={registerForm.lastName} onChange={(lastName) => setRegisterForm((current) => ({ ...current, lastName }))} />
            <AccountInput label="Email" type="email" value={registerForm.email} onChange={(email) => setRegisterForm((current) => ({ ...current, email }))} />
            <AccountInput label="Phone" value={registerForm.phone} onChange={(phone) => setRegisterForm((current) => ({ ...current, phone }))} />
            <AccountInput label="Password" type="password" value={registerForm.password} onChange={(password) => setRegisterForm((current) => ({ ...current, password }))} />
            <AccountInput label="Confirm password" type="password" value={registerForm.confirmPassword} onChange={(confirmPassword) => setRegisterForm((current) => ({ ...current, confirmPassword }))} />
            <button className="button button-primary" type="submit">
              Create account
            </button>
            {registerMutation.isError ? <p className="inline-alert">Registration failed. Check the details and try again.</p> : null}
          </form>

          <form
            className="account-card"
            onSubmit={(event) => {
              event.preventDefault();
              loginMutation.mutate(loginForm);
            }}
          >
            <UserRound aria-hidden="true" size={26} />
            <h2>Login</h2>
            <AccountInput label="Email" type="email" value={loginForm.email} onChange={(email) => setLoginForm((current) => ({ ...current, email }))} />
            <AccountInput label="Password" type="password" value={loginForm.password} onChange={(password) => setLoginForm((current) => ({ ...current, password }))} />
            <button className="button button-primary" type="submit">
              Login
            </button>
            {loginMutation.isError ? <p className="inline-alert">Login failed. Check your email and password.</p> : null}
          </form>

          <form
            className="account-card"
            onSubmit={(event) => {
              event.preventDefault();
              forgotPasswordMutation.mutate({ email: resetEmail });
            }}
          >
            <UserRound aria-hidden="true" size={26} />
            <h2>Forgot password</h2>
            <p>
              Enter your account email to start the password reset process when reset emails are
              enabled.
            </p>
            <AccountInput label="Email" type="email" value={resetEmail} onChange={setResetEmail} />
            <button className="button button-light" type="submit">
              Request reset
            </button>
            {forgotPasswordMutation.isError ? <p className="inline-alert">Could not request a reset right now.</p> : null}
          </form>
        </div>
      </div>
    );
  }

  return (
    <div className="account-page">
      <section className="account-hero">
        <div>
          <p className="eyebrow">Customer account</p>
          <h1>{customer ? `${customer.firstName}'s account` : "Account"}</h1>
          <p>
            Manage customer details and account-backed measurement profiles for future custom
            orders.
          </p>
        </div>
        <button className="button button-light" onClick={logout} type="button">
          <LogOut aria-hidden="true" size={18} />
          Logout
        </button>
      </section>

      {accountMessage ? <p className="profile-note">{accountMessage}</p> : null}

      <div className="account-grid">
        <form
          className="account-card"
          onSubmit={(event) => {
            event.preventDefault();
            updateProfileMutation.mutate(profileForm);
          }}
        >
          <UserRound aria-hidden="true" size={26} />
          <h2>Profile details</h2>
          <p>{profileQuery.data?.email ?? customer?.email}</p>
          <AccountInput label="First name" value={profileForm.firstName} onChange={(firstName) => setProfileForm((current) => ({ ...current, firstName }))} />
          <AccountInput label="Last name" value={profileForm.lastName} onChange={(lastName) => setProfileForm((current) => ({ ...current, lastName }))} />
          <AccountInput label="Phone" value={profileForm.phone} onChange={(phone) => setProfileForm((current) => ({ ...current, phone }))} />
          <button className="button button-primary" type="submit">
            <Save aria-hidden="true" size={18} />
            Save profile
          </button>
          {updateProfileMutation.isError ? <p className="inline-alert">Could not update your profile right now.</p> : null}
        </form>

        <form
          className="account-card"
          onSubmit={(event) => {
            event.preventDefault();
            createMeasurementProfileMutation.mutate({
              name: measurementProfileName,
              isDefault: true,
              notes: "Saved from the account page.",
              measurements: Object.entries(measurementValues)
                .filter(([, value]) => Number(value) > 0)
                .map(([measurementKey, value]) => ({
                  measurementKey,
                  valueCm: Number(value),
                })),
            });
          }}
        >
          <Ruler aria-hidden="true" size={26} />
          <h2>Save measurements</h2>
          <AccountInput label="Profile name" value={measurementProfileName} onChange={setMeasurementProfileName} />
          <AccountInput label="Bust / Chest cm" type="number" value={measurementValues.bust_chest} onChange={(value) => setMeasurementValues((current) => ({ ...current, bust_chest: value }))} />
          <AccountInput label="Waist cm" type="number" value={measurementValues.waist} onChange={(value) => setMeasurementValues((current) => ({ ...current, waist: value }))} />
          <AccountInput label="Hips cm" type="number" value={measurementValues.hips} onChange={(value) => setMeasurementValues((current) => ({ ...current, hips: value }))} />
          <button className="button button-primary" type="submit">
            Save account measurements
          </button>
          {createMeasurementProfileMutation.isError ? <p className="inline-alert">Could not save those measurements.</p> : null}
        </form>
      </div>

      <section className="account-card">
        <Ruler aria-hidden="true" size={26} />
        <h2>Saved measurement profiles</h2>
        {measurementProfilesQuery.isLoading ? <p>Loading saved measurements...</p> : null}
        {measurementProfilesQuery.data?.length ? (
          <div className="account-measurement-list">
            {measurementProfilesQuery.data.map((profile) => (
              <article key={profile.id}>
                <div>
                  <h3>{profile.name}</h3>
                  {profile.isDefault ? <span>Default</span> : null}
                </div>
                {profile.notes ? <p>{profile.notes}</p> : null}
                <div className="cart-measurements">
                  {profile.measurements.map((measurement) => (
                    <span key={measurement.measurementKey}>
                      {measurement.measurementName}: {measurement.valueCm} {measurement.unit}
                    </span>
                  ))}
                </div>
              </article>
            ))}
          </div>
        ) : (
          <p>No account measurement profiles saved yet.</p>
        )}
      </section>

      <section className="account-card">
        <UserRound aria-hidden="true" size={26} />
        <h2>Order history</h2>
        {ordersQuery.isLoading ? <p>Loading orders...</p> : null}
        {ordersQuery.data?.length ? (
          <div className="account-measurement-list">
            {ordersQuery.data.map((order) => (
              <article key={order.id}>
                <div>
                  <h3>{order.orderNumber}</h3>
                  <span>{order.orderStatus}</span>
                </div>
                <p>
                  {order.itemCount} item(s) / {order.paymentStatus} /{" "}
                  {new Date(order.orderDate).toLocaleDateString()}
                </p>
              </article>
            ))}
          </div>
        ) : (
          <p>Your order history will appear here after checkout and order creation are built.</p>
        )}
      </section>
    </div>
  );
}

type AccountInputProps = {
  label: string;
  onChange: (value: string) => void;
  type?: string;
  value: string;
};

function AccountInput({ label, onChange, type = "text", value }: AccountInputProps) {
  return (
    <label className="account-field">
      <span>{label}</span>
      <input onChange={(event) => onChange(event.target.value)} type={type} value={value} />
    </label>
  );
}
