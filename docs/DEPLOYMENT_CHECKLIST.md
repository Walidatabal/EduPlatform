# Deployment Checklist

## 1. Before Deployment

Run:

```bash
dotnet clean
dotnet restore
dotnet build
dotnet test
```

Confirm:

```text
No build errors
No package downgrade warnings
No secrets committed
README updated
.env.example exists
```

---

## 2. Docker Deployment

Run:

```bash
docker compose down --remove-orphans
docker compose up -d --build
docker compose ps
```

Expected services:

```text
sqlserver
redis
seq
api
web
```

Expected URLs:

```text
API/Swagger: http://localhost:8080
Web MVC:     http://localhost:8081
Seq Logs:    http://localhost:8088
SQL Server:  localhost:1433
Redis:       localhost:6379
```

---

## 3. Local Development Mode

Start infrastructure only:

```bash
docker compose up -d sqlserver redis seq
```

Then run:

```bash
dotnet run --project EduPlatform.API
dotnet run --project EduPlatform.Web
```

Local connection string must use:

```text
Server=localhost,1433
```

Docker connection string must use:

```text
Server=sqlserver,1433
```

---

## 4. Smoke Test Checklist

After deployment, test:

```text
Open Swagger
Open MVC Web
Login as Admin
Open Dashboard
Open Access Control
Create user
Unlock user
Change password
Create course through Swagger
Open Seq logs
Check /health endpoint
```

---

## 5. Common Problems

## SQL connection failed

Check SQL container:

```bash
docker compose ps
docker compose logs sqlserver --tail=100
```

## Admin login invalid

Use:

```text
admin@eduplatform.com
Admin@123456
```

If locked, run:

```sql
UPDATE AspNetUsers
SET LockoutEnd = NULL,
    AccessFailedCount = 0,
    LockoutEnabled = 0,
    EmailConfirmed = 1
WHERE Email = 'admin@eduplatform.com';
```

## Swagger returns 401

Login through API, copy accessToken only, paste token in Authorize box.

## Swagger returns 403

Token is valid but role is not allowed.

---

# Production Notes

For real production:

```text
Use HTTPS
Use secret manager
Do not seed demo users
Do not expose SQL publicly
Protect Seq with authentication
Use production CORS origins only
Enable backups
Add monitoring
```
