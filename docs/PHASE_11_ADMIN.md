# Phase 11 - Admin Order Management

## What This Phase Added

Phase 11 adds the admin order management foundation.

It adds:

- Admin-protected backend routes.
- JWT role claims.
- Admin order queue.
- Admin order detail lookup.
- Order status update.
- Payment status update.
- Payment reference review.
- Return request review foundation.
- Frontend admin orders page.

The admin area is intentionally separate from the customer account area.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Admin/`

Contains:

- `AdminOrderSummaryDto.cs`
- `AdminPaymentSummaryDto.cs`
- `AdminReturnRequestSummaryDto.cs`
- `UpdateAdminOrderStatusRequestDto.cs`
- `UpdateReturnRequestStatusRequestDto.cs`

`backend/src/BiliStore.Api/Controllers/AdminController.cs`

Adds admin-only endpoints:

```text
GET /api/admin/orders
GET /api/admin/orders/{orderNumber}
PUT /api/admin/orders/{orderNumber}/status
GET /api/admin/payments
GET /api/admin/return-requests
PUT /api/admin/return-requests/{id}/status
```

These routes use:

```text
[Authorize(Roles = "Admin")]
```

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/AuthController.cs`

Login and register responses now include roles.

JWT tokens now include role claims:

```text
ClaimTypes.Role
```

This allows admin-only endpoints to authorize correctly when a user is assigned the `Admin` role.

`backend/src/BiliStore.Application/DTOs/Auth/AuthResponseDto.cs`

Adds:

```text
Roles
```

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 11 - Admin order management
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual examples for admin routes.

## Frontend Files Added

`frontend/src/types/admin.ts`

Defines frontend types for:

- Admin order summaries.
- Admin payment summaries.
- Admin return request summaries.
- Admin status update requests.

`frontend/src/services/adminApi.ts`

Adds API calls for:

- Admin order list.
- Admin order details.
- Admin order status update.
- Admin payment list.
- Admin return request list.
- Admin return request status update.

`frontend/src/pages/AdminOrdersPage.tsx`

Adds the admin order management page at:

```text
/admin/orders
```

The page shows:

- Order queue.
- Selected order details.
- Order status editor.
- Payment status editor.
- Garment summary for selected order.
- Payment reference list.
- Return request list and status update foundation.

## Frontend Files Updated

`frontend/src/types/auth.ts`

Adds roles to the auth response.

`frontend/src/stores/authStore.ts`

Persists roles with the auth session.

`frontend/src/app/routes.ts`

Adds:

```text
/admin/orders
```

`frontend/src/components/layout/AppLayout.tsx`

Shows the Admin link only when the logged-in account has the `Admin` role.

`frontend/src/styles/global.css`

Adds admin page styling.

## Admin Role Note

This phase protects admin features but does not create a public admin signup screen.

That is intentional. Admin accounts should not be self-service.

For development, assign the `Admin` role to a trusted test user through a secure setup script, direct database setup, or a future protected admin-user management tool.

After the role is assigned, log in again so the JWT contains the role claim.

## How To Explain The Admin Flow

Customer pages handle shopping and account history.

Admin pages handle store operations:

1. View all saved orders.
2. Select an order.
3. Review customer, delivery, payment, and item details.
4. Move the order through production statuses.
5. Review payment references.
6. Review return request statuses.

The backend still controls the allowed status values by parsing domain enums.

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
3. Open `/admin/orders`.
4. Select an order.
5. Change order or payment status.
6. Save the status.
7. Confirm the order queue updates.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
GET /api/admin/orders
PUT /api/admin/orders/{orderNumber}/status
GET /api/admin/payments
GET /api/admin/return-requests
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

Passed. This means the admin API service, admin types, auth role persistence, admin route, and admin page compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## What Comes Next

Phase 12 should implement admin catalogue management:

- Admin product list.
- Create and edit products.
- Activate or deactivate products.
- Manage categories.
- Manage colours.
- Manage measurement requirements.
