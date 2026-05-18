# Refresh Token Migration Guide

This final update adds two properties to `ApplicationUser`:

```csharp
public string? RefreshToken { get; set; }
public DateTime? RefreshTokenExpiresAt { get; set; }
```

If you use an existing SQL Server database, create and apply a migration:

```powershell
dotnet ef migrations add AddRefreshTokenFields --project EduPlatform.Infrastructure --startup-project EduPlatform.API
dotnet ef database update --project EduPlatform.Infrastructure --startup-project EduPlatform.API
```

If you use a clean Docker database and the project has no migrations, the seeder can use `EnsureCreatedAsync()` in demo/local mode.

Production recommendation:

- Prefer real EF migrations.
- Do not rely on `EnsureCreatedAsync()` for production.
- Store refresh tokens as hashed values in production if higher security is required.
