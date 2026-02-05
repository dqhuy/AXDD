# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/AXDD.sln", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/Services/Investment/AXDD.Services.Investment.Api/AXDD.Services.Investment.Api.csproj", "Services/Investment/AXDD.Services.Investment.Api/"]

RUN dotnet restore "Services/Investment/AXDD.Services.Investment.Api/AXDD.Services.Investment.Api.csproj"

COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/Investment/AXDD.Services.Investment.Api/ Services/Investment/AXDD.Services.Investment.Api/

WORKDIR "/src/Services/Investment/AXDD.Services.Investment.Api"
RUN dotnet build "AXDD.Services.Investment.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.Investment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Investment.Api.dll"]
