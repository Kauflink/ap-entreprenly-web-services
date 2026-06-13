# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first to leverage layer caching
COPY src/Entreprenly.WebServices/Entreprenly.WebServices.csproj src/Entreprenly.WebServices/
RUN dotnet restore src/Entreprenly.WebServices/Entreprenly.WebServices.csproj

# Build and publish
COPY . .
RUN dotnet publish src/Entreprenly.WebServices/Entreprenly.WebServices.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Entreprenly.WebServices.dll"]
