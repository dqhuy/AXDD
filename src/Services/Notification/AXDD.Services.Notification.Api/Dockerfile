FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Services/Notification/AXDD.Services.Notification.Api/AXDD.Services.Notification.Api.csproj", "Services/Notification/AXDD.Services.Notification.Api/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/AXDD.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Infrastructure/"]

RUN dotnet restore "Services/Notification/AXDD.Services.Notification.Api/AXDD.Services.Notification.Api.csproj"

# Copy everything else and build
COPY src/ .
WORKDIR "/src/Services/Notification/AXDD.Services.Notification.Api"
RUN dotnet build "AXDD.Services.Notification.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.Notification.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Notification.Api.dll"]
