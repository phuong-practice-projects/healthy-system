# Docker Setup for Healthy System

This project uses Docker and Docker Compose to run the application and related services with development hot-reload capabilities.

## Prerequisites

- Docker Desktop
- Docker Compose
- .NET 9.0 SDK (for development)

## File Structure

- `docker-compose.yml` - Main configuration for all services
- `docker-compose.override.yml` - Development overrides (automatically loaded)
- `docker-compose.dev.yml` - Development configuration with hot reload
- `docker-compose.prod.yml` - Production configuration
- `env.development` - Development environment variables
- `env.example` - Environment variables template

## Usage

### 1. Development (Hot Reload) - Recommended

```powershell
# Run API with hot reload, Database, and Redis
docker-compose -f docker-compose.dev.yml up --build

# Run in background
docker-compose -f docker-compose.dev.yml up --build -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f healthy-api

# Stop and cleanup
docker-compose -f docker-compose.dev.yml down -v
```

### 2. Development (Full) - Advanced

```powershell
# Create .env file from template
Copy-Item env.example .env

# Edit .env file as needed
# notepad .env

# Run all services
docker-compose up -d

# View logs
docker-compose logs -f healthy-api

# Stop
docker-compose down
```

### 3. Production

```powershell
# Create .env file for production
Copy-Item env.example env.production

# Update production values in env.production
# Especially DB_PASSWORD and API keys

# Deploy
docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file env.production up -d

# Or use Docker Swarm
docker stack deploy -c docker-compose.yml -c docker-compose.prod.yml healthy-stack
```

## Services

### healthy-api
- **External Port**: 5001 (mapped to internal 8080)
- **Internal Port**: 8080
- **Description**: Main .NET 9.0 API with hot reload in development
- **Health Check**: http://localhost:5001/health
- **Swagger UI**: http://localhost:5001/swagger (development only)
- **Features**: 
  - Live code reloading during development
  - Multi-stage Docker build
  - Health checks with dependency waiting

### healthy-db
- **Port**: 1433
- **Description**: SQL Server 2022 Express Database
- **Credentials**: sa / Dev@Passw0rd123 (development)
- **Features**:
  - Persistent volume storage
  - Health checks for connection testing
  - Agent enabled for advanced features

### healthy-redis
- **External Port**: 6380 (mapped to internal 6379)
- **Internal Port**: 6379
- **Description**: Redis 7 Alpine for caching and sessions
- **Features**:
  - Persistent storage with AOF
  - Health checks with ping test
  - Internal service communication on port 6379

### Network
- **Name**: healthy-network
- **Type**: bridge
- **Description**: Internal Docker network for service communication

## Useful Commands

```powershell
# Rebuild images
docker-compose -f docker-compose.dev.yml build --no-cache

# Run only database (for external development)
docker-compose -f docker-compose.dev.yml up healthy-db -d

# Run only redis
docker-compose -f docker-compose.dev.yml up healthy-redis -d

# View logs for specific service
docker-compose -f docker-compose.dev.yml logs -f healthy-api
docker-compose -f docker-compose.dev.yml logs -f healthy-db
docker-compose -f docker-compose.dev.yml logs -f healthy-redis

# Check service health
docker-compose -f docker-compose.dev.yml ps

# Connect to API container (for debugging)
docker-compose -f docker-compose.dev.yml exec healthy-api bash

# Connect to database container
docker-compose -f docker-compose.dev.yml exec healthy-db bash

# Connect to Redis CLI
docker-compose -f docker-compose.dev.yml exec healthy-redis redis-cli

# Cleanup volumes (WARNING: This will delete all data)
docker-compose -f docker-compose.dev.yml down -v

# View resource usage
docker stats

# Remove unused Docker resources
docker system prune -f
```

## Troubleshooting

### 1. Port conflicts
If ports are already in use, modify the ports in docker-compose.dev.yml:
```yaml
ports:
  - "5002:8080"  # Change 5001 to 5002
```

### 2. Permission issues
```powershell
# Windows: Run Docker Desktop as Administrator
# Or run PowerShell as Administrator

# Linux/Mac: 
sudo chown -R $USER:$USER ./
```

### 3. Database connection issues
Check:
- Is healthy-db container running: `docker-compose -f docker-compose.dev.yml ps`
- Health check status: `docker-compose -f docker-compose.dev.yml ps healthy-db`
- Connection string in env.development uses `healthy-db` not `localhost`
- Firewall settings
- Container logs: `docker-compose -f docker-compose.dev.yml logs healthy-db`

### 4. Hot reload not working
Ensure:
- Source code is mounted correctly: `./Healthy:/src`
- File watcher is enabled: `DOTNET_USE_POLLING_FILE_WATCHER=true`
- No permission issues with mounted volumes
- VS Code or IDE is not holding file locks

### 5. Build issues
```powershell
# Clean rebuild
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml build --no-cache
docker-compose -f docker-compose.dev.yml up
```

### 6. Health check failures
Wait for services to be ready:
- Database: ~30-60 seconds first time
- Redis: ~10-15 seconds
- API: Will wait for DB and Redis to be healthy

### 7. Volume issues
```powershell
# List volumes
docker volume ls

# Remove specific volume
docker volume rm healthy-system_healthy-db-dev-data

# Inspect volume
docker volume inspect healthy-system_healthy-db-dev-data
```

## Environment Variables

See `env.development` for development configuration and `env.example` for template.

### Important Development Variables:
```env
# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
API_PORT=8080

# Database (uses service name for container communication)
ConnectionStrings__DefaultConnection=Server=healthy-db,1433;Database=HealthyDB_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=true;

# Redis (uses service name and internal port)
ConnectionStrings__RedisConnection=healthy-redis:6379

# JWT Configuration
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=healthy-system-dev
JwtSettings__Audience=healthy-system-api-dev

# Development Features
ENABLE_SWAGGER=true
ENABLE_DETAILED_ERRORS=true
AUTO_MIGRATE_DATABASE=true
SEED_DATABASE=true

# Hot Reload
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true
```

### Container-specific Variables:
- Services communicate using Docker network service names
- External ports differ from internal ports
- Development uses less secure but convenient settings

## Development Workflow

### 1. Start Development Environment
```powershell
# Clone and navigate to project
git clone <repository-url>
cd healthy-system

# Start all services
docker-compose -f docker-compose.dev.yml up --build
```

### 2. Development with Hot Reload
- API runs with `dotnet run` for hot reload
- Code changes in `./Healthy/` are automatically detected
- No need to rebuild container for code changes
- Database and Redis data persist between restarts

### 3. Access Points
- **API**: http://localhost:5001
- **Swagger**: http://localhost:5001/swagger
- **Database**: Server=localhost,1433 (from host machine)
- **Redis**: localhost:6380 (from host machine)

### 4. Database Operations
```powershell
# Connect to database from host
sqlcmd -S localhost,1433 -U sa -P Dev@Passw0rd123

# Or from container
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Dev@Passw0rd123
```

### 5. Debugging
- Attach debugger to process running in container
- Use `docker-compose -f docker-compose.dev.yml logs -f healthy-api` for real-time logs
- Enable detailed errors in development environment

## Architecture & Design

### Multi-Stage Dockerfile
- **Base**: Runtime image (aspnet:9.0)
- **Build**: SDK image for compilation
- **Development**: Uses build stage with source mounting for hot reload

### Service Dependencies
- API waits for DB and Redis to be healthy before starting
- Health checks ensure services are ready for connections
- Graceful startup sequence prevents connection errors

### Volume Management
- Source code mounted for development hot reload
- Persistent volumes for database and Redis data
- Excluded bin/obj folders prevent build conflicts

## Backup & Restore

### Database Backup
```powershell
# Create backup directory
docker-compose -f docker-compose.dev.yml exec healthy-db mkdir -p /var/opt/mssql/backup

# Backup database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Dev@Passw0rd123 -Q "BACKUP DATABASE HealthyDB_Dev TO DISK = '/var/opt/mssql/backup/HealthyDB_Dev.bak'"

# Copy backup to host
docker cp $(docker-compose -f docker-compose.dev.yml ps -q healthy-db):/var/opt/mssql/backup/HealthyDB_Dev.bak ./HealthyDB_Dev.bak

# Restore database
docker-compose -f docker-compose.dev.yml exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Dev@Passw0rd123 -Q "RESTORE DATABASE HealthyDB_Dev FROM DISK = '/var/opt/mssql/backup/HealthyDB_Dev.bak' WITH REPLACE"
```

### Volume Backup
```powershell
# Backup database volume
docker run --rm -v healthy-system_healthy-db-dev-data:/data -v ${PWD}:/backup alpine tar czf /backup/db-backup.tar.gz -C /data .

# Backup Redis volume
docker run --rm -v healthy-system_healthy-redis-dev-data:/data -v ${PWD}:/backup alpine tar czf /backup/redis-backup.tar.gz -C /data .

# Restore database volume
docker run --rm -v healthy-system_healthy-db-dev-data:/data -v ${PWD}:/backup alpine tar xzf /backup/db-backup.tar.gz -C /data

# Restore Redis volume
docker run --rm -v healthy-system_healthy-redis-dev-data:/data -v ${PWD}:/backup alpine tar xzf /backup/redis-backup.tar.gz -C /data
```

## Performance Tips

### Development
- Use `--build` only when Dockerfile changes
- Use `-d` flag to run in background during normal development
- Monitor resource usage with `docker stats`
- Clean up unused resources periodically with `docker system prune`

### Production Considerations
- Use multi-stage builds to reduce image size
- Configure proper resource limits
- Use Docker secrets for sensitive data
- Implement proper logging and monitoring
- Use Docker Swarm or Kubernetes for orchestration

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
