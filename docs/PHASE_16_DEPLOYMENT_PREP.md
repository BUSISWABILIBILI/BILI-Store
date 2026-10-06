# Phase 16 - Deployment Preparation

## Goal

Phase 16 prepares the project for GitHub handoff, hosting setup, and project explanation. This phase does not add a new customer feature. It makes the existing frontend and backend easier to configure safely outside the development machine.

## What Changed

- Added `backend/src/BiliStore.Api/appsettings.Production.example.json`.
- Added `frontend/.env.production.example`.
- Expanded `.env.example` with backend hosting, database, CORS, authentication, and payment variables.
- Updated `.gitignore` so example environment files can be committed while real environment files stay ignored.
- Added `docs/DEPLOYMENT_CHECKLIST.md`.
- Updated the API system information to Phase 16.
- Updated the README with the Phase 16 status and documentation link.

## Why This Matters

Real deployments need different settings from local development:

- The frontend must know the deployed API URL.
- The backend must know the deployed frontend URL for CORS.
- The database connection string must point to the hosted database.
- The JWT signing key must be a strong private secret.
- Payment provider secrets must not be committed to GitHub.

The example files show what values are required without exposing real credentials.

## Backend Configuration

For local development, use:

```powershell
dotnet run --project backend\src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

For production hosting, configure the host with environment variables like:

```powershell
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__BiliStoreDb=your-production-connection-string
Cors__AllowedOrigins__0=https://your-bili-store-domain.co.za
Jwt__SigningKey=your-secure-production-secret
Payments__Provider=Development
```

`Payments__Provider` should stay as `Development` until a real provider is selected and fully integrated.

## Frontend Configuration

For local development:

```env
VITE_API_BASE_URL=http://localhost:5087/api
```

For production:

```env
VITE_API_BASE_URL=https://api.your-bili-store-domain.co.za/api
```

## GitHub Handoff

Before pushing to GitHub, restore the hidden history if needed:

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store"
Remove-Item -LiteralPath .git -Recurse -Force
Rename-Item -LiteralPath .repo-history -NewName .git
git status
git log --oneline
```

Then add the remote and push:

```powershell
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
git branch -M main
git push -u origin main
```

## How To Explain This Phase

This phase is about making the project safe to share and deploy. The code already has frontend and backend features, but deployment needs correct environment variables, secret handling, build checks, and GitHub steps. The new documents give those instructions clearly so the project can be moved from local development to a hosted environment without accidentally committing private credentials.

## Verification

Use these commands:

```powershell
dotnet build backend\BiliStore.sln --no-restore -v:minimal -maxcpucount:1
cd frontend
pnpm lint
pnpm build
```

The known local `pnpm build` limitation is still the Vite/esbuild `spawn EPERM` issue if it appears after TypeScript succeeds.

## Suggested Next Phase

Phase 17 can focus on demo readiness: seed a small demo story, add screenshots, prepare admin login setup notes, and create a project presentation walkthrough.
