# BILI Store

BILI Store is a planned full-stack e-commerce ordering system for custom-sewn traditional clothing. The application will let customers browse garments, choose colors, provide body measurements, checkout online, and supply PAXI/PEP delivery details while keeping the selected outfit, color, measurements, delivery data, payment status, and order status connected from cart to confirmation.

## Current Phase

Phase 11 is complete as the admin order management foundation. The repository now contains customer accounts, cart quoting, multi-step checkout, PAXI/PEP delivery review, return-policy acceptance, development payment session creation, saved orders, order confirmation, and admin order operations.

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

See `docs/PHASE_5_MEASUREMENTS.md` for:

- Backend measurement validation endpoint
- Dynamic product measurement fields
- Frontend product measurement form
- Browser-local saved measurement profiles
- Run/test commands and verification results

See `docs/PHASE_6_CART.md` for:

- Backend cart quote endpoint
- Persisted frontend cart store
- Add-to-cart from product customisation
- Cart page item/totals behaviour
- Run/test commands and verification results

See `docs/PHASE_7_CUSTOMER_ACCOUNTS.md` for:

- Register/login and JWT authentication
- Customer profile endpoints
- Account-backed saved measurement profiles
- Order-history foundation
- Frontend auth store and account page behaviour

See `docs/PHASE_8_CHECKOUT.md` for:

- Multi-step checkout
- PAXI/PEP delivery details
- Return-policy acceptance
- Backend checkout review validation
- Shared cart quote request handling

See `docs/PHASE_9_PAYMENTS.md` for:

- Payment service abstraction
- Development payment provider
- Payment session endpoint
- Frontend payment step
- Real provider configuration points

See `docs/PHASE_10_ORDERS.md` for:

- Order creation endpoint
- Order confirmation lookup
- Order snapshot behaviour
- Frontend order confirmation page
- Account order-history links

See `docs/PHASE_11_ADMIN.md` for:

- Admin-protected order routes
- Admin order queue and detail view
- Order and payment status updates
- Payment reference review
- Return request review foundation

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
