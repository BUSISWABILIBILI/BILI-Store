# Phase 14 - Delivery and Return Operations

## What This Phase Added

Phase 14 improves the order operations workflow.

It adds:

- Admin delivery status updates.
- Delivery status syncing to major order statuses.
- Customer return request creation.
- Customer return request history.
- Account-page return request form.
- Admin order page delivery status control.
- Manual API examples for delivery and return operations.

This phase connects the customer return-policy promise to an actual workflow in the application.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Admin/UpdateAdminDeliveryStatusRequestDto.cs`

Defines the admin request body for changing an order delivery status.

`backend/src/BiliStore.Application/DTOs/Customers/CreateReturnRequestDto.cs`

Defines the customer request body for submitting a return request.

`backend/src/BiliStore.Application/DTOs/Customers/CustomerReturnRequestDto.cs`

Defines the return request data shown back to the customer.

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/AdminController.cs`

Adds:

```text
PUT /api/admin/orders/{orderNumber}/delivery-status
```

The endpoint:

1. Checks that the user has the `Admin` role.
2. Validates the delivery status enum.
3. Finds the order and its delivery details.
4. Updates the delivery status.
5. Syncs major delivery milestones to order status.
6. Returns the updated order details.

Delivery status sync rules:

```text
SentViaPaxi          -> OrderStatus.SentViaPaxi
ReadyForCollection  -> OrderStatus.ReadyForCollection
Collected           -> OrderStatus.Completed
```

`backend/src/BiliStore.Api/Controllers/CustomerOrdersController.cs`

Adds:

```text
GET  /api/customers/me/orders/return-requests
POST /api/customers/me/orders/{orderNumber}/return-requests
```

These routes are customer-protected. A customer can only submit and view return requests for orders attached to their own customer profile.

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 14 - Delivery and return operations
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual examples for:

```text
PUT  /api/admin/orders/{orderNumber}/delivery-status
GET  /api/customers/me/orders/return-requests
POST /api/customers/me/orders/{orderNumber}/return-requests
```

## Frontend Files Updated

`frontend/src/types/admin.ts`

Adds the admin delivery status update request type.

`frontend/src/services/adminApi.ts`

Adds:

```text
updateAdminDeliveryStatus()
```

`frontend/src/pages/AdminOrdersPage.tsx`

Adds a delivery status selector and separate delivery save button.

The admin can now update:

- Order status.
- Payment status.
- Delivery status.
- Return request status.

`frontend/src/types/auth.ts`

Adds customer return request types.

`frontend/src/services/authApi.ts`

Adds:

```text
getCustomerReturnRequests()
createCustomerReturnRequest()
```

`frontend/src/pages/AccountPage.tsx`

Adds a customer return request section.

Customers can:

1. Select one of their own orders.
2. Enter a return reason.
3. Enter return details.
4. Submit the return request.
5. See submitted return requests and current statuses.

`frontend/src/styles/global.css`

Adds styling for account return forms and improves admin status form layout.

## How To Explain The Delivery Code

Delivery status belongs to `DeliveryDetails` because it tracks parcel movement.

Order status belongs to `Order` because it tracks the overall business workflow.

Some delivery changes are important enough to update the order status too. For example, when delivery becomes `Collected`, the whole order can move to `Completed`.

## How To Explain The Return Code

The return request starts with the customer.

A customer submits a reason and details from their account page. The backend checks that the order belongs to the logged-in customer before saving the request.

Admins then review return requests from the admin order page and move the request through statuses such as:

- `Submitted`
- `UnderReview`
- `ApprovedForRemake`
- `ApprovedForRefund`
- `Rejected`
- `Closed`

This keeps customer submission and admin decision-making separate.

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

1. Log in as a customer with an order.
2. Open `/account`.
3. Submit a return request for an order.
4. Log in as an admin.
5. Open `/admin/orders`.
6. Review the return request.
7. Update return request status.
8. Update delivery status on the selected order.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
POST /api/customers/me/orders/{orderNumber}/return-requests
GET  /api/customers/me/orders/return-requests
PUT  /api/admin/orders/{orderNumber}/delivery-status
```

Customer return routes require a logged-in customer token. Admin delivery routes require a token with the `Admin` role.

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

Passed. This means the admin delivery status control, customer return request form, API services, and types compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## What Comes Next

Phase 15 should add reporting and export polish:

- Admin report filters.
- Order CSV export.
- Customer CSV export.
- Date range filtering for dashboard summaries.
- Cleaner role setup notes or an admin setup script.
