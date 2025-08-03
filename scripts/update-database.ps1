# PowerShell script to update database with correct connection string
# This script ensures SQL Server is running and applies migrations

param(
    [string]$Environment = "development"
)

Write-Host "🚀 Starting database update process..." -ForegroundColor Green

# Start SQL Server container if not running
Write-Host "📦 Ensuring SQL Server container is running..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml up healthy-db -d

# Wait for SQL Server to be ready
Write-Host "⏳ Waiting for SQL Server to be ready..." -ForegroundColor Yellow
$timeout = 60
$elapsed = 0
$interval = 2

do {
    $result = docker exec healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123" -Q "SELECT 1" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ SQL Server is ready!" -ForegroundColor Green
        break
    }
    Start-Sleep $interval
    $elapsed += $interval
    Write-Host "  Waiting... ($elapsed/$timeout seconds)" -ForegroundColor Gray
} while ($elapsed -lt $timeout)

if ($elapsed -ge $timeout) {
    Write-Host "❌ Timeout waiting for SQL Server to be ready" -ForegroundColor Red
    exit 1
}

# Apply migrations
Write-Host "🔄 Applying database migrations..." -ForegroundColor Yellow
$connectionString = "Server=localhost,1433;Database=HealthyDB_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=true;"

dotnet ef database update `
    --project "Healthy/Healthy.Infrastructure/Healthy.Infrastructure.csproj" `
    --startup-project "Healthy/Healthy.Api/Healthy.Api.csproj" `
    --connection $connectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Database updated successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Database update failed!" -ForegroundColor Red
    exit 1
}

Write-Host "🎉 Database update process completed!" -ForegroundColor Green
