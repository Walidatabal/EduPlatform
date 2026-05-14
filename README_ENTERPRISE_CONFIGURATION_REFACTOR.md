# Enterprise EF Core Configuration Refactor

This upgrade moves the long EF Core configuration code out of `AppDbContext` and into separate configuration classes.

## Why this is better

The previous `AppDbContext` worked, but it was becoming too large. In enterprise projects, each entity should own its mapping through `IEntityTypeConfiguration<T>`.

## New structure

```text
EduPlatform.Infrastructure
 └── Persistence
      └── Configurations
           ├── CourseConfiguration.cs
           ├── LessonConfiguration.cs
           ├── OrderConfiguration.cs
           ├── OrderItemConfiguration.cs
           └── ...
```

## What changed

`AppDbContext` now only contains:

- DbSet declarations
- ApplyConfigurationsFromAssembly
- SaveChangesAsync audit/soft-delete logic

All these were moved to configuration files:

- Soft delete query filters
- Unique indexes
- Decimal precision
- Relationships
- DeleteBehavior.NoAction to avoid SQL Server multiple cascade paths

## Why DeleteBehavior.NoAction is used

SQL Server can reject migrations when several relationships create multiple cascade paths. `DeleteBehavior.NoAction` prevents accidental cascading delete chains and is safer for large LMS systems.

## After download

Run:

```powershell
dotnet clean
dotnet build
```

If the build succeeds, create a migration:

```powershell
dotnet ef migrations add RefactorEntityConfigurations --project .\EduPlatform.Infrastructure --startup-project .\EduPlatform.API
```

Then update the database:

```powershell
dotnet ef database update --project .\EduPlatform.Infrastructure --startup-project .\EduPlatform.API
```

If you already have a recently created migration that only contains the old configuration changes, remove it first:

```powershell
dotnet ef migrations remove --project .\EduPlatform.Infrastructure --startup-project .\EduPlatform.API
```

## Result

The project now follows a cleaner enterprise EF Core structure and is easier to maintain, test, and extend.
