# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/AXDD.slnx", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/Services/Report/AXDD.Services.Report.Api/AXDD.Services.Report.Api.csproj", "Services/Report/AXDD.Services.Report.Api/"]

RUN dotnet restore "Services/Report/AXDD.Services.Report.Api/AXDD.Services.Report.Api.csproj"

COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/Services/Report/AXDD.Services.Report.Api/ Services/Report/AXDD.Services.Report.Api/

WORKDIR "/src/Services/Report/AXDD.Services.Report.Api"
RUN dotnet build "AXDD.Services.Report.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AXDD.Services.Report.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Report.Api.dll"]
