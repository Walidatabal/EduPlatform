# Starts only infrastructure containers for local Visual Studio/dotnet run mode.
# API/Web should use Server=localhost,1433 in appsettings.Development.json.

docker compose up -d sqlserver redis seq
docker compose ps

Write-Host "SQL Server: localhost,1433" -ForegroundColor Green
Write-Host "Seq:        http://localhost:8088" -ForegroundColor Green
