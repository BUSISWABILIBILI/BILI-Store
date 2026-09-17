# Phase 5 - Measurements

## What This Phase Added

Phase 5 makes measurements part of the product-detail workflow.

It adds:

- Backend measurement validation DTOs.
- A backend measurement validation service.
- A `POST /api/measurements/validate` endpoint.
- A frontend product measurement form.
- Dynamic measurement fields based on the selected product.
- Server-side validation for missing or out-of-range measurements.
- Browser-local saved measurement profiles.
- Product detail UI that shows image, fabric, colours, details, and required measurements together.

The key idea is that each product defines its own required measurements. A dress does not ask for the same measurements as a suit, shirt, skirt, or children’s set.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Measurements/`

Contains the request and response models used by the validation endpoint:

- `MeasurementValueInputDto.cs`
- `ValidateProductMeasurementsRequestDto.cs`
- `AcceptedMeasurementDto.cs`
- `MeasurementValidationIssueDto.cs`
- `MeasurementValidationResponseDto.cs`

`backend/src/BiliStore.Application/Measurements/IMeasurementValidationService.cs`

Defines the validation contract.

`backend/src/BiliStore.Infrastructure/Measurements/MeasurementValidationService.cs`

Implements the validation logic using EF Core.

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/MeasurementsController.cs`

Adds:

```text
POST /api/measurements/validate
```

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

```text
IMeasurementValidationService -> MeasurementValidationService
```

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 5 - Measurements
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds a sample validation request.

## Measurement Validation Flow

The frontend sends:

```json
{
  "productSlug": "burgundy-imvunulo-dress",
  "measurements": [
    { "measurementKey": "bust_chest", "valueCm": 94 },
    { "measurementKey": "waist", "valueCm": 76 }
  ]
}
```

The backend then:

1. Loads the product by slug.
2. Loads the product’s required measurement rules.
3. Checks every required measurement.
4. Reports missing values.
5. Checks minimum and maximum centimetre ranges.
6. Returns either accepted measurements or validation issues.

This prevents checkout from accepting a custom garment without its required measurements.

## Frontend Files Added

`frontend/src/types/measurements.ts`

Defines TypeScript types for:

- Measurement validation request.
- Measurement validation response.
- Validation issues.
- Accepted measurements.
- Saved local measurement profile.

`frontend/src/utils/measurementProfiles.ts`

Handles browser-local profile storage.

Profiles are saved in `localStorage` under:

```text
bili-store.measurement-profiles
```

This is temporary until Phase 7 adds authenticated customer accounts and real saved measurement profiles.

`frontend/src/components/shop/ProductMeasurementForm.tsx`

Displays the product’s required measurement fields and handles:

- Measurement entry.
- Backend validation.
- Validation success messages.
- Validation error messages.
- Saving a local measurement profile.
- Applying a saved local profile.

## Frontend Files Updated

`frontend/src/services/catalogueApi.ts`

Adds:

```ts
validateProductMeasurements()
```

This function posts values to:

```text
/api/measurements/validate
```

`frontend/src/pages/ShopPage.tsx`

The selected product detail panel now shows:

- Product image.
- Product name and description.
- Fabric description.
- Available colour swatches.
- Sewing time.
- Custom-colour availability.
- Dynamic required measurement form.

`frontend/src/styles/global.css`

Adds styling for:

- Product detail layout.
- Product image panel.
- Measurement form grid.
- Measurement fields.
- Validation success panel.
- Validation issue panel.
- Saved profile controls.
- Mobile stacking.

## How To Explain Local Saved Profiles

Saved measurement profiles are available now, but they are saved only in the browser.

That means:

- They help demonstrate the customer experience before authentication is built.
- They survive page refreshes on the same device/browser.
- They are not yet connected to a customer account.
- In Phase 7, the same idea will move to backend customer profiles after login/register is implemented.

This is intentional because authentication comes later in the project roadmap.

## How To Test

Backend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend"
dotnet build BiliStore.sln --no-restore
dotnet run --project src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Manual validation request:

```powershell
$body = @{
  productSlug = "burgundy-imvunulo-dress"
  measurements = @(
    @{ measurementKey = "bust_chest"; valueCm = 94 },
    @{ measurementKey = "waist"; valueCm = 76 },
    @{ measurementKey = "hips"; valueCm = 104 },
    @{ measurementKey = "shoulder_width"; valueCm = 40 },
    @{ measurementKey = "dress_length"; valueCm = 145 }
  )
} | ConvertTo-Json -Depth 4

Invoke-RestMethod -Method Post -Uri "http://localhost:5087/api/measurements/validate" -Body $body -ContentType "application/json"
```

Frontend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend"
pnpm dev
```

Then:

1. Open the shop page.
2. Click a product’s `View details`.
3. Enter the required measurements.
4. Click `Validate measurements`.
5. Save a profile.
6. Reopen another product and apply the saved profile.

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

Passed. This means the TypeScript measurement types, API calls, local profile helper, and React component compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. This is the same environment limitation from previous phases, not a TypeScript error.

## What Comes Next

Phase 6 should implement the real shopping cart.

The cart must preserve:

- Selected product.
- Selected colour or custom colour.
- Validated measurements.
- Quantity.
- Unit price.
- Subtotal.
- Delivery fee.
- Grand total.
