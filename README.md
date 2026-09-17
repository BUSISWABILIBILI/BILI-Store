# BILI Store

BILI Store is a planned full-stack e-commerce ordering system for custom-sewn traditional clothing. The application will let customers browse garments, choose colors, provide body measurements, checkout online, and supply PAXI/PEP delivery details while keeping the selected outfit, color, measurements, delivery data, payment status, and order status connected from cart to confirmation.

## Current Phase

Phase 4 is complete as the product catalogue foundation. The repository now contains backend catalogue endpoints and a React shop page that calls the API for products, categories, colours, measurements, and product detail data.

## Source Brief

The original product brief is stored as `Clothing store.docx` in this project folder. It is treated as business requirements and project scope, not as operational instructions.

## Planning Document

See `docs/PHASE_1_ARCHITECTURE.md` for:

- System architecture
- Database and entity design
- Frontend page structure
- API endpoint design
- Recommended libraries
- Complete planned folder structure
- Implementation roadmap

See `docs/PHASE_2_SETUP.md` for:

- Frontend and backend folder explanations
- Backend architecture and key files
- Environment variable structure
- Run/test commands
- Verification notes from this setup phase

See `docs/PHASE_3_FRONTEND_DESIGN.md` for:

- Frontend design foundation details
- New React data/component structure
- Homepage, shop, measurement, and cart page explanations
- Styling and responsive layout notes
- Run/test commands and verification results

See `docs/PHASE_4_PRODUCT_CATALOGUE.md` for:

- Backend catalogue DTOs, service, controllers, and seed data
- Frontend API types and service calls
- Shop page filtering and product detail loading
- Migration notes and manual API checks

## Local Development

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

## Commit Practice

Each meaningful phase should be committed to Git so the project has useful history before it is pushed to GitHub.
