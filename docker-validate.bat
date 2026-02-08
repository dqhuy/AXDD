@echo off
REM AXDD Docker Configuration Validation Script for Windows
REM This script validates that all Docker services are configured correctly

setlocal enabledelayedexpansion

echo ==========================================
echo AXDD Configuration Validation
echo ==========================================
echo.

REM Check if Docker is available
docker --version >nul 2>&1
if errorlevel 1 (
    echo X Docker is not installed
    exit /b 1
)

docker compose version >nul 2>&1
if errorlevel 1 (
    echo X Docker Compose is not available
    exit /b 1
)

echo √ Docker and Docker Compose are available
echo.

echo 1. Checking Infrastructure Services...
echo --------------------------------------
call :check_service sqlserver
call :check_service postgres-gis
call :check_service redis
call :check_service rabbitmq
call :check_service minio
call :check_service elasticsearch
echo.

echo 2. Checking Microservices...
echo --------------------------------------
call :check_service auth-api
call :check_service masterdata-api
call :check_service enterprise-api
call :check_service investment-api
call :check_service filemanager-api
call :check_service report-api
call :check_service notification-api
call :check_service logging-api
call :check_service search-api
call :check_service gis-api
echo.

echo 3. Checking API Gateway and Web Apps...
echo --------------------------------------
call :check_service api-gateway
call :check_service admin-webapp
call :check_service enterprise-portal
echo.

echo 4. Testing HTTP Endpoints...
echo --------------------------------------
call :check_endpoint "API Gateway" "http://localhost:5000"
call :check_endpoint "Admin Web App" "http://localhost:8080"
call :check_endpoint "Enterprise Portal" "http://localhost:4200"
echo.

echo 5. Testing API Gateway Routes...
echo --------------------------------------
for %%s in (auth masterdata enterprise investment filemanager report notification logging search gis) do (
    curl -f -s -m 2 "http://localhost:5000/api/%%s/health" >nul 2>&1
    if errorlevel 1 (
        echo ? Gateway route: /api/%%s (may not have /health endpoint)
    ) else (
        echo √ Gateway route: /api/%%s
    )
)
echo.

echo 6. Checking Database Connections...
echo --------------------------------------

REM Check SQL Server
docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" -C >nul 2>&1
if errorlevel 1 (
    echo X SQL Server connection
) else (
    echo √ SQL Server connection
)

REM Check PostgreSQL
docker compose exec -T postgres-gis psql -U postgres -d gisdb -c "SELECT 1" >nul 2>&1
if errorlevel 1 (
    echo X PostgreSQL connection
) else (
    echo √ PostgreSQL connection
)

REM Check Redis
docker compose exec -T redis redis-cli ping >nul 2>&1
if errorlevel 1 (
    echo X Redis connection
) else (
    echo √ Redis connection
)

REM Check RabbitMQ
docker compose exec -T rabbitmq rabbitmq-diagnostics -q ping >nul 2>&1
if errorlevel 1 (
    echo X RabbitMQ connection
) else (
    echo √ RabbitMQ connection
)

echo.
echo ==========================================
echo Validation Complete!
echo ==========================================
echo.
echo Access Points:
echo   Admin Portal:      http://localhost:8080
echo   Enterprise Portal: http://localhost:4200
echo   API Gateway:       http://localhost:5000
echo.
echo Management Consoles:
echo   RabbitMQ:          http://localhost:15672 (admin/admin)
echo   MinIO:             http://localhost:9001 (minioadmin/minioadmin)
echo.
echo To view logs:
echo   docker compose logs -f [service-name]
echo.
goto :eof

:check_service
set service_name=%1
docker compose ps --format json %service_name% 2>nul | findstr /C:"running" >nul 2>&1
if errorlevel 1 (
    docker compose ps --format json %service_name% 2>nul | findstr /C:"healthy" >nul 2>&1
    if errorlevel 1 (
        echo X %service_name%: not running
    ) else (
        echo √ %service_name%: healthy
    )
) else (
    echo √ %service_name%: running
)
goto :eof

:check_endpoint
set endpoint_name=%~1
set endpoint_url=%~2
curl -f -s -m 5 "%endpoint_url%" >nul 2>&1
if errorlevel 1 (
    echo X %endpoint_name% endpoint: %endpoint_url% (not accessible)
) else (
    echo √ %endpoint_name% endpoint: %endpoint_url%
)
goto :eof

endlocal
