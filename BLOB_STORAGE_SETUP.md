# Azure Blob Storage Setup

## 1. Add to appsettings.json (Web + API)
```json
"AzureStorage": {
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=YOUR_ACCOUNT;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net",
  "AccountName": "YOUR_ACCOUNT_NAME"
}
```

## 2. Run EF Migration (for AvatarUrl column)
```powershell
dotnet ef migrations add AddAvatarUrl --project EduPlatform.Infrastructure --startup-project EduPlatform.Web
dotnet ef database update --startup-project EduPlatform.Web
```

## 3. Rebuild Docker
```powershell
docker compose up -d --build
```

## Azure Storage Containers (auto-created on first upload)
- course-thumbnails
- avatars
- certificates
- lesson-files
