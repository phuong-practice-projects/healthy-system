# Environment Configuration Guide

This document explains how to configure the Healthy System using environment variables and `.env` files.

## Overview

The application uses `.env` files for environment configuration instead of `appsettings.json`. This approach provides:

- **Better Security**: Environment variables are not committed to source control
- **Flexibility**: Easy to switch between different environments
- **Docker Integration**: Seamless integration with Docker containers
- **Development Friendly**: Clear separation of configuration by environment

## File Structure

```
healthy-system/
├── .env                    # Base environment variables (gitignored)
├── .env.example           # Template for environment variables (new format)
├── .env.github            # GitHub Actions environment
├── env.example            # Template for environment variables (legacy)
├── env.development        # Development-specific variables
├── docker-compose.dev.yml # Development Docker compose
├── docker-compose.prod.yml# Production Docker compose
└── Healthy/
    └── Healthy.Api/
        └── Program.cs     # Loads .env files using DotNetEnv
```

## Environment Files

### 1. `.env` (Base Configuration)
Contains common variables for all environments. **This file is gitignored for security.**

```bash
# Database Configuration
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=HealthyDB
DB_USER=sa

# Application Configuration  
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://+:8081;http://+:8080

# Redis Configuration
REDIS_PASSWORD=

# SSL Certificate (for HTTPS)
CERT_PASSWORD=password

# JWT Configuration
JWT_SECRET_KEY=your-super-secret-key-with-at-least-32-characters
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

# API Keys
API_KEY=your-api-key-here

# External Services
SMTP_HOST=
SMTP_PORT=587
SMTP_USERNAME=
SMTP_PASSWORD=

# Logging
LOG_LEVEL=Information
```

### 2. `env.development` (Development Configuration)
Overrides base variables for development environment with more detailed settings.

```bash
# Development Environment Variables
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true

# Database Settings (using connection string format)
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=HealthyDB_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=true;

# JWT Configuration (using JwtSettings format)
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=healthy-system-dev  
JwtSettings__Audience=healthy-system-api-dev
JwtSettings__ExpirationInMinutes=1440
JwtSettings__RefreshTokenExpirationInDays=7

# Redis Settings
ConnectionStrings__RedisConnection=healthy-redis:6379

# API Settings
API_PORT=8080
API_HOST=0.0.0.0

# Logging
LOG_LEVEL=Debug
LOG_LEVEL_MICROSOFT=Information

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

### 1. Initial Setup

```bash
# Copy the example file to create your .env (using new format)
cp .env.example .env

# Or use legacy format if preferred
cp env.example .env

# Edit the .env file with your specific values
notepad .env  # Windows
# or
nano .env     # Linux/Mac
```

### 2. Environment Loading Logic

The application loads environment variables using DotNetEnv library in this order:

1. **Automatic file selection**:
   - If `ASPNETCORE_ENVIRONMENT=Development` → loads `env.development`
   - Otherwise → loads `.env`

2. **File search locations**:
   - Project root directory (../../ from API project)
   - Current directory (fallback)

3. **Priority order**:
   - **System Environment Variables** (highest priority)
   - **Docker Environment Variables**
   - **Loaded .env file variables**
   - **appsettings.json** (fallback)

### 3. Docker Integration

The `docker-compose.dev.yml` file automatically loads the `env.development` file:

```yaml
services:
  healthy-api:
    env_file:
      - env.development
```

## Environment Service Integration

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

## Configuration Categories

### 1. Database Configuration

| Variable | Description | Example | Used In |
|----------|-------------|---------|---------|
| `DB_PASSWORD` | SQL Server password | `YourStrong@Passw0rd` | `.env` |
| `DB_NAME` | Database name | `HealthyDB` | `.env` |
| `DB_USER` | Database username | `sa` | `.env` |
| `ConnectionStrings__DefaultConnection` | Full connection string | `Server=localhost,1433;Database=HealthyDB_Dev;...` | `env.development` |

### 2. JWT Configuration

| Variable | Description | Example | Used In |
|----------|-------------|---------|---------|
| `JWT_SECRET_KEY` | JWT signing secret | `your-super-secret-key-with-at-least-32-characters` | `.env` |
| `JWT_ISSUER` | JWT issuer | `HealthySystem` | `.env` |
| `JWT_AUDIENCE` | JWT audience | `HealthySystemUsers` | `.env` |
| `JWT_EXPIRATION_MINUTES` | Token expiry time | `60` | `.env` |
| `JwtSettings__SecretKey` | JWT secret (settings format) | `dev-super-secret-key-...` | `env.development` |
| `JwtSettings__Issuer` | JWT issuer (settings format) | `healthy-system-dev` | `env.development` |

### 3. Application Configuration

| Variable | Description | Example | Used In |
|----------|-------------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` | Both |
| `ASPNETCORE_URLS` | Application URLs | `https://+:8081;http://+:8080` | Both |
| `API_PORT` | API port | `8080` | `env.development` |
| `API_HOST` | API host | `0.0.0.0` | `env.development` |

### 4. Redis Configuration

| Variable | Description | Example | Used In |
|----------|-------------|---------|---------|
| `REDIS_PASSWORD` | Redis password | `` | `.env` |
| `ConnectionStrings__RedisConnection` | Redis connection string | `healthy-redis:6379` | `env.development` |

### 5. Development Features

| Variable | Description | Default | Used In |
|----------|-------------|---------|---------|
| `ENABLE_SWAGGER` | Enable Swagger UI | `true` | `env.development` |
| `ENABLE_DETAILED_ERRORS` | Detailed error pages | `true` | `env.development` |
| `ENABLE_DEVELOPER_EXCEPTION_PAGE` | Developer exception page | `true` | `env.development` |
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