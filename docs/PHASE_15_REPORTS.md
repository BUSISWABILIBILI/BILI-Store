# Phase 15 - Admin Reporting Exports

## Goal

Phase 15 adds reporting polish for store administrators. The admin dashboard can now be filtered by a date range, and admins can export order and customer reports as CSV files for spreadsheet review.

## Backend Changes

- `AdminDashboardController` now accepts optional `dateFrom` and `dateTo` query parameters.
- Dashboard order totals, paid revenue, pending revenue, guest order counts, status summaries, recent orders, and return counts now use the selected date range.
- Catalogue and customer profile totals remain global because they describe the current store setup, not only the selected reporting window.
- `AdminReportsController` adds protected CSV endpoints:
  - `GET /api/admin/reports/orders.csv`
  - `GET /api/admin/reports/customers.csv`
- Report downloads use the same optional `dateFrom` and `dateTo` query parameters as the dashboard.

## Frontend Changes

- The admin dashboard now includes a reporting range panel with `From` and `To` date inputs.
- Changing the range refreshes the dashboard query with those dates.
- The admin can export:
  - Orders CSV
  - Customers CSV
- CSV downloads use the authenticated API client, so admin authorization still applies.

## API Examples

```http
GET /api/admin/dashboard?dateFrom=2026-09-01&dateTo=2026-09-30
Authorization: Bearer {{accessToken}}
Accept: application/json
```

```http
GET /api/admin/reports/orders.csv?dateFrom=2026-09-01&dateTo=2026-09-30
Authorization: Bearer {{accessToken}}
Accept: text/csv
```

```http
GET /api/admin/reports/customers.csv?dateFrom=2026-09-01&dateTo=2026-09-30
Authorization: Bearer {{accessToken}}
Accept: text/csv
```

## How To Explain This Phase

The reporting feature works by passing a date range from the React dashboard to the ASP.NET API. The API applies that range to order dates and return request dates before calculating dashboard totals or building CSV rows. The frontend downloads the CSV as a Blob because the request must include the logged-in admin user's JWT token.

## Verification

Use these commands from the project folder:

```powershell
dotnet build backend\BiliStore.sln --no-restore -v:minimal -maxcpucount:1
cd frontend
pnpm lint
pnpm build
```

`pnpm build` may still fail in this local environment with the known Vite/esbuild `spawn EPERM` issue. That is an environment execution problem rather than a TypeScript compile error.

## Suggested Next Phase

Phase 16 can add production deployment preparation: environment examples, seeded admin setup notes, database migration instructions, and a final GitHub-ready project checklist.
