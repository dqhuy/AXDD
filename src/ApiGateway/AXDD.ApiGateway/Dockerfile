# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/AXDD.sln", "./"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Common/AXDD.BuildingBlocks.Common.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/AXDD.BuildingBlocks.Domain/AXDD.BuildingBlocks.Domain.csproj", "BuildingBlocks/AXDD.BuildingBlocks.Domain/"]
COPY ["src/ApiGateway/AXDD.ApiGateway/AXDD.ApiGateway.csproj", "ApiGateway/AXDD.ApiGateway/"]

# Restore dependencies
RUN dotnet restore "ApiGateway/AXDD.ApiGateway/AXDD.ApiGateway.csproj"

# Copy source code
COPY src/BuildingBlocks/ BuildingBlocks/
COPY src/ApiGateway/AXDD.ApiGateway/ ApiGateway/AXDD.ApiGateway/

# Build
WORKDIR "/src/ApiGateway/AXDD.ApiGateway"
RUN dotnet build "AXDD.ApiGateway.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AXDD.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.ApiGateway.dll"]
