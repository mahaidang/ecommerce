FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Inventory/Inventory.Domain/*.csproj ./src/Inventory/Inventory.Domain/
COPY src/Inventory/Inventory.Application/*.csproj ./src/Inventory/Inventory.Application/
COPY src/Inventory/Inventory.Infrastructure/*.csproj ./src/Inventory/Inventory.Infrastructure/
COPY src/Inventory/Inventory.Api/*.csproj ./src/Inventory/Inventory.Api/

# Restore dependencies
WORKDIR /src/src/Inventory/Inventory.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Inventory/ ./src/Inventory/

# Build và publish
WORKDIR /src/src/Inventory/Inventory.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]