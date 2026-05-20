# Starts EduPlatform using Docker Compose.
# Run from the solution root: .\scripts\run-docker.ps1

docker compose down --remove-orphans
docker compose up -d --build
docker compose ps

Write-Host "Web:     http://localhost:8081" -ForegroundColor Green
Write-Host "API:     http://localhost:8080" -ForegroundColor Green
Write-Host "Seq:     http://localhost:8088" -ForegroundColor Green
