# Environment Configuration Guide

This document explains how to configure the Healthy System using environment variables and `.env` files.

**Current Version**: v1.2  
**Environment Format**: ASP.NET Core Configuration

## Overview

The application uses `.env` files for environment configuration instead of `appsettings.json`. This approach provides:

- **Better Security**: Environment variables are not committed to source control
- **Flexibility**: Easy to switch between different environments
- **Docker Integration**: Seamless integration with Docker containers
- **Development Friendly**: Clear separation of configuration by environment

## File Structure

```
healthy-system/
├── .env                     # Base environment variables (gitignored)
├── .env.example            # Template for environment variables (current format)
├── .env.development        # Development-specific variables (current)
├── .env.github             # GitHub Actions environment
├── docker-compose.dev.yml  # Development Docker compose
├── docker-compose.prod.yml # Production Docker compose
└── Healthy/
    └── Healthy.Api/
        └── Program.cs      # Loads .env files using DotNetEnv
```

## Current Environment Files Status

| File | Status | Purpose | Format |
|------|--------|---------|---------|
| `.env` | ❌ Not present | Base configuration | Key=Value |
| `.env.example` | ✅ Available | Template for new setup | Key=Value |  
| `.env.development` | ✅ Current active | Development settings | ASP.NET Config |
| `.env.github` | ✅ Available | CI/CD pipeline | Key=Value |

## Environment Files

### 1. `.env.development` (Current Active Configuration)
**Location**: `.env.development`  
**Status**: ✅ Active  
**Format**: ASP.NET Core Configuration  

Contains development-specific variables with full ASP.NET Core configuration format:

```bash
# Application Settings
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true

# Database Settings (Current Configuration)
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false

# JWT Configuration
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=healthy-system-dev
JwtSettings__Audience=healthy-system-api-dev
JwtSettings__ExpirationInMinutes=1440
JwtSettings__RefreshTokenExpirationInDays=7

# Redis Settings
ConnectionStrings__RedisConnection=healthy-redis:6379,password=DevRedis@Passw0rd123!
REDIS_HOST=healthy-redis
REDIS_PORT=6379
REDIS_PASSWORD=DevRedis@Passw0rd123!

# Logging Settings
LOGGING_LEVEL=Debug
SERILOG_MINIMUM_LEVEL=Debug

# CORS Settings (Development)
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:4200,http://localhost:8080,http://localhost:5001,https://localhost:3000,https://localhost:4200

# Feature Flags
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
ENABLE_DEVELOPER_EXCEPTION_PAGE=true

# Database Migration Settings
AUTO_MIGRATE_DATABASE=true
SEED_DATABASE=true
```

### 2. `.env.example` (Template Configuration)
**Location**: `.env.example`  
**Status**: ✅ Template  
**Format**: Simple Key=Value  

Template for basic environment setup:

```bash
# Database Configuration
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=HealthyDB
DB_USER=sa

# Application Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://+:8081;http://+:8080

# JWT Configuration
JWT_SECRET_KEY=your-super-secret-key-with-at-least-32-characters
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

# Logging
LOG_LEVEL=Information
```

### 3. Docker Compose Configuration
**Location**: `docker-compose.dev.yml`  
**Status**: ✅ Active  
**Format**: Docker Environment Variables  

Current Docker configuration for development:

```yaml
# API Service Environment
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:8080
  - DOTNET_USE_POLLING_FILE_WATCHER=true
  - DOTNET_RUNNING_IN_CONTAINER=true
  - ConnectionStrings__DefaultConnection=Server=healthy-db,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false

# Database Service Environment  
healthy-db:
  environment:
    - SA_PASSWORD=Dev@Passw0rd123!
    - ACCEPT_EULA=Y
    - MSSQL_PID=Express
```

# CORS (Development - more permissive)
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:4200,http://localhost:8080,http://localhost:5001

# Rate Limiting (Development - more permissive)
RATE_LIMIT_PER_MINUTE=1000
RATE_LIMIT_PER_HOUR=10000

# Development Features
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
ENABLE_DEVELOPER_EXCEPTION_PAGE=true

# Database Migration Settings
AUTO_MIGRATE_DATABASE=true
SEED_DATABASE=true

# Container Settings
CONTAINER_NAME=healthy-api-dev
COMPOSE_PROJECT_NAME=healthy-system-dev
```

## Setup Instructions

### 🚀 Quick Start (Development)

The project is **already configured** for development with `.env.development`. No additional setup needed for local development.

```bash
# 1. Start the development environment
docker-compose -f docker-compose.dev.yml up -d

# 2. Run migrations (if needed)
.\scripts\update-database.ps1

# 3. Access the application
# API: http://localhost:5001
# Swagger: http://localhost:5001/swagger
```

### 🔧 Custom Configuration Setup

If you need to customize the environment variables:

```bash
# Option 1: Copy the example file to create your .env
cp .env.example .env

# Option 2: Create a custom development file
cp .env.development .env.development.local

# Edit the file with your specific values
notepad .env                    # Windows
# or
nano .env                       # Linux/Mac
```

### 🐳 Docker Environment Variables

The Docker setup uses environment variables directly in `docker-compose.dev.yml`. No `.env` file loading required for containers.

**Current Active Configuration:**
- **Database**: HealthyDB  
- **Server**: healthy-db (container) / localhost:1433 (host)
- **Password**: Dev@Passw0rd123!
- **API Port**: 5001 (host) → 8080 (container)

### 🔄 Environment Loading Logic

The application loads environment variables using DotNetEnv library in this order:

1. **Docker Environment** (highest priority when running in container)
2. **File selection logic**:
   - If `ASPNETCORE_ENVIRONMENT=Development` → loads `.env.development`
   - Otherwise → loads `.env`
3. **System Environment Variables** 
4. **appsettings.json** (fallback)

### 📁 File Search Locations

### 📁 File Search Locations
Environment files are searched in the following locations:
- **Project root directory** (recommended): `d:\Phuong\Projects\healthy-system\`
- **API project directory**: `Healthy\Healthy.Api\`
- **Current working directory** (fallback)

### 🐳 Docker Integration

Docker containers get environment variables directly from `docker-compose.dev.yml` configuration. The containers do **not** load `.env` files.

**Current Docker Environment Configuration:**

```yaml
# API Service
healthy-api:
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - ASPNETCORE_URLS=http://+:8080
    - ConnectionStrings__DefaultConnection=Server=healthy-db,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false

# Database Service
healthy-db:
  environment:
    - SA_PASSWORD=Dev@Passw0rd123!
    - ACCEPT_EULA=Y
    - MSSQL_PID=Express
```

**Key Points:**
- ✅ No `.env` file needed for Docker containers
- ✅ Database password consistent across services
- ✅ Connection string uses container service names
- ✅ Host development can use `.env.development` for different settings

## 🔧 Environment Service Integration

The application uses ASP.NET Core's built-in configuration system combined with DotNetEnv for environment variable loading. Configuration is accessed through `IConfiguration`:

```csharp
public class SomeService
{
    private readonly IConfiguration _configuration;
    
    public SomeService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void DoSomething()
    {
        // Access connection strings
        var dbConnection = _configuration.GetConnectionString("DefaultConnection");
        var redisConnection = _configuration.GetConnectionString("RedisConnection");
        
        // Access JWT settings
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        
        // Access simple values
        var apiPort = _configuration["API_PORT"];
        var enableSwagger = _configuration.GetValue<bool>("ENABLE_SWAGGER");
    }
}
```

### Configuration Binding

```csharp
// In Program.cs or Startup.cs
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Usage in service
public class AuthService
{
    private readonly JwtSettings _jwtSettings;
    
    public AuthService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
}
```

## 📊 Current Active Configuration (August 5, 2025)

### Development Environment Status
| Component | Status | Configuration Source | Value/Connection |
|-----------|--------|---------------------|------------------|
| **API** | ✅ Active | docker-compose.dev.yml | http://localhost:5001 |
| **Database** | ✅ Active | docker-compose.dev.yml | localhost:1433 (HealthyDB) |
| **Swagger** | ✅ Enabled | .env.development | http://localhost:5001/swagger |
| **Hot Reload** | ✅ Enabled | docker-compose.dev.yml | dotnet watch |
| **Environment Files** | ✅ Active | .env.development | ASP.NET Core format |

### Key Configuration Values
```bash
# Database (Current Active)
Database Name: HealthyDB
Server: healthy-db (container) / localhost:1433 (host)
Password: Dev@Passw0rd123!
User: sa

# API
Port: 5001 (host) → 8080 (container)
Environment: Development
Swagger: Enabled at /swagger
Hot Reload: Enabled

# Security
JWT Secret: dev-super-secret-key-with-at-least-32-characters-for-development
JWT Expiration: 24 hours (1440 minutes)
CORS: Localhost origins enabled

# Features
Auto Migration: Enabled
Database Seeding: Enabled
Detailed Errors: Enabled
```

### 🔧 Quick Troubleshooting

#### Issue: Environment variables not loading
```bash
# Check if .env.development exists
ls -la .env.development

# Verify docker-compose environment section
docker-compose -f docker-compose.dev.yml config
```

#### Issue: Database connection failed
```bash
# Check if database container is running
docker ps | findstr healthy-db

# Test database connection
docker exec healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "SELECT 1"
```

#### Issue: Wrong configuration being loaded
```bash
# Check which environment is active
echo $ASPNETCORE_ENVIRONMENT

# For containers, check environment variables
docker exec healthy-system-healthy-api-1 env | grep ASPNETCORE
```

## Configuration Categories

### 1. Database Configuration

| Variable | Description | Current Value | Used In |
|----------|-------------|---------------|---------|
| `ConnectionStrings__DefaultConnection` | Full connection string | `Server=healthy-db,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false` | docker-compose.dev.yml |
| `SA_PASSWORD` | SQL Server SA password | `Dev@Passw0rd123!` | docker-compose.dev.yml (DB service) |
| `DB_PASSWORD` | Database password (template) | `YourStrong@Passw0rd` | .env.example |
| `DB_NAME` | Database name (template) | `HealthyDB` | .env.example |

### 2. JWT Configuration

| Variable | Current Value | Used In | Purpose |
|----------|---------------|---------|---------|
| `JwtSettings__SecretKey` | `dev-super-secret-key-with-at-least-32-characters-for-development` | .env.development | JWT signing |
| `JwtSettings__Issuer` | `healthy-system-dev` | .env.development | Token issuer |
| `JwtSettings__Audience` | `healthy-system-api-dev` | .env.development | Token audience |
| `JwtSettings__ExpirationInMinutes` | `1440` (24 hours) | .env.development | Token expiry |
| `JwtSettings__RefreshTokenExpirationInDays` | `7` | .env.development | Refresh token expiry |

### 3. Application Configuration

| Variable | Current Value | Used In | Purpose |
|----------|---------------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | `Development` | Both docker-compose & .env.development | Environment mode |
| `ASPNETCORE_URLS` | `http://+:8080` | docker-compose.dev.yml | Container binding |
| `DOTNET_USE_POLLING_FILE_WATCHER` | `true` | Both | Hot reload support |
| `DOTNET_RUNNING_IN_CONTAINER` | `true` | Both | Container detection |

### 4. Redis Configuration (Optional)

| Variable | Current Value | Used In | Purpose |
|----------|---------------|---------|---------|
| `ConnectionStrings__RedisConnection` | `healthy-redis:6379,password=DevRedis@Passw0rd123!` | .env.development | Cache connection |
| `REDIS_HOST` | `healthy-redis` | .env.development | Redis server |
| `REDIS_PORT` | `6379` | .env.development | Redis port |
| `REDIS_PASSWORD` | `DevRedis@Passw0rd123!` | .env.development | Redis password |

### 5. Development Features

| Variable | Current Value | Used In | Purpose |
|----------|---------------|---------|---------|
| `ENABLE_SWAGGER` | `true` | .env.development | Swagger UI |
| `ENABLE_DETAILED_ERRORS` | `true` | .env.development | Error details |
| `ENABLE_DEVELOPER_EXCEPTION_PAGE` | `true` | .env.development | Exception page |
| `AUTO_MIGRATE_DATABASE` | `true` | .env.development | Auto migrations |
| `SEED_DATABASE` | `true` | .env.development | Seed data |
| `AUTO_MIGRATE_DATABASE` | Auto run migrations | `true` | `env.development` |
| `SEED_DATABASE` | Seed sample data | `true` | `env.development` |

## Environment-Specific Configurations

### Development (`env.development`)

- **Environment**: `Development`
- **Logging**: Debug level for application, Information for Microsoft
- **Rate Limiting**: More permissive (1000/min, 10000/hour)
- **Error Handling**: Detailed error pages enabled
- **Swagger**: Enabled for API documentation
- **Database**: `HealthyDB_Dev` with auto-migration and seeding
- **CORS**: Permissive for local development ports
- **Container**: Named containers for Docker development

### Production (`docker-compose.prod.yml`)

- **Environment**: `Production`
- **Logging**: Information level
- **Rate Limiting**: Strict (100/min, 1000/hour)
- **Error Handling**: Generic error pages
- **Swagger**: Disabled for security
- **Database**: Production database with manual migrations
- **CORS**: Restricted to specific domains
- **SSL**: HTTPS enabled with certificates

### GitHub Actions (`.env.github`)

- **Environment**: Special configuration for CI/CD
- **Database**: In-memory or test database
- **Secrets**: Loaded from GitHub Secrets
- **Services**: Mock services for testing

## Security Best Practices

### 1. Never Commit Sensitive Data

```bash
# ✅ Good - Use .env files (gitignored)
JWT_SECRET=your-actual-secret-here

# ❌ Bad - Don't commit secrets to source control
JWT_SECRET=my-secret-key
```

### 2. Use Different Secrets per Environment

```bash
# Development
JWT_SECRET=dev-secret-key-123

# Production
JWT_SECRET=prod-super-secure-key-456
```

### 3. Rotate Secrets Regularly

```bash
# Generate new secrets periodically
JWT_SECRET=$(openssl rand -base64 32)
```

## Troubleshooting

### 1. Environment Variables Not Loading

```bash
# Check if env file exists in project root
ls -la env.development

# Check environment loading in Program.cs output
docker-compose -f docker-compose.dev.yml up healthy-api
# Look for: "✅ Loaded environment file: ..."

# Check environment variables in container
docker-compose -f docker-compose.dev.yml exec healthy-api env | grep -i connection
```

### 2. Database Connection Issues

```bash
# Test database connection with Entity Framework
docker-compose -f docker-compose.dev.yml exec healthy-api dotnet ef database update

# Check database container
docker-compose -f docker-compose.dev.yml logs healthy-db

# Test SQL connection manually
docker-compose -f docker-compose.dev.yml exec healthy-db sqlcmd -S localhost -U sa -P "Dev@Passw0rd123"
```

### 3. JWT Configuration Issues

```bash
# Check JWT settings are loaded
docker-compose -f docker-compose.dev.yml exec healthy-api env | grep JWT

# Verify JWT settings in logs
docker-compose -f docker-compose.dev.yml logs healthy-api | grep -i jwt
```

### 4. Redis Connection Issues

```bash
# Test Redis connection
docker-compose -f docker-compose.dev.yml exec healthy-redis redis-cli ping

# Check Redis logs
docker-compose -f docker-compose.dev.yml logs healthy-redis
```

### Common Issues and Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Env file not found | Wrong path or filename | Check file exists in project root |
| Database connection failed | Wrong connection string | Verify `ConnectionStrings__DefaultConnection` |
| JWT errors | Missing or invalid secret | Check `JwtSettings__SecretKey` length (32+ chars) |
| CORS errors | Wrong origins | Update `CORS_ALLOWED_ORIGINS` |
| Container startup fails | Environment conflicts | Check `CONTAINER_NAME` uniqueness |

## Migration Guide

### From appsettings.json to Environment Variables

If you're migrating from `appsettings.json` to environment variables:

1. **Map Configuration Sections**:
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=..."
     },
     "JwtSettings": {
       "SecretKey": "...",
       "Issuer": "..."
     }
   }
   ```
   
   ```bash
   # env.development
   ConnectionStrings__DefaultConnection=Server=...
   JwtSettings__SecretKey=...
   JwtSettings__Issuer=...
   ```

2. **Update Program.cs** to load environment files:
   ```csharp
   // Add DotNetEnv package
   var envFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
       ? "env.development" : ".env";
   Env.Load(envFile);
   ```

3. **Test Configuration** in each environment
4. **Remove sensitive data** from `appsettings.json`

### Environment File Format Changes

| Old Format | New Format | Notes |
|------------|------------|-------|
| `env.example` | `.env.example` | Dot prefix for better tooling support |
| Simple key=value | Structured keys | Support for nested configuration |
| `JWT_SECRET` | `JwtSettings__SecretKey` | Follow ASP.NET Core conventions |

## Commands Reference

```bash
# Development Environment Management
docker-compose -f docker-compose.dev.yml up -d          # Start development
docker-compose -f docker-compose.dev.yml down           # Stop development
docker-compose -f docker-compose.dev.yml restart healthy-api  # Restart API only

# Production Environment Management  
docker-compose -f docker-compose.prod.yml up -d         # Start production
docker-compose -f docker-compose.prod.yml down          # Stop production

# Environment Inspection
docker-compose -f docker-compose.dev.yml exec healthy-api env                    # View all variables
docker-compose -f docker-compose.dev.yml exec healthy-api env | grep CONNECTION  # Check connections
docker-compose -f docker-compose.dev.yml exec healthy-api env | grep JWT         # Check JWT settings

# Database Operations
docker-compose -f docker-compose.dev.yml exec healthy-api dotnet ef database update  # Run migrations
docker-compose -f docker-compose.dev.yml exec healthy-api dotnet ef migrations add NewMigration  # Add migration

# Health Checks
curl http://localhost:8080/health              # API health check
curl http://localhost:8080/swagger             # Swagger UI (dev only)

# Logs and Debugging
docker-compose -f docker-compose.dev.yml logs healthy-api     # API logs
docker-compose -f docker-compose.dev.yml logs healthy-db      # Database logs  
docker-compose -f docker-compose.dev.yml logs healthy-redis   # Redis logs
docker-compose -f docker-compose.dev.yml logs -f              # Follow all logs

# Environment File Management
cp .env.example .env                           # Create base environment
cp env.example .env                           # Create legacy format environment  
echo "DB_PASSWORD=NewPassword" >> .env        # Add/update variables
```

## Best Practices Summary

### ✅ DO

- Use `.env.example` as template for team members
- Keep sensitive data in `.env` (gitignored)  
- Use structured configuration keys (`JwtSettings__SecretKey`)
- Test environment loading in each environment
- Use strong passwords and secrets (32+ characters)
- Document all environment variables
- Use different secrets per environment

### ❌ DON'T

- Commit `.env` files to source control
- Use weak or default passwords in production
- Mix environment-specific settings in base `.env`
- Hardcode secrets in source code
- Use production secrets in development
- Share environment files via insecure channels

## Security Checklist

- [ ] All `.env` files are in `.gitignore`
- [ ] Production secrets are different from development
- [ ] JWT secrets are 32+ characters long
- [ ] Database passwords are strong
- [ ] CORS origins are properly configured
- [ ] HTTPS is enabled in production
- [ ] Environment variables are validated on startup
- [ ] Secrets are rotated regularly

---

## 📝 Changelog

### August 5, 2025
- ✅ **Updated configuration documentation**: Reflected current active configuration
- ✅ **Corrected database settings**: Updated from `HealthyDB_Dev` to `HealthyDB`
- ✅ **Fixed password references**: Updated to match docker-compose (`Dev@Passw0rd123!`)
- ✅ **Added current status section**: Live configuration values and troubleshooting
- ✅ **Updated file structure**: Reflected actual files in project
- ✅ **Enhanced setup instructions**: Added quick start for immediate development
- ✅ **Improved Docker integration**: Clarified container vs host configuration

### Current Status
- **Active Environment**: Development with Docker
- **Configuration Format**: ASP.NET Core Configuration (double underscore format)
- **Primary Config File**: `.env.development`
- **Docker Integration**: Direct environment variables in docker-compose.dev.yml
- **Database**: HealthyDB on SQL Server 2022 Express 