# Phase 1 - Architecture Plan

## Requirement Handling

The Word document in the repository is used as the project requirements brief. Any instructions inside that document describe the desired application and development phases. They do not override the user's direct request to save the project in this folder and keep Git commit history.

Phase 1 produces the architecture and roadmap only. Full application scaffolding starts in Phase 2.

## Product Goal

BILI Store will be a premium online ordering platform for custom-sewn traditional clothing. The main product value is not a generic cart; it is preserving each customer's chosen garment, color, custom color notes, body measurements, quantity, PAXI/PEP delivery details, payment record, and order status throughout the entire purchase lifecycle.

## System Architecture

### High-Level Shape

- Frontend: React single-page application using React Router and a mobile-first responsive design.
- Backend: ASP.NET Core Web API using a layered architecture inspired by Clean Architecture.
- Database: SQL Server accessed through Entity Framework Core.
- API communication: JSON over HTTPS, with DTOs for all public request and response payloads.
- Authentication: ASP.NET Core Identity with JWT bearer authentication.
- Payments: Payment service abstraction with a development/test provider first and a clear path to PayFast, Peach Payments, or Yoco.
- Delivery: Delivery service abstraction beginning with PAXI/PEP collection-point delivery.
- Images: Product image metadata stored in the database; physical image storage can begin locally in development and later move to cloud/blob storage.

### Backend Layers

- Domain: Core entities, enums, and business rules that do not depend on infrastructure.
- Application: DTOs, validation, interfaces, use-case services, and orchestration logic.
- Infrastructure: EF Core DbContext, repositories, Identity configuration, payment provider implementations, delivery provider implementations, seeding, and external integrations.
- API: Controllers, middleware, authentication setup, CORS, request validation, logging, and dependency injection composition.

### Frontend Shape

- App shell: Routing, layout, navigation, footer, global providers, and protected route handling.
- Features: Product catalogue, product details, measurement guide, cart, checkout, authentication, customer account, orders, admin.
- Shared UI: Buttons, inputs, form fields, modals, cards, loaders, empty states, price display, color swatches, stepper, and validation messages.
- State: Server state handled with a query library; local cart state persisted safely in localStorage until authenticated cart sync is added.

## Core Domain Rules

- Products can belong to one or more categories.
- Products can have multiple colors, and some products can allow custom color input.
- Required measurements are configured per product or clothing type, not hard-coded globally.
- Checkout cannot continue if required measurements, customer details, PAXI/PEP delivery details, or return policy acceptance are missing.
- Raw card details must never be stored in the database.
- Orders keep a snapshot of product pricing, chosen color, custom color text, measurements, and delivery details so later catalogue changes do not alter historical orders.
- Custom-made return/refund copy must be marked as sample copy for owner review, not legal advice.

## Database and Entity Design

Use GUID primary keys for business entities unless a strong reason appears during implementation. Use created/updated timestamps on mutable entities.

### Identity and Customers

ApplicationUser:

- Id
- Email
- PhoneNumber
- PasswordHash managed by Identity
- Roles: Customer, Admin
- IsActive
- CreatedAt

CustomerProfile:

- Id
- ApplicationUserId
- FirstName
- LastName
- PreferredPhone
- CreatedAt
- UpdatedAt

CustomerMeasurementProfile:

- Id
- CustomerProfileId
- Name
- IsDefault
- Notes
- CreatedAt
- UpdatedAt

CustomerMeasurement:

- Id
- CustomerMeasurementProfileId
- MeasurementTypeId
- ValueCm

### Catalogue

Category:

- Id
- Name
- Slug
- ParentCategoryId
- DisplayOrder
- IsActive

Initial top-level categories:

- Women
- Men
- Girls
- Boys

Initial clothing type categories:

- Dresses
- Skirts
- Shorts
- Pants/Trousers
- Shirts
- Blouses
- Blazers
- Jackets
- Jumpsuits
- Two-piece outfits
- Traditional sets
- Suits
- Custom outfits

Product:

- Id
- Name
- Slug
- Description
- Price
- ClothingType
- EstimatedProductionDays
- IsAvailable
- IsFeatured
- AllowsCustomColor
- FabricDescription
- CreatedAt
- UpdatedAt

ProductCategory:

- ProductId
- CategoryId

ProductImage:

- Id
- ProductId
- Url
- AltText
- DisplayOrder
- IsPrimary

Colour:

- Id
- Name
- HexCode
- IsActive

ProductColour:

- ProductId
- ColourId

MeasurementType:

- Id
- Name
- Key
- Unit
- Description
- Instructions
- Tip
- IllustrationUrl
- DisplayOrder

ProductMeasurementRequirement:

- Id
- ProductId
- MeasurementTypeId
- IsRequired
- MinValueCm
- MaxValueCm
- HelpText

This supports product-specific requirements such as:

- Skirt: Waist, Hips, Skirt length
- Pants: Waist, Hips, Inseam, Trouser length, Thigh
- Dress: Bust/Chest, Waist, Hips, Shoulder width, Dress length

### Cart

Cart:

- Id
- CustomerProfileId nullable
- AnonymousCartKey nullable
- CreatedAt
- UpdatedAt

CartItem:

- Id
- CartId
- ProductId
- ProductSnapshotName
- ProductSnapshotPrice
- SelectedColourId nullable
- CustomColourName nullable
- CustomColourHex nullable
- Quantity
- CreatedAt
- UpdatedAt

CartItemMeasurement:

- Id
- CartItemId
- MeasurementTypeId
- ValueCm

### Orders

Order:

- Id
- OrderNumber
- CustomerProfileId nullable
- GuestEmail nullable
- FirstName
- LastName
- Email
- Phone
- OrderDate
- Subtotal
- DeliveryFee
- Total
- PaymentStatus
- OrderStatus
- ReturnPolicyAcceptedAt
- CreatedAt
- UpdatedAt

OrderItem:

- Id
- OrderId
- ProductId nullable
- ProductName
- ProductDescription
- UnitPrice
- Quantity
- SelectedColourName
- SelectedColourHex
- CustomColourName
- CustomColourHex
- ProductionDaysEstimate

OrderItemMeasurement:

- Id
- OrderItemId
- MeasurementTypeName
- MeasurementKey
- ValueCm

DeliveryDetails:

- Id
- OrderId
- DeliveryMethod
- RecipientPhone
- PaxiStoreCode
- CollectionPointName
- TownCity
- Province
- Instructions
- DeliveryStatus

Payment:

- Id
- OrderId
- Provider
- ProviderReference
- Amount
- Currency
- Status
- PaymentUrl nullable
- PaidAt nullable
- CreatedAt

ReturnRequest:

- Id
- OrderId
- Reason
- Details
- Status
- RequestedAt
- ResolvedAt nullable

### Enums

OrderStatus:

- PendingPayment
- Paid
- MeasurementsConfirmed
- InProduction
- ReadyForDelivery
- SentViaPaxi
- ReadyForCollection
- Completed
- Cancelled

PaymentStatus:

- Pending
- Paid
- Failed
- Cancelled
- Refunded

DeliveryStatus:

- Pending
- Booked
- SentViaPaxi
- ReadyForCollection
- Collected
- IssueReported

ReturnRequestStatus:

- Submitted
- UnderReview
- ApprovedForRemake
- ApprovedForRefund
- Rejected
- Closed

## API Design

All API endpoints should return DTOs, not EF entities. Use async/await, logging, validation, and meaningful HTTP status codes.

### Public Catalogue

- `GET /api/products` - search, filter, sort, paginate products.
- `GET /api/products/featured` - featured products for the homepage.
- `GET /api/products/{slug}` - product details with images, colors, and required measurements.
- `GET /api/categories` - category tree and clothing types.
- `GET /api/colours` - available color swatches.
- `GET /api/measurements` - measurement definitions and guide content.

### Authentication and Account

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`
- `GET /api/customers/me`
- `PUT /api/customers/me`
- `GET /api/customers/me/measurements`
- `POST /api/customers/me/measurements`
- `PUT /api/customers/me/measurements/{profileId}`
- `DELETE /api/customers/me/measurements/{profileId}`
- `GET /api/customers/me/orders`
- `GET /api/customers/me/orders/{orderNumber}`

### Cart

- `GET /api/cart`
- `POST /api/cart/items`
- `PUT /api/cart/items/{cartItemId}`
- `DELETE /api/cart/items/{cartItemId}`
- `DELETE /api/cart`
- `POST /api/cart/validate`

### Checkout, Delivery, Payments, Orders

- `POST /api/checkout/validate-customer`
- `POST /api/checkout/validate-delivery`
- `POST /api/checkout/review`
- `POST /api/orders` - create order from cart after validation and policy acceptance.
- `GET /api/orders/{orderNumber}` - confirmation and lookup.
- `POST /api/payments/create-session` - create payment request through the configured provider.
- `POST /api/payments/webhook/{provider}` - provider callback endpoint.
- `GET /api/delivery/options`
- `POST /api/delivery/validate-paxi-point`

### Admin

Admin routes require an Admin role.

- `GET /api/admin/products`
- `POST /api/admin/products`
- `PUT /api/admin/products/{id}`
- `DELETE /api/admin/products/{id}` or deactivate.
- `POST /api/admin/products/{id}/images`
- `DELETE /api/admin/products/{id}/images/{imageId}`
- `GET /api/admin/categories`
- `POST /api/admin/categories`
- `PUT /api/admin/categories/{id}`
- `GET /api/admin/orders`
- `GET /api/admin/orders/{orderNumber}`
- `PUT /api/admin/orders/{orderNumber}/status`
- `GET /api/admin/customers/{customerId}/measurements`
- `GET /api/admin/payments`
- `GET /api/admin/return-requests`
- `PUT /api/admin/return-requests/{id}/status`

## Frontend Page Structure

### Public Pages

- Home
- Shop
- Women
- Men
- Girls
- Boys
- Product details
- How to Measure Yourself
- Cart
- Checkout
- Order confirmation
- Return and Refund Policy
- About Us
- Contact Us
- FAQ
- Login
- Register
- Forgot Password

### Customer Pages

- Account overview
- Profile
- Saved measurements
- My orders
- Order details

### Admin Pages

- Admin dashboard
- Product list
- Product editor
- Category manager
- Color manager
- Measurement requirements
- Order list
- Order details
- Customer measurement viewer
- Payment status
- Return requests

## UX Direction

The visual direction should feel like a premium African/traditional fashion boutique:

- Strong editorial photography on the homepage and product pages.
- Warm but restrained palette with high contrast and polished typography.
- Product cards that show garment imagery clearly.
- Color swatches rather than text-only color choices.
- Clean multi-step checkout with visible progress.
- Measurement forms grouped by garment need, with concise validation messages.
- Mobile navigation collapsed into a menu.
- Empty, loading, and error states designed intentionally.

Avoid making the UI feel like a generic coding demo. The custom-sewn workflow must be visible early and often.

## Recommended Libraries

### Frontend

- React
- Vite
- React Router
- TypeScript
- Axios or Fetch wrapper
- TanStack Query for API/server state
- React Hook Form for forms
- Zod for client-side validation schemas
- Zustand or React context for cart/session state
- Lucide React for icons
- CSS Modules, plain CSS, or a small design-token layer for styling

### Backend

- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- EF Core SQL Server provider
- FluentValidation
- AutoMapper or explicit mapping classes
- Serilog or built-in structured logging
- Swashbuckle/OpenAPI for API documentation
- BCrypt/Identity password hashing through Identity
- xUnit, FluentAssertions, and WebApplicationFactory for tests

### Infrastructure and Configuration

- SQL Server for local and production database.
- User secrets for local sensitive configuration.
- Environment variables for production secrets.
- Payment provider keys kept out of source control.
- CORS configured for the local frontend origin in development.

## Planned Folder Structure

```text
BILI-Store/
  README.md
  Clothing store.docx
  .gitignore
  .env.example
  docs/
    PHASE_1_ARCHITECTURE.md
    API_CONTRACTS.md
    DATABASE_NOTES.md
  frontend/
    package.json
    index.html
    vite.config.ts
    tsconfig.json
    src/
      main.tsx
      App.tsx
      app/
        routes.tsx
        providers/
      assets/
        images/
      components/
        layout/
        ui/
        product/
        cart/
        checkout/
        measurement/
      features/
        auth/
        account/
        admin/
        cart/
        checkout/
        measurements/
        orders/
        products/
      pages/
        HomePage.tsx
        ShopPage.tsx
        ProductDetailsPage.tsx
        MeasurementGuidePage.tsx
        CartPage.tsx
        CheckoutPage.tsx
        OrderConfirmationPage.tsx
        ReturnPolicyPage.tsx
        LoginPage.tsx
        RegisterPage.tsx
        AccountPage.tsx
        AdminDashboardPage.tsx
      services/
        apiClient.ts
        productsApi.ts
        cartApi.ts
        ordersApi.ts
        paymentsApi.ts
      styles/
        tokens.css
        global.css
      types/
  backend/
    BiliStore.sln
    src/
      BiliStore.Api/
        Controllers/
        Middleware/
        Program.cs
        appsettings.json
        appsettings.Development.json
      BiliStore.Application/
        DTOs/
        Interfaces/
        Services/
        Validators/
        Mapping/
      BiliStore.Domain/
        Entities/
        Enums/
        ValueObjects/
      BiliStore.Infrastructure/
        Data/
        Identity/
        Repositories/
        Payments/
        Delivery/
        Seed/
    tests/
      BiliStore.Api.Tests/
      BiliStore.Application.Tests/
```

## Configuration Plan

Create these examples in Phase 2:

- `frontend/.env.example`
  - `VITE_API_BASE_URL=https://localhost:5001/api`
- `backend/src/BiliStore.Api/appsettings.json`
  - Non-secret defaults only.
- `backend/src/BiliStore.Api/appsettings.Development.json`
  - Local development settings only.
- User secrets or environment variables:
  - SQL Server connection string.
  - JWT signing key.
  - Payment provider API keys.
  - Payment webhook secrets.

## Seed Data Plan

Initial seed data should include:

- Top-level customer categories: Women, Men, Girls, Boys.
- Clothing types from the brief.
- Color swatches: Black, White, Red, Blue, Green, Yellow, Burgundy, Navy, Beige, Brown, Gold.
- Measurement types with guide copy and placeholder illustration URLs.
- Featured products with realistic names, prices, descriptions, image URLs, colors, and measurement requirements.
- One admin user seeded through safe development-only configuration.

## Implementation Roadmap

### Phase 1 - Planning

Completed by this document:

- Requirements analysis.
- Architecture proposal.
- Database/entity model.
- Frontend page map.
- API design.
- Recommended package list.
- Planned folder structure.
- Phase roadmap.

### Phase 2 - Project Setup

- Create Vite React frontend with TypeScript.
- Create ASP.NET Core solution and Web API.
- Add EF Core and SQL Server configuration.
- Configure CORS for local development.
- Add environment variable examples.
- Add initial health-check endpoint.
- Verify both projects run locally.
- Commit setup.

### Phase 3 - Frontend Design Foundation

- Add design tokens and global CSS.
- Build layout, navigation, footer, and responsive shell.
- Create homepage sections from the brief.
- Add shared UI components.
- Use reliable placeholder fashion imagery.
- Commit frontend foundation.

### Phase 4 - Product Catalogue

- Implement product/category/color entities and migrations.
- Seed sample products.
- Build product listing, filters, search, and product details.
- Connect frontend to backend endpoints.
- Commit catalogue.

### Phase 5 - Measurements

- Implement measurement definitions and product-specific requirements.
- Build dynamic measurement forms and validation.
- Build measurement guide page.
- Add saved measurement profiles.
- Commit measurement workflow.

### Phase 6 - Cart

- Implement cart entities and API.
- Persist cart across refreshes.
- Keep product, color, custom color, measurements, quantity, and pricing attached to each cart item.
- Commit cart.

### Phase 7 - Customer Accounts

- Add register, login, logout, JWT auth, account page, saved measurements, and order history shell.
- Commit authentication and account features.

### Phase 8 - Checkout and PAXI/PEP

- Build multi-step checkout.
- Validate customer details, delivery details, measurements, and policy acceptance.
- Add PAXI/PEP delivery details model and UI.
- Commit checkout delivery flow.

### Phase 9 - Payments

- Add payment service abstraction.
- Add development/test payment provider.
- Document PayFast, Peach Payments, or Yoco configuration points.
- Commit payment foundation.

### Phase 10 - Orders

- Create orders from validated carts.
- Snapshot product, customization, measurement, delivery, and payment details.
- Build confirmation and order history.
- Commit order flow.

### Phase 11 - Admin Dashboard

- Add protected admin routes and role checks.
- Add product, category, color, measurement, order, payment, and return request management.
- Commit admin dashboard.

### Phase 12 - Testing and Polish

- Add backend unit and integration tests.
- Add frontend component and workflow tests where useful.
- Review validation, broken routes, broken images, mobile layout, loading states, empty states, and error handling.
- Commit final polish.

## Phase 2 Entry Criteria

Before starting Phase 2, confirm:

- The architecture in this document is acceptable.
- The desired local SQL Server option is available.
- The preferred payment provider for production is known, or the project should continue with the development provider only.
- The brand name should remain BILI Store unless changed by the owner.

