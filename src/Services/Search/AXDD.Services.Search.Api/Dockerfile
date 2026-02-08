# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/AXDD.slnx", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/AXDD.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/"]
COPY ["src/Services/Search/AXDD.Services.Search.Api/AXDD.Services.Search.Api.csproj", "Services/Search/AXDD.Services.Search.Api/"]

# Restore dependencies
RUN dotnet restore "Services/Search/AXDD.Services.Search.Api/AXDD.Services.Search.Api.csproj"

# Copy source code
COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/Search/AXDD.Services.Search.Api/ Services/Search/AXDD.Services.Search.Api/

# Build
WORKDIR "/src/Services/Search/AXDD.Services.Search.Api"
RUN dotnet build "AXDD.Services.Search.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AXDD.Services.Search.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Search.Api.dll"]
