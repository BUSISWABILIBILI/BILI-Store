# Deployment Checklist

Use this checklist when preparing BILI Store for GitHub, hosting, or a project presentation.

## 1. Restore The Real Git History

The app sandbox may leave the visible `.git` folder empty while the real history is saved in `.repo-history`.

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store"
Remove-Item -LiteralPath .git -Recurse -Force
Rename-Item -LiteralPath .repo-history -NewName .git
git status
git log --oneline
```

After this, `git log --oneline` should show the phase commits.

## 2. Check Secrets

Do not commit real secrets. Keep these values in the hosting provider's secret manager or local untracked files:

- Database connection string
- JWT signing key
- Payment provider merchant keys
- Payment provider access tokens

Use these templates:

- `.env.example`
- `frontend/.env.example`
- `frontend/.env.production.example`
- `backend/src/BiliStore.Api/appsettings.Production.example.json`

## 3. Backend Verification

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store"
dotnet restore backend\BiliStore.sln
dotnet build backend\BiliStore.sln --no-restore
dotnet run --project backend\src\BiliStore.Api\BiliStore.Api.csproj --launch-profile http
```

Manual checks:

- `GET http://localhost:5087/api/health`
- `GET http://localhost:5087/api/system`
- `GET http://localhost:5087/api/products`

## 4. Frontend Verification

```powershell
cd "C:\Users\BusiswaB\Desktop\GitHub Projects\PORTFOLIO\BILI-Store\frontend"
pnpm install
pnpm lint
pnpm dev
```

Open `http://localhost:5173`.

`pnpm build` may fail on this machine with the known Vite/esbuild `spawn EPERM` issue. If that happens after TypeScript succeeds, the failure is local tooling permission related.

## 5. Production Settings

Backend:

- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Set `ConnectionStrings__BiliStoreDb`.
- Set `Jwt__SigningKey` to a strong secret.
- Set `Cors__AllowedOrigins__0` to the deployed frontend URL.
- Change `Payments__Provider` when a real payment integration is selected.

Frontend:

- Set `VITE_API_BASE_URL` to the deployed API `/api` URL.

## 6. GitHub Push

```powershell
git status
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
git branch -M main
git push -u origin main
```

If a remote already exists, use:

```powershell
git remote -v
git remote set-url origin https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
git push -u origin main
```

## 7. Presentation Notes

You can explain the project as:

"BILI Store is a full-stack custom clothing ordering system. Customers browse traditional garments, choose colors, enter measurements, review delivery details, accept the return policy, and create orders. Admin users manage orders, catalogue data, delivery status, returns, customers, dashboard metrics, and CSV reports."
