# Phase 12 - Admin Catalogue Management

## What This Phase Added

Phase 12 adds admin catalogue management.

It adds:

- Admin product list.
- Product create and edit workflow.
- Product deactivate workflow.
- Category create and edit workflow.
- Colour create and edit workflow.
- Product category assignments.
- Product colour assignments.
- Product measurement requirement assignments.
- Admin catalogue frontend page.

This phase lets the store owner maintain the products customers see in the shop.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Admin/`

Adds:

- `AdminCategoryDto.cs`
- `AdminColourDto.cs`
- `AdminProductDetailsDto.cs`
- `AdminProductMeasurementRequirementDto.cs`
- `AdminProductMeasurementRequirementInputDto.cs`
- `AdminProductSummaryDto.cs`
- `UpsertAdminCategoryRequestDto.cs`
- `UpsertAdminColourRequestDto.cs`
- `UpsertAdminProductRequestDto.cs`

These DTOs define the data contract between the admin frontend and the API.

`backend/src/BiliStore.Api/Controllers/AdminCatalogueController.cs`

Adds admin-only catalogue endpoints:

```text
GET    /api/admin/products
GET    /api/admin/products/{id}
POST   /api/admin/products
PUT    /api/admin/products/{id}
DELETE /api/admin/products/{id}
GET    /api/admin/categories
POST   /api/admin/categories
PUT    /api/admin/categories/{id}
GET    /api/admin/colours
POST   /api/admin/colours
PUT    /api/admin/colours/{id}
```

These routes use:

```text
[Authorize(Roles = "Admin")]
```

## Backend Files Updated

`backend/src/BiliStore.Api/Controllers/SystemController.cs`

Now reports:

```text
Phase 12 - Admin catalogue management
```

`backend/src/BiliStore.Api/BiliStore.Api.http`

Adds manual examples for admin catalogue routes.

## Backend Flow

Product create and update requests send one full product form to the API.

The controller:

1. Normalizes the product slug.
2. Checks duplicate product slugs.
3. Saves product fields like name, price, type, availability, and production estimate.
4. Replaces product category links.
5. Replaces product colour links.
6. Replaces product measurement requirements.
7. Upserts the primary product image.
8. Returns the saved product details.

The `DELETE /api/admin/products/{id}` route does not remove the product row from the database. It sets:

```text
IsAvailable = false
```

This protects old orders, because order history may still reference that product.

## Frontend Files Added

`frontend/src/pages/AdminCataloguePage.tsx`

Adds the admin catalogue page at:

```text
/admin/catalogue
```

The page shows:

- Product list.
- Product edit form.
- Product availability, featured, and custom-colour toggles.
- Category checkbox assignments.
- Colour checkbox assignments with swatches.
- Measurement requirement cards with min/max/help text.
- Category management form.
- Colour management form.

## Frontend Files Updated

`frontend/src/types/admin.ts`

Adds frontend types for:

- Admin products.
- Admin product details.
- Admin product save requests.
- Admin categories.
- Admin colours.
- Product measurement requirement drafts.

`frontend/src/services/adminApi.ts`

Adds API calls for:

- Admin products.
- Admin categories.
- Admin colours.

`frontend/src/app/routes.ts`

Adds:

```text
/admin/catalogue
```

`frontend/src/components/layout/AppLayout.tsx`

Adds an admin Catalogue link when the logged-in account has the `Admin` role.

`frontend/src/styles/global.css`

Adds responsive admin catalogue styling.

## How To Explain The Admin Catalogue Flow

The public shop reads catalogue data.

The admin catalogue page writes catalogue data.

That means the store owner can add or update the information customers browse, including:

- Product name and description.
- Product price.
- Product category.
- Product colours.
- Product measurements needed before checkout.
- Whether the product is available.
- Whether the product is featured.

The frontend keeps the form state in React, converts it into an `UpsertAdminProductRequest`, and sends it to the API.

The backend validates important relationships by checking whether category, colour, and measurement IDs exist before saving those links.

## Why Measurement Requirements Are Managed Per Product

Different garments need different measurements.

For example:

- A dress needs bust, waist, hips, and length.
- A shirt needs chest, shoulders, sleeve, and length.
- A skirt needs waist, hips, and skirt length.

The product measurement requirements tell the product detail page which measurement fields to ask the customer for.

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
3. Open `/admin/catalogue`.
4. Select a product.
5. Change product fields, categories, colours, or measurement requirements.
6. Save the product.
7. Open `/shop` and confirm the product data still loads.

Manual API flow:

Use:

```text
backend/src/BiliStore.Api/BiliStore.Api.http
```

Run:

```text
GET    /api/admin/products
POST   /api/admin/products
PUT    /api/admin/products/{id}
DELETE /api/admin/products/{id}
GET    /api/admin/categories
POST   /api/admin/categories
PUT    /api/admin/categories/{id}
GET    /api/admin/colours
POST   /api/admin/colours
PUT    /api/admin/colours/{id}
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

Passed. This means the admin catalogue types, API service, route, page, and styles compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. The command completes TypeScript checking before that failure, so this remains the same environment limitation from previous phases.

## What Comes Next

Phase 13 should add admin customer and dashboard support:

- Admin customer lookup.
- Store activity dashboard.
- Order count and revenue summary.
- Low-stock or unavailable product overview.
- Admin-only setup notes for assigning roles.
