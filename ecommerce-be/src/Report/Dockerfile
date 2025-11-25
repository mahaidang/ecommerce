FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Ecommerce.sln .

# Copy các file csproj trước (tận dụng Docker cache)
COPY src/BuildingBlocks/Shared.Abstractions/*.csproj ./src/BuildingBlocks/Shared.Abstractions/
COPY src/BuildingBlocks/Shared.Contracts/*.csproj ./src/BuildingBlocks/Shared.Contracts/
COPY src/BuildingBlocks/Shared.Infrastructure/*.csproj ./src/BuildingBlocks/Shared.Infrastructure/

COPY src/Report/Report.Domain/*.csproj ./src/Report/Report.Domain/
COPY src/Report/Report.Application/*.csproj ./src/Report/Report.Application/
COPY src/Report/Report.Infrastructure/*.csproj ./src/Report/Report.Infrastructure/
COPY src/Report/Report.Api/*.csproj ./src/Report/Report.Api/

# Restore dependencies
WORKDIR /src/src/Report/Report.Api
RUN dotnet restore

# Copy toàn bộ source code
WORKDIR /src
COPY src/BuildingBlocks/ ./src/BuildingBlocks/
COPY src/Report/ ./src/Report/

# Build và publish
WORKDIR /src/src/Report/Report.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Report.Api.dll"]