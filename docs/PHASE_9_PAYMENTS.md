# Phase 9 - Payment Foundation

## What This Phase Added

Phase 9 adds the payment foundation.

It adds:

- Payment service abstraction.
- Development payment provider.
- Payment session creation endpoint.
- Payment reference generation.
- Pending payment status.
- Frontend payment step after checkout review.
- Documentation for real provider configuration points.

This phase does not charge real bank cards. It creates a safe development payment session that proves the application has the correct payment flow shape before connecting PayFast, Peach Payments, or Yoco.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Payments/`

Contains:

- `CreatePaymentSessionRequestDto.cs`
- `PaymentSessionDto.cs`
- `CreatePaymentSessionResponseDto.cs`

These DTOs define the request and response for starting a payment session.

`backend/src/BiliStore.Application/Payments/IPaymentSessionService.cs`

Defines the payment service contract.

The important idea is that controllers depend on this interface, not directly on PayFast, Peach Payments, Yoco, or any specific provider.

`backend/src/BiliStore.Infrastructure/Payments/DevelopmentPaymentSessionService.cs`

Adds the development payment provider.

It:

- Revalidates checkout data through `ICheckoutReviewService`.
- Refuses to start payment if checkout review fails.
- Generates a development payment reference.
- Returns `Pending` payment status.
- Returns a test payment URL based on the frontend checkout URL.
- Sets a 30-minute expiry timestamp.

`backend/src/BiliStore.Api/Controllers/PaymentsController.cs`

Adds:

```text
POST /api/payments/create-session
```

## Backend Files Updated

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

```text
IPaymentSessionService -> DevelopmentPaymentSessionService
```

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 9 - Payment foundation
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds a manual API example for:

```text
POST /api/payments/create-session
```

## Payment Flow

1. Customer completes checkout details.
2. Customer clicks **Review checkout**.
3. Backend validates checkout and cart totals.
4. Customer opens the Payment step.
5. Customer clicks **Start test payment**.
6. Backend validates checkout again.
7. Backend returns a development payment session.
8. Frontend displays the provider reference, amount, and pending status.

The second backend validation is important. It prevents payment from starting if the checkout data changed after review.

## Frontend Files Added

`frontend/src/types/payments.ts`

Defines:

- Payment session request.
- Payment session response.
- Payment session details.

`frontend/src/services/paymentsApi.ts`

Adds:

```text
createPaymentSession()
```

This posts to:

```text
/api/payments/create-session
```

## Frontend Files Updated

`frontend/src/pages/CheckoutPage.tsx`

Adds a fifth checkout step:

```text
Payment
```

The Payment step:

- Requires checkout review to pass first.
- Starts a development payment session.
- Displays payment provider name.
- Displays payment reference.
- Displays payment status.
- Displays payment amount.
- Shows a test payment link when one is returned.

`frontend/src/styles/global.css`

Adds payment-session card styling and updates the checkout step grid to support five steps.

## How To Explain The Payment Code

The payment code is split into an interface and a provider.

The interface is:

```text
IPaymentSessionService
```

The current provider is:

```text
DevelopmentPaymentSessionService
```

That means the API can ask for a payment session without knowing which provider creates it. Later, a real provider can replace the development provider while the controller and frontend API call stay mostly the same.

The development provider does not take money. It returns a pending payment reference that proves the checkout data is ready for payment.

## Real Provider Configuration Points

Payment provider secrets must stay out of source control.

Recommended configuration keys:

PayFast:

```text
Payments:PayFast:MerchantId
Payments:PayFast:MerchantKey
Payments:PayFast:Passphrase
Payments:PayFast:ReturnUrl
Payments:PayFast:CancelUrl
Payments:PayFast:NotifyUrl
```

Peach Payments:

```text
Payments:PeachPayments:EntityId
Payments:PeachPayments:AccessToken
Payments:PeachPayments:WebhookSecret
Payments:PeachPayments:ReturnUrl
Payments:PeachPayments:CancelUrl
```

Yoco:

```text
Payments:Yoco:SecretKey
Payments:Yoco:WebhookSecret
Payments:Yoco:ReturnUrl
Payments:Yoco:CancelUrl
```

For local development, these can go into user secrets or environment variables. For production, they should be configured in the hosting provider's secret store.

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
2. Go to Checkout.
3. Complete Customer, Delivery, and Policy steps.
4. Go to Review.
5. Click **Review checkout**.
6. Go to Payment.
7. Click **Start test payment**.
8. Confirm a development payment reference appears.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
POST /api/payments/create-session
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

Passed. This means the payment API service, payment types, checkout payment step, and styling references compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## What Comes Next

Phase 10 should implement order creation:

- Create orders from validated checkout data.
- Snapshot products, colours, measurements, delivery details, and payment references.
- Add order confirmation.
- Connect account order history to real orders.
