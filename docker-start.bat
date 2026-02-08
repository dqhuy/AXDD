@echo off
REM AXDD System - Quick Start Script for Windows
REM This script helps you quickly start the entire AXDD system with Docker Compose

setlocal enabledelayedexpansion

echo ==========================================
echo AXDD System - Docker Quick Start
echo ==========================================
echo.

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo X Docker is not installed. Please install Docker Desktop first.
    exit /b 1
)

echo √ Docker is installed

REM Check if Docker Compose is available
docker compose version >nul 2>&1
if errorlevel 1 (
    echo X Docker Compose V2 is not available. Please update Docker Desktop.
    exit /b 1
)

echo √ Docker Compose is available
echo.

REM Parse command line arguments
set ACTION=%1
if "%ACTION%"=="" set ACTION=up

if "%ACTION%"=="up" goto :start
if "%ACTION%"=="start" goto :start
if "%ACTION%"=="down" goto :stop
if "%ACTION%"=="stop" goto :stop
if "%ACTION%"=="restart" goto :restart
if "%ACTION%"=="logs" goto :logs
if "%ACTION%"=="ps" goto :status
if "%ACTION%"=="status" goto :status
if "%ACTION%"=="clean" goto :clean
goto :usage

:start
echo 🚀 Starting AXDD system...
echo.

echo 📦 Starting infrastructure services...
docker compose up -d sqlserver postgres-gis redis rabbitmq minio elasticsearch

echo.
echo ⏳ Waiting for infrastructure services to be healthy...
echo This may take 1-2 minutes...
echo.

REM Wait a bit for infrastructure to start
timeout /t 30 /nobreak >nul

echo.
echo 🔧 Starting microservices...
docker compose up -d auth-api masterdata-api enterprise-api investment-api filemanager-api report-api notification-api logging-api search-api gis-api

echo.
echo ⏳ Waiting for services to start...
timeout /t 10 /nobreak >nul

echo.
echo 🌐 Starting API Gateway and Web Applications...
docker compose up -d api-gateway admin-webapp enterprise-portal

echo.
echo ==========================================
echo ✅ AXDD System Started Successfully!
echo ==========================================
echo.
echo Access Points:
echo   🌐 Admin Portal:       http://localhost:8080
echo   🌐 Enterprise Portal:  http://localhost:4200
echo   🔌 API Gateway:        http://localhost:5000
echo.
echo Infrastructure Management:
echo   📊 RabbitMQ Console:   http://localhost:15672 (admin/admin)
echo   💾 MinIO Console:      http://localhost:9001 (minioadmin/minioadmin)
echo.
echo To view logs:
echo   docker compose logs -f
echo.
echo To stop the system:
echo   docker-start.bat stop
echo.
goto :end

:stop
echo 🛑 Stopping AXDD system...
docker compose down
echo ✅ System stopped
goto :end

:restart
echo 🔄 Restarting AXDD system...
docker compose down
timeout /t 2 /nobreak >nul
call "%~f0" up
goto :end

:logs
docker compose logs -f
goto :end

:status
docker compose ps
goto :end

:clean
echo ⚠️  This will stop all services and remove all volumes (data will be lost!)
set /p confirm="Are you sure? (yes/no): "
if /i "%confirm%"=="yes" (
    docker compose down -v
    echo ✅ System stopped and volumes removed
) else (
    echo Cancelled
)
goto :end

:usage
echo Usage: %~nx0 {up^|down^|restart^|logs^|ps^|clean}
echo.
echo Commands:
echo   up       - Start the AXDD system
echo   down     - Stop the AXDD system
echo   restart  - Restart the AXDD system
echo   logs     - View logs from all services
echo   ps       - Show status of all services
echo   clean    - Stop and remove all data (use with caution!)
exit /b 1

:end
endlocal
