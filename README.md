# EduPlatform Enterprise LMS

EduPlatform is an enterprise-style Learning Management System built with **ASP.NET Core 8**, **Clean Architecture**, **Entity Framework Core 8**, **SQL Server**, **Docker**, **Redis**, **Serilog + Seq**, **JWT API authentication**, and **ASP.NET Core Identity cookie authentication** for the MVC portal.

This repository is designed as a professional portfolio project and technical interview reference. It demonstrates real-world backend architecture, layered design, authentication, authorization, Docker infrastructure, logging, validation, DTO/ViewModel separation, and LMS business workflows.

---

## Current Final Status

This final update includes:

- API + MVC separation
- SQL Server integration for local and Docker modes
- Swagger with JWT authorization
- Seq structured logging
- Redis cache registration with memory fallback
- ASP.NET Core Identity
- JWT access tokens
- Refresh token support added at code level
- Account/profile management
- Change password flow fixed
- Admin user access control
- Unlock account
- Reset user password by admin
- Pending tasks workflow
- Parent/student and teacher/course relations
- Arabic/English UI foundation
- Docker Compose setup
- GitHub Actions CI workflow
- Final architecture and troubleshooting documentation

> Important: Refresh token columns were added to `ApplicationUser`. If you apply this update to an existing database, create and apply a new EF Core migration.

---

## Architecture

```text
EduPlatform.Domain
   ↑
EduPlatform.Application
   ↑
EduPlatform.Infrastructure
   ↑
EduPlatform.API        EduPlatform.Web
```

### Projects

| Project | Responsibility |
|---|---|
| `EduPlatform.Domain` | Entities, enums, constants, repository contracts |
| `EduPlatform.Application` | DTOs, commands, validators, result models, interfaces |
| `EduPlatform.Infrastructure` | EF Core, Identity, repositories, UnitOfWork, services, seeders |
| `EduPlatform.API` | REST API, JWT, Swagger, middleware, rate limiting |
| `EduPlatform.Web` | MVC portal, Razor views, cookie auth, ViewModels, bilingual UI |
| `EduPlatform.Tests` | Unit tests for domain/application/viewmodels |

---

## Main Features

### Authentication and Authorization

- ASP.NET Core Identity
- JWT Bearer authentication for API
- Cookie authentication for MVC
- Role-based authorization
- Admin / ContentManager / Teacher / PendingTeacher / Student / Parent roles
- Account lockout
- Admin unlock account
- Admin reset password
- User change password
- User profile update
- Refresh token support for API clients

### LMS Features

- Courses
- Grades and subjects
- Categories
- Sections and lessons
- Enrollments
- Cart
- Wishlist
- Orders
- Payments placeholder
- Coupons
- Reviews
- Lesson progress
- Certificates
- Live sessions
- Notifications
- Questions and answers
- Parent/student relation
- Teacher/course relation

### Enterprise Features

- Clean Architecture
- Repository + UnitOfWork
- DTOs and ViewModels
- AutoMapper
- FluentValidation
- Global exception middleware
- Standard validation filter
- Serilog structured logging
- Seq log viewer
- Docker Compose
- SQL Server 2022
- Redis cache registration
- Health checks
- Swagger/OpenAPI
- API versioning
- Rate limiting

---

## Local Development

Use this when running from Visual Studio or `dotnet run`.

### Connection string

```json
"DefaultConnection": "Server=localhost,1433;Database=EduPlatformDb;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True"
```

Reason: the app runs on Windows/host and connects to SQL through the host port `localhost:1433`.

### Run commands

```powershell
dotnet restore
dotnet build
dotnet run --project EduPlatform.API
dotnet run --project EduPlatform.Web
```

---

## Docker Development

Use this when running API/Web/SQL/Seq/Redis as containers.

### Docker connection string

```json
"DefaultConnection": "Server=sqlserver,1433;Database=EduPlatformDb;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True"
```

Reason: containers communicate by Docker service name. The SQL Server service is named `sqlserver`.

### Docker commands

```powershell
docker compose up -d
```

Full reset:

```powershell
docker compose down -v --remove-orphans
docker compose up --build
```

---

## Default URLs

| Service | URL |
|---|---|
| API Swagger local/Docker | `http://localhost:8080` |
| MVC Web Docker | `http://localhost:8081` |
| SQL Server Docker | `localhost,1433` |
| Seq | `http://localhost:8088` |
| Health Check | `/health` |

---

## Default Accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@eduplatform.com` | `Admin@123456` |
| Teacher | `teacher1@edu.com` | `Teacher@123` |
| Student | `student1@edu.com` | `Student@123` |
| Parent | `parent1@edu.com` | `Parent@123` |

---

## EF Core Commands

Add migration:

```powershell
dotnet ef migrations add AddRefreshTokenFields --project EduPlatform.Infrastructure --startup-project EduPlatform.API
```

Apply migration:

```powershell
dotnet ef database update --project EduPlatform.Infrastructure --startup-project EduPlatform.API
```

---

## Swagger JWT Test

1. Open Swagger.
2. Run `POST /api/Auth/login`.
3. Copy only the `accessToken`.
4. Click `Authorize`.
5. Paste token only, without typing `Bearer`.
6. Execute protected endpoints.

Expected results:

| Code | Meaning |
|---|---|
| 200/201 | Authenticated and authorized |
| 401 | Not authenticated or invalid token |
| 403 | Authenticated but role not allowed |

---

## Important Troubleshooting

### SQL error: target machine actively refused connection

Cause: SQL Server container is not running or port 1433 is unavailable.

Fix:

```powershell
docker ps
docker compose up -d sqlserver
```

### Docker API pipe error

Cause: Docker Desktop is closed.

Fix: open Docker Desktop and wait until Engine Running.

### Admin password not working

Cause: stale database/volume or locked account.

Fix:

```sql
UPDATE AspNetUsers
SET LockoutEnd = NULL,
    AccessFailedCount = 0,
    LockoutEnabled = 0,
    EmailConfirmed = 1
WHERE Email = 'admin@eduplatform.com';
```

### AutoMapper package downgrade

All projects must use the same AutoMapper version, currently `15.0.1`.

### Change Password button returns to profile

Fixed by moving Change Password outside the profile POST form and using a GET form/button.

---

## Documentation

See:

- `docs/FINAL_ARCHITECTURE_GUIDE_v5.md`
- `docs/FINAL_IMPLEMENTATION_REPORT.md`
- `docs/FINAL_INSPECTION_AND_TROUBLESHOOTING.md`
- `docs/PRODUCTION_SECURITY_CHECKLIST.md`
- `docs/DEPLOYMENT_CHECKLIST.md`
- `docs/TESTING_EXPANSION_GUIDE.md`

---

## Production Notes

Before real deployment:

- Move secrets to environment variables or a secrets manager.
- Replace development passwords.
- Replace email stub with SMTP/SendGrid/Mailgun.
- Enforce confirmed email.
- Use HTTPS everywhere.
- Add integration tests with Testcontainers.
- Add real payment gateway.
- Add object storage for lesson files/videos.

---

## Final Evaluation

EduPlatform is now a strong enterprise-style portfolio project. It demonstrates backend architecture, real authentication, role management, Docker infrastructure, logging, database design, MVC/API separation, and professional troubleshooting.
