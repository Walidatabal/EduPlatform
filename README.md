# EduPlatform API

> Online learning marketplace — Udemy / Uula style  
> Built with **ASP.NET Core 8** · **Clean Architecture** · **SQL Server** · **Docker** · **Swagger**

---

## 🗂️ Solution structure

```
EduPlatform/
├── EduPlatform.Domain/          # Entities, enums, repository interfaces (no external deps)
├── EduPlatform.Application/     # DTOs, commands, service interfaces, Result<T>
├── EduPlatform.Infrastructure/  # EF Core, repositories, JWT, seeders
├── EduPlatform.API/             # Controllers, middleware, Swagger, Program.cs
├── EduPlatform.Tests/           # xUnit unit + integration tests
├── Dockerfile                   # Multi-stage build
├── docker-compose.yml           # API + SQL Server 2022
└── .github/workflows/           # CI (build+test) + CD (Docker push)
```

---

## 🚀 Quick start

### Option A — Docker (recommended)

```bash
# 1. Copy env file
cp .env.example .env

# 2. Start everything (API + SQL Server)
docker compose up -d

# 3. Open Swagger UI
open http://localhost:8080
```

### Option B — Local development

**Prerequisites:** .NET 8 SDK · SQL Server (or Docker for just the DB)

```bash
# Start SQL Server only
docker compose up sqlserver -d

# Restore + run
dotnet restore
dotnet run --project EduPlatform.API

# Open Swagger
open http://localhost:5000
```

---

## 📋 Environment variables

| Variable | Default | Description |
|---|---|---|
| `MSSQL_SA_PASSWORD` | `Dev_Password123!` | SQL Server SA password |
| `JWT_KEY` | *(required)* | JWT signing secret (min 32 chars) |
| `ADMIN_EMAIL` | `admin@eduplatform.com` | Seeded admin email |
| `ADMIN_PASSWORD` | `Admin@123456` | Seeded admin password |

---

## 🔐 Authentication

All protected routes require a **Bearer JWT** in the `Authorization` header.

```
Authorization: Bearer <your_token>
```

**Getting a token:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@eduplatform.com",
  "password": "Admin@123456"
}
```

---

## 📡 API endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | — | Register student or pending-teacher |
| `POST` | `/api/auth/login` | — | Login, get JWT |
| `GET` | `/api/auth/me` | Any | Current user info |
| `GET` | `/api/grades` | — | List all grades |
| `POST` | `/api/grades` | Admin | Create grade |
| `PUT` | `/api/grades/{id}` | Admin | Update grade |
| `DELETE` | `/api/grades/{id}` | Admin | Soft-delete grade |
| `GET` | `/api/subjects` | — | List subjects (filter by `?gradeId=`) |
| `POST` | `/api/subjects` | Admin | Create subject |
| `GET` | `/api/courses` | — | Browse published courses (filter by grade/subject/price) |
| `GET` | `/api/courses/{id}` | — | Course detail + syllabus |
| `POST` | `/api/courses` | Teacher | Create course |
| `PUT` | `/api/courses/{id}` | Teacher/Admin | Update course |
| `POST` | `/api/courses/{id}/submit` | Teacher | Submit for review |
| `POST` | `/api/courses/{id}/review` | Admin | Approve or reject |
| `POST` | `/api/enrollments` | Student | Enroll in free course |
| `GET` | `/api/enrollments/my` | Student | My enrollments |
| `GET` | `/api/enrollments/check/{courseId}` | Any | Check enrollment status |

---

## 🧪 Running tests

```bash
dotnet test EduPlatform.Tests --verbosity normal
```

---

## 🔨 Database migrations

```bash
# Add new migration
dotnet ef migrations add <MigrationName> \
  --project EduPlatform.Infrastructure \
  --startup-project EduPlatform.API

# Apply migrations
dotnet ef database update \
  --project EduPlatform.Infrastructure \
  --startup-project EduPlatform.API
```

> Migrations also auto-apply on startup via `MigrateAsync()` in `DbSeeder`.

---

## 📦 Postman collection

Import **`EduPlatform.postman_collection.json`** from the repo root.

The collection includes pre-configured requests for all endpoints with environment variables for `base_url` and `token`.

---

## 🗺️ Roadmap

| Phase | Weeks | Focus |
|---|---|---|
| **1 — Foundation** | 1–6 | ✅ Auth, catalog CRUD, video hosting, free enrollment |
| **2 — Monetize** | 7–12 | MyFatoorah + Stripe, teacher revenue, discount codes |
| **3 — Learning** | 13–20 | Progress tracking, quizzes, certificates, reviews |
| **4 — Scale** | 21–30 | RTL/Arabic, live sessions, CI/CD, Redis, mobile |

---

## 🛠️ Tech stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Docs | Swagger / Swashbuckle |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Tests | xUnit |
| Video | Bunny.net *(Phase 1)* |
| Payments | MyFatoorah + Stripe *(Phase 2)* |
