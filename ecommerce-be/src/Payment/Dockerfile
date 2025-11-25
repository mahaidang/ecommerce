FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Payment/Payment.Domain/*.csproj ./src/Payment/Payment.Domain/
COPY src/Payment/Payment.Application/*.csproj ./src/Payment/Payment.Application/
COPY src/Payment/Payment.Infrastructure/*.csproj ./src/Payment/Payment.Infrastructure/
COPY src/Payment/Payment.Api/*.csproj ./src/Payment/Payment.Api/

# Restore dependencies
WORKDIR /src/src/Payment/Payment.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Payment/ ./src/Payment/

# Build và publish
WORKDIR /src/src/Payment/Payment.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Payment.Api.dll"]