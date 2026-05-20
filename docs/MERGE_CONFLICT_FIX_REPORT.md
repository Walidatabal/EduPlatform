# Merge Conflict Fix Report

## Fixed Issues

- Removed Git merge conflict markers from project files.
- Repaired `EduPlatform.Infrastructure.csproj` XML structure.
- Repaired `EduPlatform.Web.csproj` XML structure.
- Kept the latest Live Sessions + Attendance implementation.
- Kept `MailKit` package reference for the real Email Service.
- Added missing namespace to `IEmailService`.
- Ensured `EmailSettings` remains public and usable from Infrastructure.
- Ensured `DependencyInjection.cs` registers `EmailSettings` and `EmailService`.
- Kept Live Session attendance DbSet registration.
- Kept UI image folder registration for Web project.

## After Extracting

Run:

```powershell
dotnet clean
dotnet build
dotnet test
```

If Docker SQL has old schema, run:

```powershell
dotnet ef database update --project EduPlatform.Infrastructure --startup-project EduPlatform.API
docker compose restart
```

## Important

Do not commit `.env`, `bin`, `obj`, `.vs`, or logs.
