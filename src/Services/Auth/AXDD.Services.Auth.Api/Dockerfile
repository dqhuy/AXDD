# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/AXDD.sln", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/Services/Auth/AXDD.Services.Auth.Api/AXDD.Services.Auth.Api.csproj", "Services/Auth/AXDD.Services.Auth.Api/"]

# Restore dependencies
RUN dotnet restore "Services/Auth/AXDD.Services.Auth.Api/AXDD.Services.Auth.Api.csproj"

# Copy source code
COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/Auth/AXDD.Services.Auth.Api/ Services/Auth/AXDD.Services.Auth.Api/

# Build
WORKDIR "/src/Services/Auth/AXDD.Services.Auth.Api"
RUN dotnet build "AXDD.Services.Auth.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AXDD.Services.Auth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Auth.Api.dll"]
