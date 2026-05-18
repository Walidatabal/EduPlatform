# EduPlatform — Changelog

## [Final Production Build] — 2026-05-18

### Bug Fixes (Build-Breaking)
- **DbSeeder.cs** — Replaced `await db.Database.GetMigrationsAsync()` with synchronous
  `db.Database.GetMigrations().Any()`. `GetMigrationsAsync` is an extension method that
  requires `Microsoft.EntityFrameworkCore.Relational` to be explicitly resolved; the sync
  overload lives directly on `DatabaseFacade` and always compiles.
- **EduPlatform.Infrastructure.csproj** — Added explicit `PackageReference` for
  `Microsoft.EntityFrameworkCore.Relational 8.0.15` to prevent the above from regressing.

### Bug Fixes (Runtime)
- **AccountController / appsettings** — Admin login was failing with "Invalid email or
  password" because `Web/appsettings.json` had `Admin@12345` (missing a `6`) while Docker
  Compose seeds with `Admin@123456`. Password aligned across all config files.
- **`_ViewImports.cshtml`** — Added `@using EduPlatform.Domain.Constants` so `AppRoles.*`
  resolves in Razor. Without it the sidebar Access Control and Pending Tasks links were
  silently hidden because the `@if` block around them evaluated as a compile error.
- **`API/appsettings.json`** — Replaced `${ADMIN_PASSWORD}` literal (shell syntax, never
  interpolated in JSON) with `Admin@123456`.
- **`API/appsettings.Development.json`** — Was missing. API failed to start with
  `dotnet run` because `appsettings.json` uses `Server=sqlserver,1433` (Docker DNS);
  the Dev override now uses `Server=localhost,1433`.
- **`Web/appsettings.Docker.json`** — Added missing Docker-specific config file for the
  Web container.

### Features Completed
- **DbSeeder.cs** — Wired `ParentSeeder` and `ParentStudentLinkSeeder` which were defined
  but never called, so parent/student relation data was never seeded.
- **AccountController.cs** — Added `RoleSortOrder()` helper; roles now display in priority
  order (Admin → ContentManager → Teacher → PendingTeacher → Parent → Student) everywhere.
- **Profile.cshtml** — Role badges now use distinct per-role colour classes.
- **site.css** — Added `.badge-role`, `.badge-admin` … `.badge-student` colour rules.
  Added `.summary-row` for the checkout summary layout.
- **DependencyInjection.cs** — Added Identity lockout policy (5 attempts = 15 min),
  cookie path configuration (`/Account/Login`), and sliding expiration (8 h).
- **Web/Program.cs** — Added `CookiePolicyOptions` registration and `UseCookiePolicy()`
  middleware with environment-aware `Secure` flag.

### Tests
- Added `EduPlatform.Tests/Unit/Infrastructure/LmsDtoTests.cs` — 8 DTO record tests.
- Added `EduPlatform.Tests/Unit/Infrastructure/CheckoutRequestTests.cs` — 2 tests.
- Added `EduPlatform.Tests/Unit/Web/ViewModelTests.cs` — 8 ViewModel default-state tests.
- Added `EduPlatform.Tests/Unit/Domain/EnumTests.cs` — 22 enum value tests.
- Updated `EduPlatform.Tests.csproj` — Added Web project reference and `coverlet.collector`.

### Configuration (Connection String Rules)
| Context              | Server name       | Reason                              |
|----------------------|-------------------|-------------------------------------|
| `dotnet run` locally | `localhost,1433`  | SQL on dev machine / Docker port    |
| Docker container     | `sqlserver,1433`  | Docker Compose internal DNS         |
| Compose env override | `sqlserver,1433`  | Always overrides JSON at runtime    |

### Default Credentials
| Account | Email | Password |
|---------|-------|----------|
| Admin | admin@eduplatform.com | Admin@123456 |
| Teachers | teacher1@edu.com … | Teacher@123 |
| Students | student1@edu.com … | Student@123 |
| Parents | parent1@edu.com … | Parent@123 |

### Quick Start
```powershell
# Local (SQL Server already running on localhost:1433)
cd EduPlatform
dotnet run --project EduPlatform.API    # seeds DB, serves API  → http://localhost:5000
dotnet run --project EduPlatform.Web   # serves MVC            → http://localhost:5001

# Docker Compose (everything in containers)
docker compose down -v
docker compose build --no-cache
docker compose up -d
# API → http://localhost:8080   Web → http://localhost:8081   Seq → http://localhost:8088
```
