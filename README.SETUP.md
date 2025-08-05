# 🚀 Healthy System - Development Environment Setup Guide

> **Complete guide to setup and run Healthy System API from scratch**

## 📋 Table of Contents

1. [System Requirements](#system-requirements)
2. [Software Installation](#software-installation)
3. [Clone Project](#clone-project)
4. [Environment Configuration](#environment-configuration)
5. [Database Setup](#database-setup)
6. [Running the Application](#running-the-application)
7. [Verify Operation](#verify-operation)
8. [Troubleshooting](#troubleshooting)

---

## 🖥️ System Requirements

### Minimum Requirements
- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 20.04+)
- **RAM**: 8GB (16GB recommended)
- **Storage**: 10GB free space
- **Processor**: x64 architecture

### Software Requirements
- **.NET 9.0 SDK** - Main framework
- **Docker Desktop** - Container platform
- **Git** - Version control
- **IDE**: Visual Studio 2022 or VS Code (recommended)

---

## 💾 Software Installation

### 1. .NET 9.0 SDK
```powershell
# Check current version
dotnet --version

# Download from: https://dotnet.microsoft.com/download/dotnet/9.0
# Install and verify again
dotnet --version
```

### 2. Docker Desktop
```powershell
# Download from: https://www.docker.com/products/docker-desktop
# After installation, verify:
docker --version
docker-compose --version
```

### 3. Git
```powershell
# Check Git
git --version

# If not installed, download from: https://git-scm.com/
```

### 4. Visual Studio Code (Recommended)
Download from: https://code.visualstudio.com/

**Required Extensions:**
- C# Dev Kit
- Docker
- GitLens
- REST Client

---

## 📥 Clone Project

```powershell
# Clone repository
git clone <repository-url>

# Navigate to project directory
cd healthy-system

# Check project structure
dir  # Windows
ls   # macOS/Linux
```

---

## ⚙️ Environment Configuration

### 1. Create Environment File

```powershell
# Copy environment template file
Copy-Item env.example .env
```

### 2. Configure .env File

Open the `.env` file and configure environment variables:

```bash
# Database Configuration
DB_PASSWORD=Dev@Passw0rd123!
DB_NAME=HealthyDB
DB_USER=sa

# Application Configuration  
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-here-at-least-32-characters
JWT_ISSUER=HealthySystemAPI
JWT_AUDIENCE=HealthySystemClient
JWT_EXPIRE_MINUTES=60

# Database Connection String
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false

# Redis Configuration (Optional)
REDIS_CONNECTION_STRING=localhost:6379
REDIS_PASSWORD=

# SSL Certificate (for HTTPS - Optional)
CERT_PASSWORD=password
```

### 3. Verify Configuration

```powershell
# Check if .env file was created
Get-Content .env | Select-String "DB_PASSWORD"
```

---

## 🗄️ Database Setup

### Option 1: Using Docker (Recommended)

```powershell
# 1. Start SQL Server container
docker-compose -f docker-compose.dev.yml up healthy-db -d

# 2. Check running containers
docker ps

# 3. Wait 10-15 seconds for database to start completely
Start-Sleep -Seconds 15

# 4. Test database connection
docker exec healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "SELECT @@VERSION"
```

### Option 2: Local SQL Server

If you have SQL Server installed locally:

```powershell
# Update connection string in .env
# ConnectionStrings__DefaultConnection=Server=localhost;Database=HealthyDB;Integrated Security=true;TrustServerCertificate=true
```

### 3. Apply Database Migrations

```powershell
# Navigate to solution directory
cd Healthy

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj

# Check migration success
dotnet ef migrations list --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj
```

---

## 🚀 Running the Application

### Option 1: Using Docker (Development with Hot Reload)

```powershell
# Run full stack (API + Database)
docker-compose -f docker-compose.dev.yml up --build

# Or run in background
docker-compose -f docker-compose.dev.yml up --build -d

# View real-time logs
docker-compose -f docker-compose.dev.yml logs -f healthy-api
```

### Option 2: Run Locally (.NET CLI)

```powershell
# Ensure database container is running
docker-compose -f docker-compose.dev.yml up healthy-db -d

# Navigate to API directory
cd Healthy\Healthy.Api

# Run application
dotnet run

# Or run with hot reload
dotnet watch run
```

### Option 3: Visual Studio

1. Open `Healthy\Healthy.sln` in Visual Studio
2. Set `Healthy.Api` as startup project
3. Ensure database container is running
4. Press `F5` or click **Start**

---

## ✅ Verify Operation

### 1. Health Check Endpoints

```powershell
# Check API is running
curl http://localhost:5092/health
# Or with Docker:
curl http://localhost:5001/health
```

### 2. Swagger Documentation

Open browser and navigate to:
- **Local**: http://localhost:5092/swagger
- **Docker**: http://localhost:5001/swagger

### 3. Test API Endpoints

```powershell
# Test registration endpoint
curl -X POST http://localhost:5092/api/auth/register `
  -H "Content-Type: application/json" `
  -d '{
    "email": "test@example.com",
    "password": "Test@123456",
    "firstName": "Test",
    "lastName": "User"
  }'

# Test login endpoint
curl -X POST http://localhost:5092/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{
    "email": "test@example.com",
    "password": "Test@123456"
  }'
```

### 4. Database Verification

```powershell
# Connect and check tables
docker exec -it healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "USE HealthyDB; SELECT name FROM sys.tables;"
```

---

## 🛠️ Development Workflow

### 1. Daily Development

```powershell
# Start development day
docker-compose -f docker-compose.dev.yml up healthy-db -d
cd Healthy\Healthy.Api
dotnet watch run

# End development day
docker-compose -f docker-compose.dev.yml down
```

### 2. Code Changes and Migrations

```powershell
# After changing entities, create new migration
dotnet ef migrations add YourMigrationName --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj

# Apply migration
dotnet ef database update --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj
```

### 3. Testing

```powershell
# Run unit tests
cd Healthy\Healthy.Tests.Unit
dotnet test

# Run all tests
cd Healthy
dotnet test
```

---

## 🔧 Troubleshooting

### Database Connection Issues

```powershell
# Check SQL Server container
docker ps | Select-String "healthy-db"

# Restart database container
docker-compose -f docker-compose.dev.yml restart healthy-db

# Check database logs
docker-compose -f docker-compose.dev.yml logs healthy-db
```

### Port Conflicts

```powershell
# Check ports in use
netstat -an | Select-String ":5092|:1433|:5001"

# Kill process using port (if needed)
# Find PID: Get-Process -Name "dotnet" | Select-Object Id,ProcessName
# Stop-Process -Id <PID>
```

### Migration Errors

```powershell
# Reset database completely
dotnet ef database drop --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj --force

# Recreate database
dotnet ef database update --project Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy.Api\Healthy.Api.csproj
```

### Docker Issues

```powershell
# Clean Docker system
docker system prune -a

# Rebuild containers
docker-compose -f docker-compose.dev.yml build --no-cache
docker-compose -f docker-compose.dev.yml up
```

### Package Restore Issues

```powershell
# Clean and restore packages
cd Healthy
dotnet clean
dotnet restore
dotnet build
```

---

## 📚 Useful Commands Reference

### Docker Commands
```powershell
# Start only database
docker-compose -f docker-compose.dev.yml up healthy-db -d

# Start full stack
docker-compose -f docker-compose.dev.yml up

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop all services
docker-compose -f docker-compose.dev.yml down

# Remove volumes (reset database)
docker-compose -f docker-compose.dev.yml down -v
```

### .NET Commands
```powershell
# Run with hot reload
dotnet watch run

# Build solution
dotnet build

# Run tests
dotnet test

# Entity Framework
dotnet ef migrations add <name>
dotnet ef database update
dotnet ef migrations list
```

### Git Commands
```powershell
# Pull latest changes
git pull origin main

# Create feature branch
git checkout -b feature/your-feature-name

# Commit changes
git add .
git commit -m "Your commit message"
git push origin feature/your-feature-name
```

---

## 🎯 Next Steps

After successful setup, you can:

1. **Read documentation**: Review other README files to understand the architecture
2. **Explore API**: Use Swagger to test endpoints
3. **Development**: Start developing new features
4. **Testing**: Write unit tests for new code

## 📖 Related Documentation

- **[Authorization System](README.Authorization.md)** - Authorization system
- **[Database Relationships](README.Database-Relationships.md)** - Database relationships
- **[Migrations Guide](README.Migrations.md)** - Migration management
- **[Docker Setup](README.Docker.md)** - Docker details
- **[Environment Config](README.Environment.md)** - Environment configuration

---

## 🆘 Need Help?

- Check [Troubleshooting](#troubleshooting) section
- View logs for debugging: `docker-compose -f docker-compose.dev.yml logs -f`
- Create an issue on repository with detailed error information

**Happy Coding! 🎉**
