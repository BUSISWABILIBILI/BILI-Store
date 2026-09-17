# Phase 6 - Shopping Cart

## What This Phase Added

Phase 6 turns the cart from a placeholder into a working custom-clothing cart.

It adds:

- Backend cart quote DTOs.
- A backend cart quote service.
- A `POST /api/cart/quote` endpoint.
- A persisted frontend cart store.
- Add-to-cart from the product measurement form.
- Colour/custom-colour selection before adding to cart.
- Quantity selection before adding to cart.
- A real cart page showing products, colours, measurements, quantities, unit prices, and totals.
- Cart quantity updates.
- Remove item and clear cart actions.
- Backend quote integration for subtotal, delivery fee, and grand total.

The cart now preserves the important custom-ordering data from the brief:

- Product.
- Selected colour or custom colour.
- Validated measurements.
- Quantity.
- Unit price.
- Production estimate.
- Delivery estimate.
- Grand total.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Cart/`

Contains cart quote request/response models:

- `CartMeasurementInputDto.cs`
- `CartQuoteItemRequestDto.cs`
- `CartQuoteRequestDto.cs`
- `CartQuoteItemResponseDto.cs`
- `CartQuoteIssueDto.cs`
- `CartQuoteResponseDto.cs`

`backend/src/BiliStore.Application/Cart/ICartQuoteService.cs`

Defines the cart quote contract.

`backend/src/BiliStore.Infrastructure/Cart/CartQuoteService.cs`

Implements cart quote logic using EF Core.

It:

- Loads each product by slug.
- Uses backend product prices instead of trusting frontend prices.
- Checks product availability.
- Checks quantity.
- Checks selected colour availability.
- Checks whether custom colours are allowed.
- Checks required measurements are present.
- Calculates subtotal, delivery fee, and grand total.

`backend/src/BiliStore.Api/Controllers/CartController.cs`

Adds:

```text
POST /api/cart/quote
```

## Backend Files Updated

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

```text
ICartQuoteService -> CartQuoteService
```

`backend/src/BiliStore.Infrastructure/Data/BiliStoreDbContext.cs`

Uses the fully qualified domain cart entity name to avoid a C# namespace conflict with `BiliStore.Infrastructure.Cart`.

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 6 - Cart
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds a sample cart quote request.

## Frontend Files Added

`frontend/src/types/cart.ts`

Defines cart item, colour selection, quote request, and quote response types.

`frontend/src/services/cartApi.ts`

Adds:

```ts
quoteCart()
```

This posts cart data to:

```text
/api/cart/quote
```

`frontend/src/stores/cartStore.ts`

Creates a Zustand store that persists cart items in local storage.

The storage key is:

```text
bili-store.cart
```

This lets the cart survive page refreshes before customer login is built.

## Frontend Files Updated

`frontend/src/components/shop/ProductMeasurementForm.tsx`

Now supports:

- Product colour swatch selection.
- Custom colour name and colour picker.
- Quantity.
- Measurement validation.
- Add custom garment to cart.

The item can only be added properly after the measurement validator accepts the measurements.

`frontend/src/components/layout/AppLayout.tsx`

The cart badge now shows the real persisted cart item count.

`frontend/src/pages/CartPage.tsx`

Now displays:

- Cart items.
- Product image.
- Product name and description.
- Clothing type.
- Selected colour/custom colour.
- Measurements.
- Quantity controls.
- Remove item button.
- Clear cart button.
- Backend quote totals.
- Checkout call-to-action.

If the backend quote endpoint is offline, the cart still shows local estimated totals so the UI remains usable during frontend work.

`frontend/src/styles/global.css`

Adds styling for:

- Product colour choices.
- Custom colour fields.
- Quantity controls.
- Cart item cards.
- Cart summary panel.
- Cart measurements.
- Mobile cart layout.

## How To Explain The Cart Architecture

The cart has two layers:

1. Frontend persisted cart:
   - Stores the customer’s current cart in browser local storage.
   - Keeps the selected product, colour, measurements, quantity, and display data.
   - Makes page refresh safe.

2. Backend cart quote:
   - Recalculates prices using database product data.
   - Checks products, colours, quantities, and measurements.
   - Returns trusted totals.

This is important because the frontend should not be trusted as the final source of price truth.

## How To Test

Backend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend"
dotnet build BiliStore.sln --no-restore
dotnet run --project src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Manual cart quote request:

```powershell
$body = @{
  items = @(
    @{
      productSlug = "burgundy-imvunulo-dress"
      selectedColourId = "20000000-0000-0000-0000-000000000007"
      customColourName = $null
      customColourHex = $null
      quantity = 1
      measurements = @(
        @{ measurementKey = "bust_chest"; measurementName = "Bust/Chest"; valueCm = 94; unit = "cm" },
        @{ measurementKey = "waist"; measurementName = "Waist"; valueCm = 76; unit = "cm" },
        @{ measurementKey = "hips"; measurementName = "Hips"; valueCm = 104; unit = "cm" },
        @{ measurementKey = "shoulder_width"; measurementName = "Shoulder width"; valueCm = 40; unit = "cm" },
        @{ measurementKey = "dress_length"; measurementName = "Dress length"; valueCm = 145; unit = "cm" }
      )
    }
  )
} | ConvertTo-Json -Depth 5

Invoke-RestMethod -Method Post -Uri "http://localhost:5087/api/cart/quote" -Body $body -ContentType "application/json"
```

Frontend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend"
pnpm dev
```

Then:

1. Open Shop.
2. Click `View details` on a product.
3. Choose a colour or custom colour.
4. Enter required measurements.
5. Click `Validate measurements`.
6. Click `Add custom garment to cart`.
7. Open Cart.
8. Change quantity, remove an item, or continue to checkout.

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

Passed. This means the cart store, cart API types, product measurement form, cart page, and layout updates compile.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. This is the same environment limitation from previous phases, not a TypeScript error.

## What Comes Next

Phase 7 should implement customer accounts:

- Register.
- Login.
- JWT authentication.
- Logout.
- Account profile.
- Saved measurement profiles connected to a customer account.
- Order history foundation.
