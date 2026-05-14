# Docker repeated restart / SQL login fix

## What was fixed

1. `docker-compose.yml`
   - The API connection string now uses the same `MSSQL_SA_PASSWORD` value as the SQL Server container.
   - Before, SQL Server could use the password from `.env`, while the API still used the hard-coded `Dev_Password123!`.
   - That caused repeated API restarts and SQL login failures.

2. `EduPlatform.Infrastructure/Seeders/DbSeeder.cs`
   - Added retry logic before applying EF Core migrations.
   - This protects startup when SQL Server is still becoming ready.

## Clean restart commands

Run these from the solution folder:

```powershell
docker compose down -v
docker compose build --no-cache
docker compose up -d
```

Then check:

```powershell
docker compose ps
docker logs eduplatform_api --tail 100
```

Open:

```text
http://localhost:8080
```

## Important

If you change `MSSQL_SA_PASSWORD` in `.env`, you must recreate the SQL volume:

```powershell
docker compose down -v
docker compose up -d --build
```

SQL Server keeps the old SA password inside the volume. Changing `.env` alone is not enough if the volume already exists.
