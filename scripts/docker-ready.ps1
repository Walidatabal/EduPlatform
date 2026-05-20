\
Write-Host "EduPlatform Docker Ready Run" -ForegroundColor Cyan

if (!(Test-Path ".env")) {
    Write-Host "WARNING: .env file not found. Copy .env.example to .env and update values." -ForegroundColor Yellow
}

docker compose down
docker compose up -d --build

docker ps
Write-Host "URLs:" -ForegroundColor Green
Write-Host "API Swagger: http://localhost:8080/swagger"
Write-Host "MVC Web:     http://localhost:8081"
Write-Host "Seq:         http://localhost:8088"
Write-Host "Health:      http://localhost:8080/health"
