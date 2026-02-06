# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/AXDD.slnx", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/Services/MasterData/AXDD.Services.MasterData.Api/AXDD.Services.MasterData.Api.csproj", "Services/MasterData/AXDD.Services.MasterData.Api/"]

RUN dotnet restore "Services/MasterData/AXDD.Services.MasterData.Api/AXDD.Services.MasterData.Api.csproj"

COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/MasterData/AXDD.Services.MasterData.Api/ Services/MasterData/AXDD.Services.MasterData.Api/

WORKDIR "/src/Services/MasterData/AXDD.Services.MasterData.Api"
RUN dotnet build "AXDD.Services.MasterData.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.MasterData.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.MasterData.Api.dll"]
