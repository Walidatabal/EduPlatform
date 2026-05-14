FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution
COPY EduPlatform.sln .

# Copy project files
COPY EduPlatform.API/*.csproj EduPlatform.API/
COPY EduPlatform.Application/*.csproj EduPlatform.Application/
COPY EduPlatform.Domain/*.csproj EduPlatform.Domain/
COPY EduPlatform.Infrastructure/*.csproj EduPlatform.Infrastructure/
COPY EduPlatform.Tests/*.csproj EduPlatform.Tests/
COPY EduPlatform.Web/*.csproj EduPlatform.Web/

# Restore
RUN dotnet restore EduPlatform.sln

# Copy everything
COPY . .

# Build API
WORKDIR /src/EduPlatform.API

RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EduPlatform.API.dll"]