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
├── env.example            # Template for environment variables
├── env.development        # Development-specific variables
├── env.production         # Production-specific variables (future)
└── Healthy/
    └── Healthy.Api/
        └── Program.cs     # Loads .env files
```

## Environment Files

### 1. `.env` (Base Configuration)
Contains common variables for all environments. **This file is gitignored for security.**

```bash
# Application Settings
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# Database Settings
DB_SERVER=healthy-db
DB_NAME=HealthyDB
DB_USER=sa
DB_PASSWORD=Dev@Passw0rd123
DB_TRUST_SERVER_CERTIFICATE=true

# Redis Settings
REDIS_HOST=healthy-redis
REDIS_PORT=6379

# API Settings
API_PORT=5001
API_HOST=0.0.0.0

# Logging
LOG_LEVEL=Information
LOG_LEVEL_MICROSOFT=Warning

# Security
JWT_SECRET=your-super-secret-jwt-key-here-change-in-production
JWT_ISSUER=healthy-system
JWT_AUDIENCE=healthy-system-api
JWT_EXPIRY_HOURS=24

# CORS
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:4200,http://localhost:8080

# Rate Limiting
RATE_LIMIT_PER_MINUTE=100
RATE_LIMIT_PER_HOUR=1000
```

### 2. `env.development` (Development Configuration)
Overrides base variables for development environment.

```bash
# Development Environment Variables
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true

# Database Settings
DB_SERVER=healthy-db
DB_NAME=HealthyDB_Dev
DB_USER=sa
DB_PASSWORD=Dev@Passw0rd123
DB_TRUST_SERVER_CERTIFICATE=true

# Development Features
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
ENABLE_DEVELOPER_EXCEPTION_PAGE=true

# Development - More Permissive Settings
LOG_LEVEL=Debug
RATE_LIMIT_PER_MINUTE=1000
RATE_LIMIT_PER_HOUR=10000
```

## Setup Instructions

### 1. Initial Setup

```bash
# Copy the example file to create your .env
cp env.example .env

# Edit the .env file with your specific values
notepad .env
```

### 2. Environment Variables Priority

The application reads environment variables in this order:

1. **System Environment Variables** (highest priority)
2. **Docker Environment Variables**
3. **`.env` file variables**
4. **`appsettings.json`** (fallback)

### 3. Docker Integration

The `docker-compose.dev.yml` file automatically loads the `env.development` file:

```yaml
services:
  healthy-api:
    env_file:
      - env.development
```

## Environment Service

The application includes an `IEnvironmentService` that provides type-safe access to environment variables:

```csharp
public interface IEnvironmentService
{
    string GetValue(string key);
    string GetValue(string key, string defaultValue);
    int GetIntValue(string key, int defaultValue = 0);
    bool GetBoolValue(string key, bool defaultValue = false);
    
    // Database specific
    string GetConnectionString();
    
    // Redis specific
    string GetRedisConnectionString();
    
    // JWT specific
    string GetJwtSecret();
    string GetJwtIssuer();
    string GetJwtAudience();
    int GetJwtExpiryHours();
    
    // CORS specific
    string[] GetCorsAllowedOrigins();
    
    // Rate limiting
    int GetRateLimitPerMinute();
    int GetRateLimitPerHour();
}
```

### Usage Example

```csharp
public class SomeService
{
    private readonly IEnvironmentService _envService;
    
    public SomeService(IEnvironmentService envService)
    {
        _envService = envService;
    }
    
    public void DoSomething()
    {
        var dbConnection = _envService.GetConnectionString();
        var redisConnection = _envService.GetRedisConnectionString();
        var jwtSecret = _envService.GetJwtSecret();
        var rateLimit = _envService.GetRateLimitPerMinute();
    }
}
```

## Configuration Categories

### 1. Database Configuration

| Variable | Description | Default |
|----------|-------------|---------|
| `DB_SERVER` | SQL Server hostname | `localhost` |
| `DB_NAME` | Database name | `HealthyDB` |
| `DB_USER` | Database username | `sa` |
| `DB_PASSWORD` | Database password | `` |
| `DB_TRUST_SERVER_CERTIFICATE` | Trust server certificate | `true` |

### 2. Redis Configuration

| Variable | Description | Default |
|----------|-------------|---------|
| `REDIS_HOST` | Redis hostname | `localhost` |
| `REDIS_PORT` | Redis port | `6379` |

### 3. Security Configuration

| Variable | Description | Default |
|----------|-------------|---------|
| `JWT_SECRET` | JWT signing secret | `your-super-secret-jwt-key-here-change-in-production` |
| `JWT_ISSUER` | JWT issuer | `healthy-system` |
| `JWT_AUDIENCE` | JWT audience | `healthy-system-api` |
| `JWT_EXPIRY_HOURS` | JWT expiry time | `24` |

### 4. API Configuration

| Variable | Description | Default |
|----------|-------------|---------|
| `API_PORT` | API port | `5001` |
| `API_HOST` | API host | `0.0.0.0` |
| `CORS_ALLOWED_ORIGINS` | CORS allowed origins | `http://localhost:3000,http://localhost:4200` |

### 5. Rate Limiting

| Variable | Description | Default |
|----------|-------------|---------|
| `RATE_LIMIT_PER_MINUTE` | Requests per minute | `100` |
| `RATE_LIMIT_PER_HOUR` | Requests per hour | `1000` |

## Environment-Specific Configurations

### Development

- **Logging**: Debug level
- **Rate Limiting**: More permissive (1000/min, 10000/hour)
- **Error Handling**: Detailed error pages
- **Swagger**: Enabled
- **Database**: `HealthyDB_Dev`

### Production (Future)

- **Logging**: Information level
- **Rate Limiting**: Strict (100/min, 1000/hour)
- **Error Handling**: Generic error pages
- **Swagger**: Disabled
- **Database**: `HealthyDB_Prod`

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
# Check if .env file exists
ls -la .env

# Check environment variables in container
docker-compose -f docker-compose.dev.yml exec healthy-api env | grep DB_
```

### 2. Database Connection Issues

```bash
# Test database connection
docker-compose -f docker-compose.dev.yml exec healthy-api dotnet ef database update
```

### 3. Redis Connection Issues

```bash
# Test Redis connection
docker-compose -f docker-compose.dev.yml exec healthy-redis redis-cli ping
```

## Migration from appsettings.json

If you're migrating from `appsettings.json` to `.env` files:

1. **Extract variables** from `appsettings.json`
2. **Create `.env` file** with extracted variables
3. **Update Program.cs** to load `.env` files
4. **Test configuration** in each environment
5. **Remove hardcoded values** from `appsettings.json`

## Next Steps

1. **Add Production Environment**: Create `env.production`
2. **Add Staging Environment**: Create `env.staging`
3. **Add Validation**: Validate required environment variables
4. **Add Encryption**: Encrypt sensitive environment variables
5. **Add Monitoring**: Monitor environment variable usage

## Commands Reference

```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# View environment variables
docker-compose -f docker-compose.dev.yml exec healthy-api env

# Restart with new environment
docker-compose -f docker-compose.dev.yml restart healthy-api

# Check environment service
curl http://localhost:5001/health
``` 