FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Ordering/Ordering.Domain/*.csproj ./src/Ordering/Ordering.Domain/
COPY src/Ordering/Ordering.Application/*.csproj ./src/Ordering/Ordering.Application/
COPY src/Ordering/Ordering.Infrastructure/*.csproj ./src/Ordering/Ordering.Infrastructure/
COPY src/Ordering/Ordering.Api/*.csproj ./src/Ordering/Ordering.Api/

# Restore dependencies
WORKDIR /src/src/Ordering/Ordering.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Ordering/ ./src/Ordering/

# Build và publish
WORKDIR /src/src/Ordering/Ordering.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]