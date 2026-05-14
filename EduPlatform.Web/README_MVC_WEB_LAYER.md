# EduPlatform.Web - Enterprise MVC Presentation Layer

## What was added

A new MVC project was added beside the existing API project:

```text
EduPlatform.API       -> REST API, Swagger, JWT, external clients
EduPlatform.Web       -> MVC UI, dashboards, school portal screens
EduPlatform.Domain    -> Entities and domain rules
EduPlatform.Application -> Contracts and use-case models
EduPlatform.Infrastructure -> EF Core, Identity, repositories, seeders
EduPlatform.Tests     -> Unit/integration testing foundation
```

## Why this is better

This keeps the architecture clean. The API remains focused on external clients such as mobile apps, React, or integrations. The MVC project becomes the server-rendered web portal for Admin, Teachers, Students, and Parents.

## MVC screens added

```text
Home page
Login page
Dashboard page
Grades list/details
Courses list/details
Shared layout
CSS styling
```

## Enterprise rule

Controllers are thin. They do not talk directly to SQL. They use `IUnitOfWork`, which comes from the Domain/Infrastructure layers.

## How to run

```powershell
dotnet restore
dotnet build
dotnet ef database update --project .\EduPlatform.Infrastructure --startup-project .\EduPlatform.API
dotnet run --project .\EduPlatform.Web
```

Then open the URL shown in PowerShell, usually:

```text
https://localhost:xxxx
```

## Next recommended MVC upgrades

```text
Admin Area
Teacher Area
Student Area
Parent Area
Course management CRUD
Grade/Subject management CRUD
Live sessions calendar
Student progress screen
Payments screen
Reports dashboard
```
