FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/GIS/AXDD.Services.GIS.Api/AXDD.Services.GIS.Api.csproj", "src/Services/GIS/AXDD.Services.GIS.Api/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "src/BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/AXDD.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/"]

# Restore packages
RUN dotnet restore "src/Services/GIS/AXDD.Services.GIS.Api/AXDD.Services.GIS.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/src/Services/GIS/AXDD.Services.GIS.Api"
RUN dotnet build "AXDD.Services.GIS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.GIS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.GIS.Api.dll"]
