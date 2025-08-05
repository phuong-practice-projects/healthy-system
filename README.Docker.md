# Docker Setup for Healthy System

This project uses Docker and Docker Compose to run the application and related services with development hot-reload capabilities.

## Prerequisites

- Docker Desktop
- Docker Compose
- .NET 9.0 SDK (for development)

## File Structure

- `docker-compose.dev.yml` - Development configuration with hot reload
- `docker-compose.prod.yml` - Production configuration
- `Healthy/Healthy.Api/Dockerfile` - Multi-stage Docker build
- `nginx/nginx.conf` - Nginx reverse proxy configuration
- `scripts/update-database.ps1` - Database migration script

## Quick Start

### Development Environment

```powershell
# Start all services with hot reload
docker-compose -f docker-compose.dev.yml up --build

# Run in background
docker-compose -f docker-compose.dev.yml up --build -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f healthy-api

# Stop and cleanup
docker-compose -f docker-compose.dev.yml down
```

### Production Environment

```powershell
# Create environment file
Copy-Item env.example env.production

# Update production values in env.production
# Set DB_PASSWORD and other production secrets

# Deploy with production configuration
docker-compose -f docker-compose.prod.yml --env-file env.production up -d
```

## Services

### healthy-api
- **External Port**: 5001 (mapped to internal 8080)
- **Internal Port**: 8080
- **Description**: .NET 9.0 API with hot reload in development
- **Health Check**: http://localhost:5001/health
- **Swagger UI**: http://localhost:5001/swagger (development only)
- **Features**: 
  - Live code reloading during development
  - Multi-stage Docker build
  - JWT Authentication
  - Role-based authorization
  - Comprehensive API documentation

### healthy-db
- **Port**: 1433
- **Description**: SQL Server 2022 Express Database
- **Credentials**: sa / Dev@Passw0rd123! (development)
- **Features**:
  - Persistent volume storage
  - Health checks for connection testing
  - Agent enabled for advanced features
  - Automatic database seeding

### healthy-nginx (Production)
- **Ports**: 80, 443
- **Description**: Nginx reverse proxy with SSL termination
- **Features**:
  - SSL/TLS encryption
  - Rate limiting (10 requests/second)
  - Security headers
  - Load balancing
  - HTTP to HTTPS redirection

### Network
- **Name**: healthy-network
- **Type**: bridge
- **Description**: Internal Docker network for service communication

## Development Workflow

### 1. Start Development Environment
```powershell
# Clone and navigate to project
git clone <repository-url>
cd healthy-system

# Start all services with hot reload
docker-compose -f docker-compose.dev.yml up --build
```

### 2. Development with Hot Reload
- API runs with `dotnet watch run` for hot reload
- Code changes in `./Healthy/` are automatically detected
- No need to rebuild container for code changes
- Database data persists between restarts

### 3. Access Points
- **API**: http://localhost:5001
- **Swagger**: http://localhost:5001/swagger
- **Database**: Server=localhost,1433 (from host machine)
- **Health Check**: http://localhost:5001/health

### 4. Database Operations
```powershell
# Run database migrations
.\scripts\update-database.ps1

# Connect to database from host
sqlcmd -S localhost,1433 -U sa -P "Dev@Passw0rd123!"

# Or from container
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!"
```

## Environment Configuration

### Development Environment Variables
The application automatically loads environment variables from `.env.development` file:

```env
# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true

# Database
ConnectionStrings__DefaultConnection=Server=healthy-db,1433;Database=HealthyDB;User Id=sa;Password=Dev@Passw0rd123!;TrustServerCertificate=true;Encrypt=false

# JWT Configuration
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=HealthySystem
JwtSettings__Audience=HealthySystemUsers
JwtSettings__ExpirationInMinutes=60

# Development Features
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
AUTO_MIGRATE_DATABASE=true
SEED_DATABASE=true
```

### Production Environment Variables
For production, create a `.env.production` file:

```env
# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database
DB_PASSWORD=your-secure-production-password
ConnectionStrings__DefaultConnection=Server=healthy-db;Database=HealthyDB;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=true;

# JWT Configuration
JWT_SECRET=your-secure-jwt-secret-key
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRY_HOURS=24

# Security
ENABLE_SWAGGER=false
ENABLE_DETAILED_ERRORS=false
```

## Dockerfile Architecture

### Multi-Stage Build
The Dockerfile uses a multi-stage build approach:

```dockerfile
# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# ... build process

# Publish stage
FROM build AS publish
# ... publish process

# Final runtime stage
FROM base AS final
# ... runtime configuration
```

### Development vs Production
- **Development**: Uses build stage with source mounting for hot reload
- **Production**: Uses final stage with optimized runtime image

## Useful Commands

### Development Commands
```powershell
# Rebuild images
docker-compose -f docker-compose.dev.yml build --no-cache

# Run only database (for external development)
docker-compose -f docker-compose.dev.yml up healthy-db -d

# View logs for specific service
docker-compose -f docker-compose.dev.yml logs -f healthy-api
docker-compose -f docker-compose.dev.yml logs -f healthy-db

# Check service health
docker-compose -f docker-compose.dev.yml ps

# Connect to API container (for debugging)
docker-compose -f docker-compose.dev.yml exec healthy-api bash

# Connect to database container
docker-compose -f docker-compose.dev.yml exec healthy-db bash
```

### Production Commands
```powershell
# Deploy production stack
docker-compose -f docker-compose.prod.yml --env-file env.production up -d

# View production logs
docker-compose -f docker-compose.prod.yml logs -f

# Scale API instances
docker-compose -f docker-compose.prod.yml up --scale healthy-api=3 -d

# Update production deployment
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d
```

### Maintenance Commands
```powershell
# Cleanup volumes (WARNING: This will delete all data)
docker-compose -f docker-compose.dev.yml down -v

# View resource usage
docker stats

# Remove unused Docker resources
docker system prune -f

# Backup database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "BACKUP DATABASE HealthyDB TO DISK = '/var/opt/mssql/backup/HealthyDB.bak'"
```

## Troubleshooting

### 1. Port Conflicts
If ports are already in use, modify the ports in docker-compose.dev.yml:
```yaml
ports:
  - "5002:8080"  # Change 5001 to 5002
```

### 2. Database Connection Issues
Check:
- Is healthy-db container running: `docker-compose -f docker-compose.dev.yml ps`
- Connection string uses `healthy-db` not `localhost`
- Container logs: `docker-compose -f docker-compose.dev.yml logs healthy-db`
- Run database migration script: `.\scripts\update-database.ps1`

### 3. Hot Reload Not Working
Ensure:
- Source code is mounted correctly: `./Healthy:/src`
- File watcher is enabled: `DOTNET_USE_POLLING_FILE_WATCHER=true`
- No permission issues with mounted volumes
- VS Code or IDE is not holding file locks

### 4. Build Issues
```powershell
# Clean rebuild
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml build --no-cache
docker-compose -f docker-compose.dev.yml up
```

### 5. Permission Issues
```powershell
# Windows: Run Docker Desktop as Administrator
# Or run PowerShell as Administrator

# Linux/Mac: 
sudo chown -R $USER:$USER ./
```

### 6. Health Check Failures
Wait for services to be ready:
- Database: ~30-60 seconds first time
- API: Will wait for DB to be healthy

## Security Features

### Development Environment
- JWT authentication with development keys
- Role-based authorization
- CORS configured for development
- Detailed error messages for debugging

### Production Environment
- JWT authentication with secure keys
- Role-based authorization with strict policies
- Nginx reverse proxy with SSL/TLS
- Rate limiting (10 requests/second)
- Security headers (HSTS, XSS Protection, etc.)
- No external database port exposure
- Resource limits and reservations

## Performance Optimization

### Development
- Use `--build` only when Dockerfile changes
- Use `-d` flag to run in background during normal development
- Monitor resource usage with `docker stats`
- Clean up unused resources periodically with `docker system prune`

### Production
- Multi-stage builds reduce image size
- Resource limits prevent container resource exhaustion
- Nginx load balancing for multiple API instances
- SSL/TLS termination at nginx level
- Rate limiting prevents abuse

## Backup & Restore

### Database Backup
```powershell
# Create backup directory
docker-compose -f docker-compose.dev.yml exec healthy-db mkdir -p /var/opt/mssql/backup

# Backup database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "BACKUP DATABASE HealthyDB TO DISK = '/var/opt/mssql/backup/HealthyDB.bak'"

# Copy backup to host
docker cp $(docker-compose -f docker-compose.dev.yml ps -q healthy-db):/var/opt/mssql/backup/HealthyDB.bak ./HealthyDB.bak

# Restore database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "RESTORE DATABASE HealthyDB FROM DISK = '/var/opt/mssql/backup/HealthyDB.bak' WITH REPLACE"
```

### Volume Backup
```powershell
# Backup database volume
docker run --rm -v healthy-system_healthy-db-data:/data -v ${PWD}:/backup alpine tar czf /backup/db-backup.tar.gz -C /data .

# Restore database volume
docker run --rm -v healthy-system_healthy-db-data:/data -v ${PWD}:/backup alpine tar xzf /backup/db-backup.tar.gz -C /data
```

## API Features

The Docker setup supports all current API features:

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (Admin, User, Moderator, etc.)
- Custom authorization attributes
- Current user context

### Health Tracking
- Body records management
- Exercise tracking
- Meal and nutrition tracking
- Diary entries

### Content Management
- Column/article management
- Category organization
- Dashboard analytics

### API Documentation
- Swagger UI (development)
- Comprehensive endpoint documentation
- Request/response examples
- Authentication documentation

## Quick Reference

### Start/Stop Commands
```powershell
# Quick start development
docker-compose -f docker-compose.dev.yml up -d

# Stop and remove
docker-compose -f docker-compose.dev.yml down

# Reset everything (WARNING: Deletes all data)
docker-compose -f docker-compose.dev.yml down -v
docker system prune -f
```

### Monitoring Commands
```powershell
# Service status
docker-compose -f docker-compose.dev.yml ps

# Resource usage
docker stats

# Container logs
docker-compose -f docker-compose.dev.yml logs -f [service-name]
```

### Database Commands
```powershell
# Run migrations
.\scripts\update-database.ps1

# Connect to database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!"

# Check database status
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123!" -Q "SELECT name, state_desc FROM sys.databases"
```

## Future Enhancements

1. **Redis Integration**: Add Redis for caching and session management
2. **Monitoring**: Add Prometheus and Grafana for metrics
3. **Logging**: Centralized logging with ELK stack
4. **CI/CD**: Automated Docker builds and deployments
5. **Kubernetes**: Kubernetes deployment manifests
6. **Service Mesh**: Istio for advanced traffic management
7. **Secrets Management**: Docker secrets for sensitive data
8. **Health Checks**: Advanced health check endpoints
9. **Auto-scaling**: Horizontal pod autoscaling
10. **Blue-Green Deployment**: Zero-downtime deployments
