FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Basket/Basket.Domain/*.csproj ./src/Basket/Basket.Domain/
COPY src/Basket/Basket.Application/*.csproj ./src/Basket/Basket.Application/
COPY src/Basket/Basket.Infrastructure/*.csproj ./src/Basket/Basket.Infrastructure/
COPY src/Basket/Basket.Api/*.csproj ./src/Basket/Basket.Api/

# Restore dependencies
WORKDIR /src/src/Basket/Basket.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Basket/ ./src/Basket/

# Build và publish
WORKDIR /src/src/Basket/Basket.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Basket.Api.dll"]