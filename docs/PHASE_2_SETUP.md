# Phase 2 - Project Setup Notes

## What This Phase Added

Phase 2 created the runnable full-stack foundation for BILI Store:

- A React + TypeScript frontend in `frontend/`.
- An ASP.NET Core Web API backend in `backend/`.
- A layered backend structure for Domain, Application, Infrastructure, and API code.
- EF Core entity and DbContext setup for the custom clothing domain.
- SQL Server connection-string configuration.
- ASP.NET Core Identity user storage foundation.
- CORS configuration so the React app can call the backend.
- Development configuration and environment variable examples.
- Health/system endpoints so the frontend can verify API connectivity.

This phase does not implement the full catalogue, cart, accounts, checkout, payments, orders, or admin dashboard yet. Those are separate phases from the original brief.

## Backend Structure

The backend solution lives at:

`C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend\BiliStore.sln`

Projects:

- `BiliStore.Domain`: Core business entities and enums. This project does not know about HTTP, SQL Server, or UI.
- `BiliStore.Application`: DTOs, service contracts, and application-level wiring. Later phases will add validation and use-case services here.
- `BiliStore.Infrastructure`: EF Core, SQL Server, Identity storage, seed data, repositories, payment providers, and delivery providers.
- `BiliStore.Api`: Controllers, CORS, authentication setup, logging, OpenAPI, and HTTP pipeline configuration.

This structure matters because it keeps business rules away from framework-specific code. For example, an `Order` is a business concept in Domain, while `BiliStoreDbContext` is Infrastructure because it is specifically about database persistence.

## Backend Files To Understand

- `backend/src/BiliStore.Api/Program.cs`: The startup file. It registers services, configures CORS, JWT bearer authentication, controllers, OpenAPI, and logging.
- `backend/src/BiliStore.Api/Controllers/HealthController.cs`: Returns a simple health response at `/api/health`.
- `backend/src/BiliStore.Api/Controllers/SystemController.cs`: Returns API information at `/api/system`.
- `backend/src/BiliStore.Infrastructure/Data/BiliStoreDbContext.cs`: EF Core database context. It lists database tables and configures relationships, indexes, precision, and string lengths.
- `backend/src/BiliStore.Infrastructure/Data/SeedData.cs`: Seeds permanent foundation data such as categories, colors, and measurement types.
- `backend/src/BiliStore.Domain/Entities/`: Contains the business data models for catalogue, customers, cart, orders, delivery, payments, and returns.
- `backend/src/BiliStore.Domain/Enums/`: Contains controlled status values such as order status, payment status, and delivery status.

## Backend Configuration

Development settings are in:

`backend/src/BiliStore.Api/appsettings.Development.json`

Important values:

- `ConnectionStrings:BiliStoreDb`: SQL Server LocalDB connection string for development.
- `Jwt:SigningKey`: A development-only placeholder key. Real secrets should use user secrets or environment variables.
- `Cors:AllowedOrigins`: Allows the frontend running at `http://localhost:5173`.

Production secrets must not be committed to Git. Use environment variables such as:

- `ConnectionStrings__BiliStoreDb`
- `Jwt__SigningKey`
- `Payments__PayFast__MerchantId`
- `Payments__PayFast__MerchantKey`
- `Payments__Yoco__SecretKey`

The root `.env.example` shows the expected shape without real secrets.

## Backend Commands

From:

`C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\backend`

Normal restore/build commands:

```powershell
dotnet restore BiliStore.sln
dotnet build BiliStore.sln
dotnet run --project src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Then test:

```powershell
Invoke-RestMethod -Uri "http://localhost:5087/api/health"
Invoke-RestMethod -Uri "http://localhost:5087/api/system"
```

In the Codex sandbox, Windows TLS could not connect to NuGet through normal shell tools, so I used Python to create a temporary local package source under `backend/.nuget/`. That folder is ignored by Git. The backend was restored and built successfully using that local package source.

## Frontend Structure

The frontend lives at:

`C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend`

Important files:

- `frontend/package.json`: React, routing, API, form, validation, state, and icon dependencies.
- `frontend/pnpm-workspace.yaml`: Allows the required `esbuild` build script when pnpm installs Vite dependencies.
- `frontend/src/main.tsx`: React entry point.
- `frontend/src/App.tsx`: Top-level route configuration.
- `frontend/src/app/routes.ts`: Central list of current route shells.
- `frontend/src/components/layout/AppLayout.tsx`: Shared navigation, announcement bar, page shell, and footer.
- `frontend/src/components/ui/ApiStatusPanel.tsx`: Calls the backend health and system endpoints.
- `frontend/src/services/apiClient.ts`: Axios client using `VITE_API_BASE_URL`.
- `frontend/src/styles/tokens.css`: Design tokens such as colors, font stacks, and shadow values.
- `frontend/src/styles/global.css`: Base responsive layout styling.

The current pages are placeholders for the correct phase order. Phase 3 will turn the frontend into a stronger premium boutique design, and later phases will fill in real catalogue, measurements, cart, checkout, account, and admin features.

## Frontend Commands

From:

`C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend`

Use pnpm:

```powershell
pnpm install
pnpm dev
```

The frontend development URL is:

`http://127.0.0.1:5173`

If your global `npm` command is still pointing to another user profile, use the bundled pnpm path that worked in Codex:

```powershell
& "C:\Users\BusiswaB\.cache\codex-runtimes\codex-primary-runtime\dependencies\bin\fallback\pnpm.cmd" install
& "C:\Users\BusiswaB\.cache\codex-runtimes\codex-primary-runtime\dependencies\bin\fallback\pnpm.cmd" dev
```

## Verification Results

Backend:

- `dotnet build BiliStore.sln --no-restore` passed.
- `GET http://localhost:5087/api/health` returned `Healthy`.
- `GET http://localhost:5087/api/system` returned Phase 2 API information.

Frontend:

- `pnpm lint` passed, which runs `tsc --noEmit --pretty false`.
- `pnpm build` reached Vite bundling but Codex could not finish it because this sandbox blocks Node from spawning the `esbuild` process. That is an environment restriction, not a TypeScript code error.

## What To Explain If Asked

This project is being built phase by phase so it does not become a shallow online shop. Phase 2 created the technical foundation:

- React handles the customer interface.
- ASP.NET Core exposes real API endpoints.
- EF Core is prepared to map the custom clothing data model to SQL Server.
- Identity is prepared for customer/admin authentication.
- CORS allows frontend-backend communication during development.
- Configuration separates development settings from real production secrets.

The next phase is Phase 3: frontend design foundation. That phase will make the visible customer experience look like a premium traditional fashion boutique.
