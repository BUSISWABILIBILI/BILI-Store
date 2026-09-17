# Phase 8 - Checkout and PAXI/PEP Delivery

## What This Phase Added

Phase 8 turns checkout from a placeholder into a real review flow.

It adds:

- Multi-step frontend checkout.
- Customer detail collection.
- PAXI/PEP delivery detail collection.
- Return and refund policy acceptance.
- Backend checkout review endpoint.
- Backend validation for missing checkout data.
- Trusted cart review using the existing backend cart quote logic.

This phase does not create final orders yet. It prepares and validates the checkout data that Phase 10 will use for order creation.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Checkout/`

Contains:

- `CheckoutCustomerDetailsDto.cs`
- `CheckoutDeliveryDetailsDto.cs`
- `CheckoutCustomerReviewDto.cs`
- `CheckoutDeliveryReviewDto.cs`
- `CheckoutValidationIssueDto.cs`
- `CheckoutReviewRequestDto.cs`
- `CheckoutReviewResponseDto.cs`

These DTOs define the data shape for checkout review requests and responses.

`backend/src/BiliStore.Application/Checkout/ICheckoutReviewService.cs`

Defines the checkout review service contract.

`backend/src/BiliStore.Infrastructure/Checkout/CheckoutReviewService.cs`

Validates:

- Customer first name.
- Customer last name.
- Customer email.
- Customer phone.
- PAXI/PEP recipient phone.
- PAXI/PEP store code.
- Collection point name.
- Town or city.
- Province.
- Return policy acceptance.
- Cart item presence.
- Cart product, colour, quantity, and measurement validity through `ICartQuoteService`.

`backend/src/BiliStore.Api/Controllers/CheckoutController.cs`

Adds:

```text
POST /api/checkout/review
```

## Backend Files Updated

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

```text
ICheckoutReviewService -> CheckoutReviewService
```

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 8 - Checkout and PAXI/PEP delivery
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds a manual example for:

```text
POST /api/checkout/review
```

## Checkout Review Flow

1. The frontend sends customer details, delivery details, return policy acceptance, and cart items.
2. The backend checks that required checkout fields are present.
3. The backend sends the cart section through the same cart quote service used by the cart page.
4. The backend returns validation issues if anything is missing or invalid.
5. If everything passes, `canContinue` is `true`.

This matters because totals and cart validity are checked by the API, not trusted only from browser state.

## Frontend Files Added

`frontend/src/types/checkout.ts`

Defines frontend types for:

- Customer checkout details.
- PAXI/PEP delivery details.
- Checkout review request.
- Checkout validation issue.
- Checkout review response.

`frontend/src/services/checkoutApi.ts`

Adds:

```text
reviewCheckout()
```

This posts to:

```text
/api/checkout/review
```

`frontend/src/utils/cartQuote.ts`

Adds:

```text
buildCartQuoteRequest()
```

The cart page and checkout page now share the same helper for turning persisted cart items into the backend quote request shape.

## Frontend Files Updated

`frontend/src/pages/CheckoutPage.tsx`

Replaces the placeholder with a real checkout flow:

- Empty-cart state.
- Customer step.
- PAXI/PEP delivery step.
- Return policy step.
- Order review step.
- Backend checkout review submission.
- Checkout issue display.
- Successful review state.
- Sticky review totals on desktop.

`frontend/src/pages/CartPage.tsx`

Now uses `buildCartQuoteRequest()` and updates the checkout helper text.

`frontend/src/pages/AccountPage.tsx`

Updates the order-history placeholder so it now points only to order creation, because checkout review exists.

`frontend/src/styles/global.css`

Adds checkout page styling.

## How To Explain The Checkout Code

The checkout page is a multi-step form, but it does not create an order yet.

The steps collect:

1. Who is ordering.
2. Where the PAXI/PEP parcel should be collected.
3. Whether the customer accepts the return policy.
4. A final review of the cart and totals.

When the customer clicks **Review checkout**, the frontend sends the full checkout payload to the backend.

The backend validates the customer fields, delivery fields, policy checkbox, and cart. It uses the same cart quote service from Phase 6, so checkout totals come from backend product prices rather than only from the browser.

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

1. Add a product to the cart from the shop.
2. Open Cart.
3. Continue to Checkout.
4. Enter customer details.
5. Enter PAXI/PEP delivery details.
6. Accept the return policy.
7. Open Review.
8. Click **Review checkout**.
9. Confirm the successful review message appears.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
POST /api/checkout/review
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

Passed. This means the checkout page, checkout API service, checkout types, and shared cart quote helper compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this is the same environment limitation from previous phases, not a TypeScript error.

## What Comes Next

Phase 9 should implement payment preparation:

- Payment provider abstraction.
- Payment intent or payment reference placeholder.
- Payment status model usage.
- Payment-start endpoint.
- Frontend payment step after checkout review.
