# Phase 4 - Product Catalogue

## What This Phase Added

Phase 4 connects the frontend catalogue UI to real backend API endpoints.

It adds:

- Product catalogue DTOs in the Application layer.
- A catalogue service interface.
- An EF Core-backed catalogue service in Infrastructure.
- Seeded sample products, product images, product colours, product categories, and measurement requirements.
- Public API endpoints for products, categories, colours, and measurements.
- Frontend TypeScript types matching the API DTOs.
- Frontend API service functions using Axios.
- Shop page search and filters that call the backend.
- Product detail loading by product slug.
- Homepage featured products loaded from `/api/products?featured=true`.
- Measurement guide cards loaded from `/api/measurements`.

The important shift is that product data is no longer only a static React preview. The React app now knows how to request catalogue data from the ASP.NET Core API.

## Backend Files Added

`backend/src/BiliStore.Application/DTOs/Catalogue/`

Contains API response models:

- `CategoryDto.cs`
- `ColourDto.cs`
- `MeasurementTypeDto.cs`
- `ProductImageDto.cs`
- `ProductMeasurementRequirementDto.cs`
- `ProductSummaryDto.cs`
- `ProductDetailsDto.cs`

These DTOs are what the frontend receives. We do not return EF Core entities directly.

`backend/src/BiliStore.Application/Catalogue/ProductCatalogueQuery.cs`

Represents product filters:

- Search
- Category
- Clothing type
- Minimum price
- Maximum price
- Featured status

`backend/src/BiliStore.Application/Catalogue/ICatalogueService.cs`

Defines what catalogue data the application can request without knowing how EF Core works.

## Backend Files Updated

`backend/src/BiliStore.Infrastructure/Catalogue/CatalogueService.cs`

Implements `ICatalogueService` using EF Core.

It:

- Reads products from `BiliStoreDbContext`.
- Includes categories, images, colours, and measurement requirements.
- Applies filters.
- Maps domain entities into DTOs.
- Returns product details by slug.

`backend/src/BiliStore.Infrastructure/Data/SeedData.cs`

Adds realistic catalogue seed data:

- 8 sample products.
- Product images.
- Product-category relationships.
- Product-colour relationships.
- Product-measurement requirements.

This keeps the custom clothing concept visible in the database model. For example, a dress requires bust/chest, waist, hips, shoulder width, and dress length, while a skirt set requires waist, hips, and skirt length.

`backend/src/BiliStore.Infrastructure/DependencyInjection.cs`

Registers:

`ICatalogueService -> CatalogueService`

This is how controllers receive the catalogue service through dependency injection.

`backend/src/BiliStore.Api/Controllers/`

New controllers:

- `ProductsController.cs`
- `CategoriesController.cs`
- `ColoursController.cs`
- `MeasurementsController.cs`

## Backend API Endpoints

Products:

```text
GET /api/products
GET /api/products?category=women
GET /api/products?category=women&featured=true
GET /api/products?search=dress
GET /api/products?clothingType=Suit
GET /api/products/{slug}
```

Categories:

```text
GET /api/categories
```

Colours:

```text
GET /api/colours
```

Measurements:

```text
GET /api/measurements
```

System:

```text
GET /api/system
```

Now returns Phase 4 API information.

## Frontend Files Added

`frontend/src/types/catalogue.ts`

Defines TypeScript types that match the backend DTOs.

`frontend/src/services/catalogueApi.ts`

Defines Axios calls:

- `getProducts`
- `getProductBySlug`
- `getCategories`
- `getColours`
- `getMeasurements`

## Frontend Files Updated

`frontend/src/components/home/FeaturedProductCard.tsx`

Can now render either:

- Phase 3 local preview products.
- Phase 4 API products.

This lets the UI keep working while the API is offline, but it uses real backend data when available.

`frontend/src/pages/HomePage.tsx`

Loads featured products from:

```text
/api/products?featured=true
```

`frontend/src/pages/ShopPage.tsx`

Now:

- Loads categories from `/api/categories`.
- Loads products from `/api/products`.
- Sends search/category/clothing type filters to the backend.
- Loads selected product details from `/api/products/{slug}`.
- Falls back to local preview cards if the API is offline.

`frontend/src/pages/MeasurementGuidePage.tsx`

Loads measurement guide data from:

```text
/api/measurements
```

`frontend/src/styles/global.css`

Adds styling for:

- Active filter pills.
- API warning messages.
- Product detail panel.

## How To Explain The Architecture

The product catalogue uses a layered flow:

1. React calls `catalogueApi.ts`.
2. Axios sends requests to the ASP.NET Core API.
3. Controllers receive HTTP requests.
4. Controllers call `ICatalogueService`.
5. `CatalogueService` uses EF Core and `BiliStoreDbContext`.
6. EF Core reads Products, Categories, Colours, Images, and Measurement Requirements.
7. The service maps entities into DTOs.
8. React receives typed product data and renders cards, filters, and details.

This is cleaner than having React use hard-coded product arrays forever, and safer than exposing database entities directly.

## How To Run And Test

Backend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend"
dotnet restore BiliStore.sln
dotnet build BiliStore.sln
dotnet run --project src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Frontend:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend"
pnpm install
pnpm dev
```

Manual API checks:

```powershell
Invoke-RestMethod -Uri "http://localhost:5087/api/products"
Invoke-RestMethod -Uri "http://localhost:5087/api/categories"
Invoke-RestMethod -Uri "http://localhost:5087/api/colours"
Invoke-RestMethod -Uri "http://localhost:5087/api/measurements"
Invoke-RestMethod -Uri "http://localhost:5087/api/products/burgundy-imvunulo-dress"
```

## Database Migration Note

The EF Core model and seed data are now ready for catalogue tables and data.

This Codex environment does not have the `dotnet-ef` CLI tool installed, so migrations could not be generated here.

On your normal machine, install the tool if needed:

```powershell
dotnet tool install --global dotnet-ef
```

Then create the migration:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend"
dotnet ef migrations add InitialCatalogue --project src\BiliStore.Infrastructure --startup-project src\BiliStore.Api --output-dir Data\Migrations
dotnet ef database update --project src\BiliStore.Infrastructure --startup-project src\BiliStore.Api
```

After the database update, the product endpoints can read the seeded sample catalogue from SQL Server LocalDB.

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

Passed. This means the TypeScript API types, React Query calls, and component props compile.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the Vite/esbuild child process with `spawn EPERM`. This is the same environment limitation from previous phases, not a TypeScript error.

## What Comes Next

Phase 5 should implement measurements deeply:

- Product detail measurement form.
- Dynamic required measurement fields from product details.
- Measurement validation.
- Saved customer measurement profiles.
- Clear missing-measurement messages before cart/checkout.
