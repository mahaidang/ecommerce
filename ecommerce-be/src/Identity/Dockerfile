FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Identity/Identity.Domain/*.csproj ./src/Identity/Identity.Domain/
COPY src/Identity/Identity.Application/*.csproj ./src/Identity/Identity.Application/
COPY src/Identity/Identity.Infrastructure/*.csproj ./src/Identity/Identity.Infrastructure/
COPY src/Identity/Identity.Api/*.csproj ./src/Identity/Identity.Api/

# Restore dependencies
WORKDIR /src/src/Identity/Identity.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Identity/ ./src/Identity/

# Build và publish
WORKDIR /src/src/Identity/Identity.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Identity.Api.dll"]