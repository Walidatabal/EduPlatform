# EduPlatform Enterprise LMS - Final Architecture Guide v5

Version: Final Portfolio Release  
Stack: ASP.NET Core 8, EF Core 8, SQL Server 2022, Docker, Redis, Serilog, Seq, xUnit, Swagger, MVC, JWT

---

## 1. Executive Summary

EduPlatform is an enterprise-style Learning Management System that supports students, teachers, parents, administrators, and content managers. It contains two presentation surfaces:

1. `EduPlatform.API` - JWT-secured REST API for machine/mobile/front-end clients.
2. `EduPlatform.Web` - ASP.NET Core MVC portal using cookie authentication and Razor views.

Both surfaces share the same Infrastructure layer, database, Identity system, repositories, UnitOfWork, services, and seeders.

---

## 2. Solution Structure

```text
EduPlatform/
├── EduPlatform.Domain
├── EduPlatform.Application
├── EduPlatform.Infrastructure
├── EduPlatform.API
├── EduPlatform.Web
├── EduPlatform.Tests
├── docs
├── scripts
├── docker-compose.yml
└── README.md
```

### Dependency Rule

```text
Domain <- Application <- Infrastructure <- API/Web
```

The Domain layer does not depend on EF Core, MVC, Identity, Docker, or any external framework.

---

## 3. Domain Layer

Contains:

- Entities
- Enums
- Role constants
- Repository interfaces
- UnitOfWork contract
- BaseEntity

Key entities include:

- Course
- Grade
- Subject
- Category
- Section
- Lesson
- Enrollment
- CartItem
- WishlistItem
- Order
- Payment
- Coupon
- Certificate
- LessonProgress
- LiveSession
- Notification
- CourseQuestion
- CourseAnswer
- ParentStudentLink

### BaseEntity

Provides:

- `Id`
- `CreatedAt`
- `UpdatedAt`
- `CreatedBy`
- `UpdatedBy`
- `IsDeleted`

Soft-delete support protects academic/payment data from physical deletion.

---

## 4. Application Layer

Contains:

- DTOs
- Commands
- Validators
- Result wrappers
- Interfaces
- Mapping profile
- Pagination models

### Key Patterns

- `Result`
- `ServiceResult<T>`
- `ApiResponse<T>`
- `PagedResult<T>`
- FluentValidation validators
- AutoMapper profile

### Why DTOs?

DTOs prevent domain entities from leaking into API responses or MVC views.

---

## 5. Infrastructure Layer

Contains:

- `AppDbContext`
- Identity user
- Entity configurations
- Repositories
- UnitOfWork implementation
- LMS services
- Auth services
- Token service
- Email service
- Seeders

### ApplicationUser

Extends IdentityUser with:

- FullName
- IsDeleted
- CreatedAt
- RefreshToken
- RefreshTokenExpiresAt

### UnitOfWork

Provides one shared DbContext and one atomic commit for multi-step operations.

### Seeders

Seeders create:

- roles
- admin account
- teachers
- students
- parents
- grades
- subjects
- categories
- courses
- sections/lessons
- enrollments
- demo LMS data
- parent/student links

---

## 6. API Layer

Responsibilities:

- REST API endpoints
- JWT authentication
- Swagger/OpenAPI
- Rate limiting
- CORS
- Validation filter
- Exception middleware
- Health checks

### Middleware Order

1. ExceptionMiddleware
2. Swagger
3. Static files
4. Serilog request logging
5. CORS
6. Rate limiter
7. Authentication
8. Authorization
9. Health checks and controllers

### Auth Endpoints

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/refresh`
- `POST /api/Auth/revoke`
- `POST /api/Auth/forgot-password`
- `POST /api/Auth/reset-password`
- `GET /api/Auth/me`

---

## 7. Web Layer

Responsibilities:

- MVC portal
- Razor views
- ViewModels
- Cookie authentication
- role-based dashboard
- bilingual UI
- profile/change-password UI
- admin user access control

### Key MVC Routes

| Route | Purpose |
|---|---|
| `/Account/Login` | Login |
| `/Account/Profile` | User profile |
| `/Account/ChangePassword` | User changes own password |
| `/Account/Users` | Admin/ContentManager user access list |
| `/Account/CreateUser` | Admin creates users |
| `/Account/EditRoles/{id}` | Admin edits roles |
| `/Account/ResetPassword/{id}` | Admin resets password |
| `/PendingTasks` | Pending teachers/courses/locked users |
| `/Dashboard` | Role-based dashboard |

---

## 8. Authentication Design

### API

Uses JWT Bearer tokens.

Access token:

- contains user id
- email
- roles
- JTI
- 24-hour expiry by default

Refresh token:

- generated using cryptographic random bytes
- stored on the user account
- expires based on `Jwt:RefreshTokenDays`
- can be revoked

### MVC

Uses ASP.NET Core Identity cookie auth.

Cookie:

- HttpOnly
- SameSite=Lax
- 8-hour sliding expiration

---

## 9. Authorization Model

Roles:

- Admin
- ContentManager
- Teacher
- PendingTeacher
- Student
- Parent

### Admin

Can manage users, roles, passwords, unlock accounts, approve teachers/courses, and access all dashboards.

### ContentManager

Can review users/pending tasks and unlock accounts, but does not have full user creation/reset power unless explicitly granted.

### Teacher

Can create/manage own courses and live sessions.

### Student

Can browse, enroll, track progress, review courses, and earn certificates.

### Parent

Can view linked student learning status.

---

## 10. Database Strategy

### Local Visual Studio

Use:

```text
Server=localhost,1433
```

### Docker Containers

Use:

```text
Server=sqlserver,1433
```

### Why?

- Local apps connect through the host port.
- Containers connect through Docker internal DNS service names.

---

## 11. Docker Architecture

Services:

- sqlserver
- redis
- seq
- api
- web

Ports:

- API: 8080
- Web: 8081
- SQL: 1433
- Seq: 8088
- Redis: 6379

---

## 12. Logging Strategy

Serilog writes to:

- Console
- Rolling files
- Seq

Seq allows searching logs by:

- request path
- status code
- trace id
- exception type
- application name

---

## 13. Validation Strategy

FluentValidation validates request DTOs/commands before controller execution.

`ValidateModelFilter` standardizes validation failures into a consistent API response.

---

## 14. Testing Strategy

Current unit tests cover:

- BaseEntity
- enums
- result wrappers
- DTOs
- ViewModels

Recommended next test layer:

- controller tests
- service tests
- repository tests
- integration tests with Testcontainers SQL Server

---

## 15. OBS / Live Video Preparation

OBS Studio setup was prepared for future LMS content creation:

- Display Capture
- Webcam
- Microphone
- 1080p / 30 FPS
- hardware encoding
- teaching scene

Future integration options:

- YouTube Live links
- Zoom/Teams links
- WebRTC
- Agora
- Jitsi
- recording upload to course lessons

---

## 16. Security Checklist

Before production:

- Move JWT key to environment variable or secret manager.
- Replace development SA password.
- Enable confirmed email.
- Add real email provider.
- Use HTTPS only.
- Add refresh-token rotation policy.
- Add audit logs for role/password changes.
- Add file upload validation.
- Add rate limits per endpoint category.

---

## 17. Final Evaluation

EduPlatform is now a strong enterprise-style project suitable for:

- GitHub portfolio
- technical interviews
- backend architecture demonstration
- full-stack .NET practice
- MVP demonstration

Overall status: strong portfolio / enterprise junior-to-mid level project.
