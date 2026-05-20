# EduPlatform Enterprise LMS

> **Production-grade Learning Management System** built with ASP.NET Core 8, Clean Architecture, EF Core 8, SQL Server, Docker, Redis, Serilog + Seq, JWT API authentication, and ASP.NET Core Identity cookie authentication for the MVC portal.
>
> Designed as a professional portfolio project and technical interview reference.

---

## Current Status — Last Tested Build (May 2026)

| Area | Status |
|---|---|
| Clean Architecture | ✅ Strong |
| API Layer (JWT + Swagger) | ✅ Strong |
| MVC Web Layer | ✅ Strong |
| Identity / Auth | ✅ Strong |
| Docker Compose | ✅ Working |
| Role-Based Access Control | ✅ Working |
| Refresh Token Support | ✅ Added |
| Notifications Module | ✅ Implemented Foundation |
| Email Service | ⚠️ Stub / production provider pending |
| GitHub Actions CI | ✅ Added |
| Unit Tests (40+) | ✅ Passing |
| Production Deployment | ⏳ Pending |
| Portfolio Score | **8.5 / 10** |

See also: [`docs/FINAL_TESTED_STATUS_AND_PENDING_TASKS.md`](docs/FINAL_TESTED_STATUS_AND_PENDING_TASKS.md)

---|---|
| Clean Architecture | ✅ Strong |
| API Layer (JWT + Swagger) | ✅ Strong |
| MVC Web Layer | ✅ Strong |
| Identity / Auth | ✅ Strong |
| Docker Compose | ✅ Working |
| Role-Based Access Control | ✅ Working |
| Refresh Token Support | ✅ Added |
| GitHub Actions CI | ✅ Added |
| Unit Tests (40+) | ✅ Passing |
| Portfolio Score | **8.2 / 10** |

---

## Architecture

```
EduPlatform.Domain          ← Entities, enums, IRepository contracts
        ↑
EduPlatform.Application     ← DTOs, commands, validators, result patterns
        ↑
EduPlatform.Infrastructure  ← EF Core, Identity, repositories, seeders, services
        ↑                ↑
EduPlatform.API        EduPlatform.Web
(JWT REST API)         (Cookie MVC Portal)

EduPlatform.Tests           ← xUnit unit tests
```

### Projects

| Project | Responsibility |
|---|---|
| `EduPlatform.Domain` | Entities, enums, constants, repository contracts |
| `EduPlatform.Application` | DTOs, commands, validators, AutoMapper, service interfaces |
| `EduPlatform.Infrastructure` | EF Core, Identity, repositories, UnitOfWork, services, seeders |
| `EduPlatform.API` | REST API, JWT, Swagger, middleware, rate limiting, versioning |
| `EduPlatform.Web` | MVC portal, Razor views, cookie auth, ViewModels, bilingual UI |
| `EduPlatform.Tests` | Unit tests for domain, application, and web layers |

---

## Technology Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 8 (.NET 8 LTS) |
| Language | C# 12 with nullable reference types |
| ORM | Entity Framework Core 8 (code-first) |
| Database | SQL Server 2022 |
| Auth — API | JWT Bearer (stateless, 24h expiry) |
| Auth — Web | ASP.NET Core Identity Cookie (sliding 8h) |
| Cache | Redis 7 via StackExchange.Redis (in-memory fallback) |
| Logging | Serilog → Console + Rolling File + Seq |
| Containers | Docker Compose (5 services, 2 named volumes) |
| Tests | xUnit — 40+ tests |
| Rate Limiting | 100 req/min per user (PartitionedRateLimiter) |
| API Versioning | URL segment /api/v1/ + x-api-version header |

---

## Quick Start

### Option A — Docker (Recommended — Everything in Containers)

```powershell
# Clone or extract the project
cd edu_final_update

# Start all 5 services (SQL Server, Redis, Seq, API, Web)
docker compose up -d

# OR for a full clean rebuild:
docker compose down --remove-orphans
docker compose up --build
```

**Open in browser:**

| Service | URL |
|---|---|
| API Swagger | http://localhost:8080 |
| MVC Web Portal | http://localhost:8081 |
| Seq Log Viewer | http://localhost:8088 |
| Health Check | http://localhost:8080/health |

---

### Option B — Local Development (Visual Studio + Docker for SQL/Redis/Seq)

```powershell
# Start only infrastructure containers
docker compose up -d sqlserver redis seq

# Run the API (seeds DB on first run)
dotnet run --project EduPlatform.API

# Run the Web portal (in a separate terminal)
dotnet run --project EduPlatform.Web
```

**Local URLs:**
- API Swagger: http://localhost:5000
- MVC Web: http://localhost:5001

> **Important:** Local mode uses `Server=localhost,1433`. Docker mode uses `Server=sqlserver,1433`. Do not mix them.

---

## Default Accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@eduplatform.com` | `Admin@123456` |
| Teacher | `teacher1@edu.com` | `Teacher@123` |
| Student | `student1@edu.com` | `Student@123` |
| Parent | `parent1@edu.com` | `Parent@123` |

> The AdminSeeder resets the admin account (password + lockout) on **every startup** to ensure you can always log in.

---

## Connection String Rules

| Context | Server Name | Location |
|---|---|---|
| `dotnet run` locally | `Server=localhost,1433` | appsettings.Development.json |
| Docker container | `Server=sqlserver,1433` | Compose env var override |
| Production | Your cloud DB host | Secrets manager / env var |

---

## EF Core Commands

If you applied this update to an existing database, run a new migration for the refresh token fields:

```powershell
dotnet ef migrations add AddRefreshTokenFields \
  --project EduPlatform.Infrastructure \
  --startup-project EduPlatform.API

dotnet ef database update \
  --project EduPlatform.Infrastructure \
  --startup-project EduPlatform.API
```

List all migrations:
```powershell
dotnet ef migrations list --startup-project EduPlatform.API
```

---

## Swagger JWT Testing

1. Open http://localhost:8080
2. Run `POST /api/Auth/login` with admin credentials
3. Copy the `accessToken` value only
4. Click **Authorize** (top right)
5. Paste the token — do **not** type `Bearer` (Swagger adds it automatically)
6. Execute any protected endpoint

| Code | Meaning |
|---|---|
| 200 / 201 | Authenticated and authorized |
| 401 | Not authenticated or token expired |
| 403 | Authenticated but wrong role |
| 429 | Rate limit exceeded (100 req/min) |

---

## Role & Access Control

| Role | Key Permissions |
|---|---|
| **Admin** | Everything — manage users, approve teachers/courses, reset passwords, unlock accounts |
| **ContentManager** | View users, approve/reject teachers and courses, unlock accounts, view Pending Tasks |
| **Teacher** | Create/edit own courses, submit for approval, create live sessions |
| **PendingTeacher** | Register, log in, view dashboard — awaiting Admin approval |
| **Student** | Browse, cart, enrol, track progress, review courses, earn certificates |
| **Parent** | View dashboard, see linked students' learning data |

### Teacher Approval Workflow
`Register as PendingTeacher` → `Admin approves in Pending Tasks` → `Becomes Teacher`

### Course Publication Workflow
`Teacher creates Draft` → `Submits` → `Admin approves` → `Published & visible in catalog`

---

## Features

### Authentication & Authorization
- ASP.NET Core Identity
- JWT Bearer for API + Cookie for MVC
- Role-based authorization (6 roles)
- Account lockout (5 attempts → 15-minute lockout)
- Admin unlock account
- Admin reset user password
- User change password + profile update
- Refresh token support (`/api/Auth/refresh`, `/api/Auth/revoke`)
- Forgot/reset password API endpoints

### LMS Features
- Courses, Grades, Subjects, Categories
- Sections and Lessons
- Enrollments, Cart, Wishlist
- Orders and Payments (gateway placeholder)
- Coupons (percentage + fixed amount)
- Reviews, Lesson Progress, Certificates
- Live Sessions, Notifications
- Questions and Answers
- Parent/Student Relations
- Teacher/Course Relations

### Enterprise Features
- Clean Architecture (Domain → Application → Infrastructure → API + Web)
- Repository + Unit of Work pattern
- DTOs and ViewModels (no entities in views)
- AutoMapper + FluentValidation
- Global exception middleware (JSON error responses)
- Standard validation filter (400 with field errors)
- Serilog structured logging (Console + File + Seq)
- Docker Compose (5 services + 2 named volumes)
- SQL Server 2022 with EF Core migrations
- Redis cache with in-memory fallback
- Health checks (`/health`)
- Swagger / OpenAPI with JWT authorization
- API versioning (`/api/v1/`)
- Rate limiting (100 req/min per user)
- Soft delete (all entities, never hard-deleted)
- Audit trail (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy on all entities)
- Bilingual UI (English + Arabic RTL)
- GitHub Actions CI workflow

---

## Database Protection During Maintenance

### The Golden Rule
```powershell
# ❌ NEVER — deletes all data
docker compose down -v

# ✅ SAFE — keeps all data
docker compose down
docker compose restart
```

### Backup Before Any Maintenance

**Via SSMS (easiest):**
Right-click `EduPlatformDb` → Tasks → Back Up → save .bak file to your PC

**Via PowerShell:**
```powershell
docker exec -it eduplatform_sqlserver /opt/mssql-tools/bin/sqlcmd `
  -S localhost -U sa -P "Dev_Password123!" `
  -Q "BACKUP DATABASE EduPlatformDb TO DISK='/var/opt/mssql/backup/EduPlatformDb.bak' WITH FORMAT"

docker cp eduplatform_sqlserver:/var/opt/mssql/backup/EduPlatformDb.bak ./EduPlatformDb.bak
```

---

## Troubleshooting

### SQL Error 10054 — Connection Forcibly Closed
```powershell
# SQL Server not running or still starting up
docker ps
docker compose up -d sqlserver
# Wait 30 seconds, then restart app
docker compose restart api web
```

### Admin Password Not Working / Account Locked
```sql
UPDATE AspNetUsers
SET LockoutEnd = NULL, AccessFailedCount = 0, LockoutEnabled = 0, EmailConfirmed = 1
WHERE Email = 'admin@eduplatform.com';
```
Or simply run the API once — AdminSeeder automatically resets the admin account on startup.

### Docker API Pipe Error
Open Docker Desktop and wait for the green "Engine Running" icon in the system tray.

### AutoMapper Version Conflict
All projects must use AutoMapper `15.0.1`. Verify all `.csproj` files match.

### Swagger Returns 401
Token may be expired (24h lifetime). Re-login via `POST /api/Auth/login` and paste only the token — do not include `Bearer`.

### Docker Container Name Conflict
```powershell
docker rm -f eduplatform_sqlserver eduplatform_seq eduplatform_redis eduplatform_api eduplatform_web
docker compose up -d
```

---

## What Is Required Before Going to Production

| Priority | Requirement |
|---|---|
| 🔴 Critical | Move all secrets to environment variables or Azure Key Vault / AWS Secrets Manager |
| 🔴 Critical | Replace EmailService stub with SendGrid / Mailgun / SMTP |
| 🔴 Critical | Enable HTTPS / SSL certificate everywhere |
| 🔴 Critical | Disable AdminSeeder automatic password reset in production |
| 🔴 Critical | Set specific CORS origins (remove AllowAnyOrigin) |
| 🟡 High | Enable email confirmation on registration |
| 🟡 High | Implement refresh token rotation + revocation on password change |
| 🟡 High | Add real payment gateway (Stripe / PayTabs) |
| 🟡 High | Add [ValidateAntiForgeryToken] to all MVC POST actions |
| 🟡 High | Add integration tests with Testcontainers |
| 🟡 High | Add audit log table for admin actions |
| 🟠 Medium | Add object storage for video/lesson files (Azure Blob / S3) |
| 🟠 Medium | Add permission-based authorization (beyond role-based) |
| 🟠 Medium | Add MFA for Admin and ContentManager roles |
| 🟠 Medium | Protect Seq with admin password — never expose publicly |

---

## Documentation

| File | Contents |
|---|---|
| `docs/FINAL_ARCHITECTURE_GUIDE_v5.md` | Full architecture reference |
| `docs/FINAL_IMPLEMENTATION_REPORT.md` | What was implemented and fixed |
| `docs/FINAL_INSPECTION_AND_TROUBLESHOOTING.md` | Inspection report + troubleshooting |
| `docs/PRODUCTION_SECURITY_CHECKLIST.md` | Security hardening for production |
| `docs/DEPLOYMENT_CHECKLIST.md` | Step-by-step deployment procedure |
| `docs/TESTING_EXPANSION_GUIDE.md` | How to expand the test suite |
| `docs/WEAKNESS_IMPROVEMENTS_APPLIED.md` | What was improved in this version |
| `docs/REFRESH_TOKEN_MIGRATION_GUIDE.md` | Refresh token DB migration steps |
| `.env.example` | Template for all required environment variables |
| `.github/workflows/ci.yml` | GitHub Actions CI: restore → build → test |

---

## Key File Locations

| File | Purpose |
|---|---|
| `EduPlatform.Infrastructure/DependencyInjection.cs` | All DI registrations, Identity + lockout + cookie config |
| `EduPlatform.Infrastructure/Data/AppDbContext.cs` | DbSets, SaveChangesAsync audit + soft-delete override |
| `EduPlatform.Infrastructure/Seeders/DbSeeder.cs` | Master seeder — migrations + full data seed pipeline |
| `EduPlatform.Infrastructure/Services/Lms/LmsPlatformService.cs` | All LMS business logic hub |
| `EduPlatform.API/Program.cs` | API middleware pipeline, rate limiter, CORS, Redis, versioning |
| `EduPlatform.API/Middleware/ExceptionMiddleware.cs` | Global exception → HTTP status code mapping |
| `EduPlatform.Web/Controllers/AccountController.cs` | Login, register, profile, admin user + role management |
| `EduPlatform.Web/Controllers/DashboardController.cs` | Role-aware dashboard data queries |
| `docker-compose.yml` | Container definitions, env vars, health checks, volumes |
| `EduPlatform.Domain/Constants/AppRoles.cs` | Role name string constants used across all projects |

---

## Running Tests

```powershell
dotnet test

# With coverage:
dotnet test --collect:"XPlat Code Coverage"
```

Test coverage includes: BaseEntity defaults, Result factory methods, LMS DTOs, Checkout requests, ViewModels, and all 8 domain enum values (22 enum tests).

---

## Production Notes

Before any public or production deployment:

1. Move all secrets to environment variables or a cloud secrets manager
2. Replace the email stub with a real SMTP / SendGrid / Mailgun provider
3. Enable HTTPS everywhere with a valid SSL certificate
4. Disable the AdminSeeder automatic password reset
5. Replace development passwords across all seeded accounts
6. Set production CORS origins — remove AllowAnyOrigin
7. Enable email confirmation on registration
8. Add real payment gateway integration
9. Add object storage for lesson files and videos
10. Add integration tests with Testcontainers + WebApplicationFactory
11. Add audit log table for admin actions
12. Enable database backups on a schedule
13. Add monitoring and alerting

---

*EduPlatform Enterprise LMS — Final Production Build · May 2026*
*ASP.NET Core 8 · Clean Architecture · EF Core 8 · SQL Server 2022 · Redis 7 · Serilog · Docker Compose · xUnit*

---

## Production Readiness Update

This repository is prepared for professional deployment with:

- safe `appsettings.json` templates
- `.env.example` for Docker secrets
- Docker Compose stack
- SQL Server persistent volume
- Redis
- Seq logging
- JWT + Refresh Tokens
- production migration documentation

### Do not commit secrets

The following files must stay out of GitHub:

```text
.env
appsettings.Development.json
appsettings.Production.json
*.secrets.json
```

### Local Docker run

```powershell
copy .env.example .env
# edit .env values
powershell ./scripts/docker-ready.ps1
```

### Preflight check

```powershell
powershell ./scripts/preflight.ps1
```

### Main URLs

```text
API Swagger: http://localhost:8080/swagger
MVC Web:     http://localhost:8081
Seq Logs:    http://localhost:8088
Health:      http://localhost:8080/health
```
