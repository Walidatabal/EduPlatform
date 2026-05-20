\
Write-Host "EduPlatform Preflight Check" -ForegroundColor Cyan

$ErrorActionPreference = "Stop"

Write-Host "1) Checking .NET build..." -ForegroundColor Yellow
dotnet clean
dotnet build

Write-Host "2) Running tests..." -ForegroundColor Yellow
dotnet test

Write-Host "3) Checking EF migrations list..." -ForegroundColor Yellow
dotnet ef migrations list --project EduPlatform.Infrastructure --startup-project EduPlatform.API

Write-Host "4) Checking Docker compose config..." -ForegroundColor Yellow
docker compose config | Out-Null

Write-Host "Preflight completed successfully." -ForegroundColor Green
