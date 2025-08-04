#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Automated database migration script for Healthy System
.DESCRIPTION
    This script starts the database container, waits for it to be ready, and applies all pending migrations.
.PARAMETER Force
    Force restart the database container
.PARAMETER Verbose
    Show verbose output
.EXAMPLE
    .\scripts\update-database.ps1
.EXAMPLE
    .\scripts\update-database.ps1 -Force -Verbose
#>

param(
    [switch]$Force,
    [switch]$Verbose
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Colors for output
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = $Reset
    )
    Write-Host "${Color}${Message}${Reset}"
}

function Test-DatabaseConnection {
    Write-ColorOutput "🔍 Testing database connection..." $Blue
    try {
        # Wait for SQL Server to be ready - attempt connection
        $timeout = 60
        $elapsed = 0
        do {
            try {
                # Try to connect using sqlcmd in the container
                $result = docker exec healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "SELECT 1" -h -1 2>$null
                if ($LASTEXITCODE -eq 0) {
                    Write-ColorOutput "✅ Database connection successful" $Green
                    return $true
                }
            }
            catch {
                # Continue waiting
            }
            Start-Sleep -Seconds 2
            $elapsed += 2
            Write-ColorOutput "⏳ Waiting for database... ($elapsed/$timeout seconds)" $Yellow
        } while ($elapsed -lt $timeout)
        
        Write-ColorOutput "❌ Database connection timeout after $timeout seconds" $Red
        return $false
    }
    catch {
        Write-ColorOutput "❌ Failed to test database connection: $($_.Exception.Message)" $Red
        return $false
    }
}

try {
    Write-ColorOutput "🚀 Starting Healthy System Database Migration" $Blue
    Write-ColorOutput "=============================================" $Blue

    # Check if Docker is running
    Write-ColorOutput "🐳 Checking Docker status..." $Blue
    try {
        docker version | Out-Null
        Write-ColorOutput "✅ Docker is running" $Green
    }
    catch {
        Write-ColorOutput "❌ Docker is not running. Please start Docker and try again." $Red
        exit 1
    }

    # Navigate to project root
    $scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
    $projectRoot = Split-Path -Parent $scriptPath
    Set-Location $projectRoot
    Write-ColorOutput "📁 Working directory: $projectRoot" $Blue

    # Check if docker-compose file exists
    if (-not (Test-Path "docker-compose.dev.yml")) {
        Write-ColorOutput "❌ docker-compose.dev.yml not found" $Red
        exit 1
    }

    # Start or restart database container
    if ($Force) {
        Write-ColorOutput "🔄 Force restarting database container..." $Yellow
        docker-compose -f docker-compose.dev.yml down healthy-db
        docker-compose -f docker-compose.dev.yml up healthy-db -d
    }
    else {
        Write-ColorOutput "🔧 Starting database container..." $Blue
        docker-compose -f docker-compose.dev.yml up healthy-db -d
    }

    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Failed to start database container" $Red
        exit 1
    }

    Write-ColorOutput "✅ Database container started" $Green

    # Wait for database to be ready
    if (-not (Test-DatabaseConnection)) {
        Write-ColorOutput "❌ Database is not ready. Exiting." $Red
        exit 1
    }

    # Run migrations
    Write-ColorOutput "📊 Applying database migrations..." $Blue
    
    $migrationArgs = @(
        "ef", "database", "update",
        "--project", "Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj",
        "--startup-project", "Healthy\Healthy.Api\Healthy.Api.csproj"
    )
    
    if ($Verbose) {
        $migrationArgs += "--verbose"
    }

    & dotnet @migrationArgs

    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✅ Database migrations completed successfully!" $Green
        
        # Show migration status
        Write-ColorOutput "📋 Current migration status:" $Blue
        & dotnet ef migrations list --project "Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj" --startup-project "Healthy\Healthy.Api\Healthy.Api.csproj"
    }
    else {
        Write-ColorOutput "❌ Migration failed" $Red
        exit 1
    }

    Write-ColorOutput "🎉 Database update completed successfully!" $Green
    Write-ColorOutput "=============================================" $Blue
}
catch {
    Write-ColorOutput "❌ An error occurred: $($_.Exception.Message)" $Red
    Write-ColorOutput "Stack trace: $($_.ScriptStackTrace)" $Red
    exit 1
}
