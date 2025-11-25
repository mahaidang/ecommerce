FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Product/Product.Domain/*.csproj ./src/Product/Product.Domain/
COPY src/Product/Product.Application/*.csproj ./src/Product/Product.Application/
COPY src/Product/Product.Infrastructure/*.csproj ./src/Product/Product.Infrastructure/
COPY src/Product/Product.Api/*.csproj ./src/Product/Product.Api/

# Restore dependencies
WORKDIR /src/src/Product/Product.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Product/ ./src/Product/

# Build và publish
WORKDIR /src/src/Product/Product.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Product.Api.dll"]