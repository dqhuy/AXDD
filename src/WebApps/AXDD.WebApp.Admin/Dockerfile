# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/AXDD.slnx", "./"]
COPY ["src/WebApps/AXDD.WebApp.Admin/AXDD.WebApp.Admin.csproj", "WebApps/AXDD.WebApp.Admin/"]

# Restore dependencies
RUN dotnet restore "WebApps/AXDD.WebApp.Admin/AXDD.WebApp.Admin.csproj"

# Copy source code
COPY src/WebApps/AXDD.WebApp.Admin/ WebApps/AXDD.WebApp.Admin/

# Build
WORKDIR "/src/WebApps/AXDD.WebApp.Admin"
RUN dotnet build "AXDD.WebApp.Admin.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AXDD.WebApp.Admin.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.WebApp.Admin.dll"]
