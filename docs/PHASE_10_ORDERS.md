# Phase 10 - Order Creation and Confirmation

## What This Phase Added

Phase 10 creates real orders from validated checkout data.

It adds:

- Order creation endpoint.
- Order lookup endpoint.
- Order service abstraction.
- Order persistence using the existing order tables.
- Product, colour, measurement, delivery, and payment snapshots.
- Frontend order creation after payment session.
- Order confirmation page.
- Account order-history links to saved orders.

This phase connects the customer journey from cart to checkout, payment session, saved order, and confirmation.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Orders/`

Contains:

- `CreateOrderPaymentDto.cs`
- `CreateOrderRequestDto.cs`
- `CreateOrderResponseDto.cs`
- `OrderConfirmationDto.cs`
- `OrderCustomerDto.cs`
- `OrderDeliveryDto.cs`
- `OrderItemDto.cs`
- `OrderItemMeasurementDto.cs`
- `OrderPaymentDto.cs`

These DTOs keep order API data separate from Entity Framework database entities.

`backend/src/BiliStore.Application/Orders/IOrderService.cs`

Defines:

```text
CreateAsync()
GetByOrderNumberAsync()
```

`backend/src/BiliStore.Infrastructure/Orders/OrderService.cs`

Creates and retrieves orders.

It:

- Revalidates checkout through `ICheckoutReviewService`.
- Checks that a payment provider and payment reference exist.
- Checks that payment amount and currency match the checkout total.
- Links the order to the logged-in customer when a JWT is present.
- Saves guest email when the customer is not logged in.
- Generates a unique order number.
- Saves PAXI/PEP delivery details.
- Saves pending payment details.
- Saves product, colour, measurement, price, quantity, and production snapshots.

`backend/src/BiliStore.Api/Controllers/OrdersController.cs`

Adds:

```text
POST /api/orders
GET  /api/orders/{orderNumber}
```

## Backend Files Updated

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

```text
IOrderService -> OrderService
```

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 10 - Order creation and confirmation
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual examples for:

```text
POST /api/orders
GET  /api/orders/{orderNumber}
```

## Order Creation Flow

1. Customer adds measured custom garments to cart.
2. Customer completes checkout details.
3. Customer reviews checkout.
4. Customer starts a development payment session.
5. Customer creates the order.
6. Backend revalidates checkout.
7. Backend checks the payment reference.
8. Backend saves the order and its snapshots.
9. Frontend clears the cart.
10. Frontend opens the order confirmation page.

The backend does not trust the browser blindly. It revalidates checkout and payment data before saving.

## Why Order Snapshots Matter

Orders store snapshots because catalogue data can change later.

For example, if a product price or description changes next month, an old order must still show the product name, price, measurements, and colour that were accepted at checkout time.

Phase 10 snapshots:

- Product name.
- Product description.
- Unit price.
- Quantity.
- Selected colour.
- Custom colour.
- Measurements.
- Production estimate.
- Delivery details.
- Payment reference.

## Frontend Files Added

`frontend/src/types/orders.ts`

Defines:

- Order creation request.
- Order creation response.
- Order confirmation data.
- Order item snapshots.
- Order delivery details.
- Order payment details.

`frontend/src/services/ordersApi.ts`

Adds:

```text
createOrder()
getOrder()
```

`frontend/src/pages/OrderConfirmationPage.tsx`

Displays:

- Order number.
- Totals.
- Customer details.
- PAXI/PEP delivery details.
- Payment reference.
- Garment snapshots.
- Measurements.

## Frontend Files Updated

`frontend/src/pages/CheckoutPage.tsx`

Adds final order creation after the payment session.

The flow is now:

```text
Customer -> Delivery -> Policy -> Review -> Payment -> Create order
```

When the order is created successfully:

- The cart is cleared.
- The customer is sent to `/orders/{orderNumber}`.

`frontend/src/app/routes.ts`

Adds:

```text
/orders/:orderNumber
```

`frontend/src/pages/AccountPage.tsx`

Adds a **View order** link in account order history.

`frontend/src/styles/global.css`

Adds order confirmation styling.

## How To Explain The Order Code

The order service is the part that turns a temporary checkout into a permanent order.

Checkout review says:

```text
Is the cart, customer, delivery, and policy data valid?
```

Payment session says:

```text
Is there a payment reference for the total?
```

Order creation says:

```text
Save all accepted details permanently so the customer and store can track the order.
```

The order starts with:

```text
PaymentStatus.Pending
OrderStatus.PendingPayment
```

That is correct because the development payment provider does not confirm real money yet.

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

1. Add a product to the cart.
2. Open Checkout.
3. Complete Customer, Delivery, and Policy.
4. Open Review.
5. Click **Review checkout**.
6. Open Payment.
7. Click **Start test payment**.
8. Click **Create order**.
9. Confirm the order confirmation page opens.
10. If logged in, open Account and confirm the order appears in order history.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
POST /api/orders
GET  /api/orders/{orderNumber}
```

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

Passed. This means the order API service, order types, checkout order creation, account order links, and order confirmation page compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## What Comes Next

Phase 11 should implement admin order management:

- Admin order list.
- Admin order detail.
- Order status updates.
- Payment status review.
- Return request review foundation.
