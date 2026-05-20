# Cleans generated development artifacts that should not be committed.
# Run from the solution root: .\scripts\clean.ps1

$folders = @(".vs", "TestResults", "coverage", "Logs", "logs")
foreach ($folder in $folders) {
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force
    }
}

Get-ChildItem -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -File -Include *.user,*.suo,*.zip,*.log | Remove-Item -Force

Write-Host "Cleanup complete." -ForegroundColor Green
