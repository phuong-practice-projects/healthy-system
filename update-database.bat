@echo off
setlocal EnableDelayedExpansion

REM Automated database migration script for Healthy System
REM This script starts the database container and applies all pending migrations

echo.
echo ========================================
echo   Healthy System Database Migration
echo ========================================
echo.

REM Check if Docker is running
echo [INFO] Checking Docker status...
docker version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running. Please start Docker and try again.
    pause
    exit /b 1
)
echo [SUCCESS] Docker is running

REM Navigate to project root
cd /d "%~dp0"
cd ..

REM Check if docker-compose file exists
if not exist "docker-compose.dev.yml" (
    echo [ERROR] docker-compose.dev.yml not found
    pause
    exit /b 1
)

REM Start database container
echo [INFO] Starting database container...
docker-compose -f docker-compose.dev.yml up healthy-db -d
if errorlevel 1 (
    echo [ERROR] Failed to start database container
    pause
    exit /b 1
)
echo [SUCCESS] Database container started

REM Wait for database to be ready
echo [INFO] Waiting for database to be ready...
timeout /t 15 /nobreak >nul

REM Test database connection
echo [INFO] Testing database connection...
set "connectionTest=false"
for /L %%i in (1,1,10) do (
    docker exec healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "SELECT 1" -h -1 >nul 2>&1
    if not errorlevel 1 (
        set "connectionTest=true"
        goto :connectionSuccess
    )
    echo [INFO] Attempt %%i/10 - waiting for database...
    timeout /t 3 /nobreak >nul
)

:connectionSuccess
if "%connectionTest%"=="false" (
    echo [ERROR] Database connection failed after multiple attempts
    pause
    exit /b 1
)
echo [SUCCESS] Database connection established

REM Apply migrations
echo [INFO] Applying database migrations...
dotnet ef database update --project "Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj" --startup-project "Healthy\Healthy.Api\Healthy.Api.csproj"

if errorlevel 1 (
    echo [ERROR] Migration failed
    pause
    exit /b 1
)

echo [SUCCESS] Database migrations completed successfully!

REM Show migration status
echo [INFO] Current migration status:
dotnet ef migrations list --project "Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj" --startup-project "Healthy\Healthy.Api\Healthy.Api.csproj"

echo.
echo ========================================
echo   Database update completed successfully!
echo ========================================
echo.
pause
