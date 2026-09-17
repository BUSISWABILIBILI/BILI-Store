# Phase 3 - Frontend Design Foundation

## What This Phase Added

Phase 3 turns the basic React shell into the first polished customer-facing storefront design.

It adds:

- A generated local hero image for the boutique visual identity.
- A more professional navigation and footer.
- A premium homepage layout.
- Customer category tiles for Women, Men, Girls, and Boys.
- Featured product preview cards.
- A custom ordering process section.
- A measurement guide layout with step-by-step cards.
- A stronger shop page visual foundation.
- A styled cart empty state that explains what the cart will preserve later.
- Shared data and reusable components so pages do not become messy.

This phase is still frontend-only. Phase 4 will connect product browsing to real backend endpoints.

## Generated Visual Asset

The project now contains a local generated image:

`frontend/public/images/bili-atelier-hero.png`

The frontend references it as:

`/images/bili-atelier-hero.png`

This is better than using a random remote image URL because the page can render reliably even when working offline or before final business photos are ready.

## New Frontend Data File

`frontend/src/data/storefront.ts`

This file holds the structured display data used by the homepage and shop preview:

- `heroImage`: the local visual asset path.
- `storeCategories`: Women, Men, Girls, and Boys category tiles.
- `featuredProducts`: temporary product preview data for the design foundation.
- `orderingSteps`: the custom-ordering journey.
- `measurementGuideItems`: measurement guide content.

This matters because the page components read from clean data instead of hard-coding everything inside the JSX layout.

## New Reusable Components

`frontend/src/components/ui/SectionHeader.tsx`

Creates consistent section headings with an eyebrow label, title, and optional description.

`frontend/src/components/home/CategoryTile.tsx`

Displays one category tile. It uses the shared hero image as a cropped background and links to the shop with a category query string.

`frontend/src/components/home/FeaturedProductCard.tsx`

Displays one product preview card with category, clothing type, price, sewing time, required measurements, colour swatches, and a detail link placeholder.

`frontend/src/components/home/OrderingStepCard.tsx`

Displays one step in the custom ordering journey.

`frontend/src/utils/formatters.ts`

Formats prices as South African Rand using `Intl.NumberFormat`.

## Updated Pages

`frontend/src/pages/HomePage.tsx`

The homepage now includes:

- Hero section with BILI Store branding.
- Shop Women / Men / Girls / Boys category area.
- Custom fit feature explanation.
- Featured product preview cards.
- Custom ordering steps.
- Measurement callout.
- Trust/service highlights.
- Newsletter placeholder.
- Backend API status panel.

`frontend/src/pages/ShopPage.tsx`

The shop page now has:

- A proper catalogue hero.
- Search and filter control styling.
- Category pills.
- Product card preview grid.

The controls are visual shells for now. Phase 4 will connect them to API data.

`frontend/src/pages/MeasurementGuidePage.tsx`

The measurement guide now has:

- Introductory measurement instructions.
- A measurement illustration placeholder.
- Cards for bust/chest, waist, hips, and shoulder.
- Step-by-step guidance and tips.

`frontend/src/pages/CartPage.tsx`

The cart page now explains the data that must remain attached to a custom clothing order:

- Selected colour.
- Required measurements.
- Garment and quantity.

## Updated Shared Layout

`frontend/src/components/layout/AppLayout.tsx`

The layout now separates:

- Primary navigation: Shop, Measurements, Return Policy.
- Utility actions: Account and Cart icon buttons.
- Footer links: Shop, Measurements, Return Policy, Checkout.

This keeps the main navigation clean while preserving access to account and cart features.

## Updated Styling

`frontend/src/styles/tokens.css`

Defines the visual tokens:

- Brand colours.
- Accent colours.
- Surface, paper, border, and text colours.
- Font stacks.
- Shadows.

`frontend/src/styles/global.css`

Defines:

- Base typography.
- Header and footer layout.
- Hero image treatment.
- Responsive category, product, step, trust, measurement, and cart grids.
- Buttons, links, cards, forms, and mobile navigation.

The CSS uses fixed responsive breakpoints instead of viewport-scaled font sizes so text stays predictable on desktop and mobile.

## How To Explain The Design Approach

The frontend is being built like a real boutique storefront, not a quick demo.

The important idea is:

- `storefront.ts` stores display data.
- Small reusable components render each kind of UI block.
- Pages compose those blocks into full screens.
- Styling is centralized in design tokens and global layout rules.

This makes it easier to replace preview data with backend API data in later phases without redesigning the page.

## How To Run And Test

From:

`C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend`

Run:

```powershell
pnpm install
pnpm dev
```

Then open:

`http://127.0.0.1:5173`

Type-check:

```powershell
pnpm lint
```

Production build:

```powershell
pnpm build
```

## Verification Results

`pnpm lint` passed. This means the TypeScript imports, component props, and JSX compile correctly.

`pnpm build` still fails inside the Codex sandbox because Windows blocks the `esbuild` child process with `spawn EPERM`. This is the same environment limitation from Phase 2, not a TypeScript error in the Phase 3 code.

On a normal PowerShell session outside the sandbox, `pnpm dev` and `pnpm build` should use the same project files.

## What Comes Next

Phase 4 should implement the real product catalogue:

- Backend product/category endpoints.
- Frontend API services for products.
- Product search.
- Category filtering.
- Clothing type filtering.
- Price filtering.
- Product details page.
- Product images and colour selection connected to backend data.
