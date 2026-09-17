# Phase 7 - Customer Accounts

## What This Phase Added

Phase 7 adds customer account functionality.

It adds:

- Customer registration.
- Customer login.
- JWT access tokens.
- Authenticated profile lookup.
- Authenticated profile update.
- Forgot-password architecture placeholder.
- Authenticated saved measurement profiles.
- Authenticated order-history foundation.
- Frontend auth store persisted in local storage.
- Account page for register, login, profile editing, saved measurements, and order history.

This phase connects the account system to the existing ASP.NET Core Identity foundation.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Auth/`

Contains:

- `RegisterCustomerRequestDto.cs`
- `LoginCustomerRequestDto.cs`
- `ForgotPasswordRequestDto.cs`
- `UpdateCustomerProfileRequestDto.cs`
- `CustomerProfileDto.cs`
- `AuthResponseDto.cs`

`backend/src/BiliStore.Application/DTOs/Customers/`

Adds account-facing customer DTOs:

- `CustomerMeasurementValueDto.cs`
- `CustomerMeasurementProfileDto.cs`
- `CustomerMeasurementInputDto.cs`
- `CreateMeasurementProfileRequestDto.cs`
- `CustomerOrderSummaryDto.cs`

`backend/src/BiliStore.Api/Controllers/AuthController.cs`

Adds:

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/forgot-password
GET  /api/auth/me
PUT  /api/auth/me
```

`backend/src/BiliStore.Api/Controllers/CustomerMeasurementProfilesController.cs`

Adds authenticated saved measurement profile endpoints:

```text
GET  /api/customers/me/measurement-profiles
POST /api/customers/me/measurement-profiles
```

`backend/src/BiliStore.Api/Controllers/CustomerOrdersController.cs`

Adds order history foundation:

```text
GET /api/customers/me/orders
```

This will return account orders after Phase 10 creates real orders.

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 7 - Customer accounts
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual request examples for:

- Register.
- Login.
- Current customer profile.
- Customer profile update.
- Forgot password.
- Saved measurement profile creation.
- Order history.

## Authentication Flow

Register:

1. Customer submits first name, last name, email, phone, password.
2. ASP.NET Core Identity creates the `ApplicationUser`.
3. A `CustomerProfile` is created.
4. The API returns a JWT token and customer profile.

Login:

1. Customer submits email and password.
2. Identity verifies the password.
3. The API returns a JWT token and customer profile.

Authenticated requests:

1. Frontend stores the JWT in the auth store.
2. Axios adds `Authorization: Bearer <token>`.
3. ASP.NET Core JWT middleware validates the token.
4. Controllers use the user ID claim to load the customer profile.

## Frontend Files Added

`frontend/src/types/auth.ts`

Defines frontend account, auth, saved measurement, and order-history types.

`frontend/src/services/authApi.ts`

Defines API calls for:

- Register.
- Login.
- Forgot password.
- Get current customer.
- Update current customer.
- Get saved measurement profiles.
- Create saved measurement profiles.
- Get customer orders.

`frontend/src/stores/authStore.ts`

Creates a Zustand auth store.

It persists:

- JWT access token.
- Customer profile.
- Token expiry timestamp.

The storage key is:

```text
bili-store.auth
```

## Frontend Files Updated

`frontend/src/services/apiClient.ts`

Adds the JWT token to outgoing API requests.

`frontend/src/pages/AccountPage.tsx`

Replaces the placeholder account page with:

- Register form.
- Login form.
- Forgot password form.
- Customer profile editor.
- Account measurement profile creator.
- Saved measurement profile list.
- Order history foundation section.
- Logout button.

`frontend/src/styles/global.css`

Adds account page styling.

## Forgot Password Note

The forgot-password endpoint is a safe placeholder.

It returns a generic response and does not reveal whether an email exists. Real email delivery and reset-token handling should be added later when an email provider is configured.

This is enough for the architecture stage because the route exists, the UI exists, and the response pattern is safe.

## Saved Measurements Note

Before Phase 7, measurement profiles were browser-local only.

Now authenticated customers can save measurement profiles to the backend:

- Profile name.
- Default profile flag.
- Notes.
- Measurement keys and centimetre values.

The account page currently saves bust/chest, waist, and hips as a simple foundation. Later phases can connect product-detail measurement forms directly to account-backed profiles.

## How To Test

Backend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend"
dotnet build BiliStore.sln --no-restore
dotnet run --project src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Frontend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend"
pnpm dev
```

Manual flow:

1. Open Account.
2. Register a new customer.
3. Confirm the account page changes to the logged-in profile view.
4. Edit profile details.
5. Save a measurement profile.
6. Logout.
7. Login again.
8. Confirm the saved measurement profile loads from the backend.

## Verification Results

Backend:

```text
dotnet build BiliStore.sln --no-restore -v:minimal -maxcpucount:1
```

Passed with 0 warnings and 0 errors.

Frontend:

```text
pnpm lint
```

Passed. This means the auth API service, auth store, account page, and typed account models compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. This is the same environment limitation from previous phases, not a TypeScript error.

## What Comes Next

Phase 8 should implement checkout and PAXI/PEP delivery:

- Multi-step checkout.
- Customer details.
- PAXI/PEP collection point information.
- Return policy acceptance.
- Order review using the cart.
