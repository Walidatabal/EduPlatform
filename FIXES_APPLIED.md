# EduPlatform — Fixes Applied (2026-05-17)

## What was fixed in this build

| File | Fix |
|------|-----|
| `EduPlatform.Web/Views/_ViewImports.cshtml` | Added `@using EduPlatform.Domain.Constants` so `AppRoles.*` resolves in Razor views and the sidebar Access Control links appear |
| `EduPlatform.Web/appsettings.json` | Fixed `AdminPassword` typo: `Admin@12345` → `Admin@123456` |
| `EduPlatform.Web/appsettings.Development.json` | Same password fix |
| `EduPlatform.API/appsettings.json` | Replaced literal `${ADMIN_PASSWORD}` (invalid in JSON) with `Admin@123456` |
| `EduPlatform.API/appsettings.Development.json` | **New file** — adds `localhost,1433` connection string so the API runs with `dotnet run` outside Docker |
| `EduPlatform.Web/Controllers/AccountController.cs` | Added `RoleSortOrder()` helper; roles now sorted Admin→ContentManager→Teacher→PendingTeacher→Parent→Student everywhere |
| `EduPlatform.Web/Views/Account/Profile.cshtml` | Role badges now use per-role colour classes |
| `EduPlatform.Web/wwwroot/css/site.css` | Added `.badge-role .badge-admin .badge-manager` etc colour rules |

## Root cause of admin "Invalid" login

Docker Compose seeds the admin password as `Admin@123456`.
`EduPlatform.Web/appsettings.json` had `Admin@12345` (missing the trailing `6`).
The stored hash never matched. Now both use the same password.

## Quick unlock (if account is still locked in SSMS)

```sql
UPDATE AspNetUsers
SET LockoutEnd = NULL, AccessFailedCount = 0, LockoutEnabled = 0
WHERE Email = 'admin@eduplatform.com';
```

## Connection string rules

| Where | Server name | Reason |
|-------|-------------|--------|
| `appsettings.json` (API) | `sqlserver,1433` | Docker Compose internal DNS |
| `appsettings.Development.json` (API + Web) | `localhost,1433` | Local `dotnet run` |
| Docker Compose env vars | `sqlserver,1433` | Always overrides JSON at runtime |
