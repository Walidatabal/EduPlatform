# EduPlatform Production Go-Live Runbook

## 1. Goal

This runbook prepares EduPlatform for a real production deployment using Azure App Service + Azure SQL or a Docker VPS.

## 2. Required Before Go-Live

| Requirement | Status |
|---|---|
| `dotnet build` passes | Required |
| `dotnet test` passes | Required |
| Docker stack runs | Required |
| Swagger login works | Required |
| MVC login works | Required |
| Database migrations applied | Required |
| `.env` not committed | Required |
| Production secrets configured outside GitHub | Required |
| HTTPS enabled | Required |
| Default passwords changed | Required |

## 3. Production Environment Variables

Configure these on the hosting platform:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<production SQL connection string>
Jwt__Key=<long production JWT key>
Jwt__Issuer=EduPlatform
Jwt__Audience=EduPlatformUsers
Jwt__ExpiryHours=24
Jwt__RefreshTokenDays=7
Seeding__AdminEmail=admin@eduplatform.com
Seeding__AdminPassword=<strong production admin password>
Cors__AllowedOrigins__0=https://your-web-domain.com
```

## 4. Azure Deployment Steps

1. Create Azure SQL Database.
2. Create App Service for API.
3. Create App Service for MVC Web.
4. Add environment variables to both App Services as needed.
5. Publish API:

```powershell
dotnet publish EduPlatform.API -c Release
```

6. Publish MVC:

```powershell
dotnet publish EduPlatform.Web -c Release
```

7. Apply database migrations against the production connection string:

```powershell
dotnet ef database update --project EduPlatform.Infrastructure --startup-project EduPlatform.API
```

8. Test production endpoints.

## 5. Production Verification

| Test | Expected |
|---|---|
| API `/health` | Healthy |
| Swagger opens | OK or secured |
| Admin login | JWT generated |
| MVC login | Dashboard opens |
| Seq/Application logs | No critical errors |
| SQL connection | No migration errors |
| Refresh token | Returned at login |

## 6. Rollback

If production fails:

1. Stop API/MVC.
2. Restore last SQL backup.
3. Revert last Git commit or redeploy previous artifact.
4. Re-run verification checklist.
