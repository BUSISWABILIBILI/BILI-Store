# Phase 13 - Admin Dashboard and Customer Support

## What This Phase Added

Phase 13 adds an admin dashboard and customer lookup support.

It adds:

- Admin dashboard summary endpoint.
- Admin customer list endpoint.
- Admin customer details endpoint.
- Store activity summary cards.
- Recent order overview.
- Order and payment status counts.
- Catalogue alert overview.
- Registered customer lookup page.
- Customer order and saved measurement review.

This phase gives the store owner a quicker way to understand what is happening in the shop before opening individual order or catalogue records.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Admin/`

Adds:

- `AdminDashboardDto.cs`
- `AdminDashboardProductAlertDto.cs`
- `AdminDashboardRecentOrderDto.cs`
- `AdminDashboardStatusCountDto.cs`
- `AdminCustomerSummaryDto.cs`
- `AdminCustomerDetailsDto.cs`
- `AdminCustomerOrderSummaryDto.cs`
- `AdminCustomerMeasurementProfileDto.cs`
- `AdminCustomerMeasurementValueDto.cs`

These DTOs keep dashboard/customer API data separate from database entities.

`backend/src/BiliStore.Api/Controllers/AdminDashboardController.cs`

Adds:

```text
GET /api/admin/dashboard
```

This endpoint returns:

- Order counts.
- Paid and pending revenue.
- Customer count.
- Guest order count.
- Product availability counts.
- Return request counts.
- Recent orders.
- Catalogue alerts.

`backend/src/BiliStore.Api/Controllers/AdminCustomersController.cs`

Adds:

```text
GET /api/admin/customers
GET /api/admin/customers/{id}
```

These endpoints return registered customer profile data, order summaries, total spend, latest order date, and saved measurement profiles.

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 13 - Admin dashboard and customer support
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual examples for:

```text
GET /api/admin/dashboard
GET /api/admin/customers
GET /api/admin/customers/{id}
```

## Backend Flow

The dashboard endpoint is read-only.

It calculates store totals from existing data:

1. Counts orders by status.
2. Adds paid revenue from paid orders.
3. Adds pending revenue from pending orders.
4. Counts registered customer profiles.
5. Counts guest orders.
6. Counts available and unavailable products.
7. Counts open return requests.
8. Returns recent orders and catalogue alerts.

The customer endpoints join `CustomerProfile` records with ASP.NET Identity users so admin screens can show account email, activity, order count, and saved measurements.

## Frontend Files Added

`frontend/src/pages/AdminDashboardPage.tsx`

Adds the admin landing page at:

```text
/admin
```

The page shows:

- Store metric cards.
- Admin quick links.
- Recent order list.
- Catalogue alerts.
- Order status counts.
- Payment status counts.

`frontend/src/pages/AdminCustomersPage.tsx`

Adds the customer lookup page at:

```text
/admin/customers
```

The page shows:

- Searchable customer list.
- Customer account details.
- Customer order summaries.
- Customer saved measurement profiles.

## Frontend Files Updated

`frontend/src/types/admin.ts`

Adds dashboard and customer types.

`frontend/src/services/adminApi.ts`

Adds API calls for:

- `getAdminDashboard()`
- `getAdminCustomers()`
- `getAdminCustomer()`

`frontend/src/app/routes.ts`

Adds:

```text
/admin
/admin/customers
```

`frontend/src/components/layout/AppLayout.tsx`

Changes the top admin navigation to one `Admin` link that opens the dashboard. The dashboard then links to orders, catalogue, and customers.

`frontend/src/styles/global.css`

Adds dashboard and customer management styling.

## How To Explain The Dashboard Code

The dashboard is not a separate database table.

It is an API endpoint that summarizes existing tables:

- `Orders`
- `Products`
- `CustomerProfiles`
- `ReturnRequests`

This means dashboard numbers update naturally as orders, products, customers, and returns change.

## How To Explain The Customer Lookup Code

Customer accounts are stored in two places:

- ASP.NET Identity stores login details like email, password hash, and active status.
- `CustomerProfile` stores business profile details like first name, last name, and preferred phone.

The admin customer endpoint joins those two pieces together, then adds order totals and measurement profile data.

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

Manual browser flow:

1. Assign the `Admin` role to a test user.
2. Log in with that user.
3. Open `/admin`.
4. Confirm dashboard metrics load.
5. Open `/admin/customers`.
6. Select a customer.
7. Confirm orders and measurement profiles display.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
GET /api/admin/dashboard
GET /api/admin/customers
GET /api/admin/customers/{id}
```

These requests require an access token for a user with the `Admin` role.

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

Passed. This means the admin dashboard page, customer page, admin API service, routes, and types compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## Admin Role Setup Note

The project still does not expose public admin registration.

That is intentional. Admin accounts should be assigned through a trusted setup process, database seed, or future protected user-management tool. After assigning the role, the user must log in again so the JWT token includes the `Admin` role claim.

## What Comes Next

Phase 14 should add stronger admin setup and operations polish:

- Admin role seed/setup script.
- Dashboard date filtering.
- Export-friendly order/customer reports.
- Delivery status management.
- Return request creation from customer order pages.
