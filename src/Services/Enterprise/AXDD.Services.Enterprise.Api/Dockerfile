# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/AXDD.sln", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/Services/Enterprise/AXDD.Services.Enterprise.Api/AXDD.Services.Enterprise.Api.csproj", "Services/Enterprise/AXDD.Services.Enterprise.Api/"]

RUN dotnet restore "Services/Enterprise/AXDD.Services.Enterprise.Api/AXDD.Services.Enterprise.Api.csproj"

COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/Enterprise/AXDD.Services.Enterprise.Api/ Services/Enterprise/AXDD.Services.Enterprise.Api/

WORKDIR "/src/Services/Enterprise/AXDD.Services.Enterprise.Api"
RUN dotnet build "AXDD.Services.Enterprise.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.Enterprise.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Enterprise.Api.dll"]
