# Docker Setup for Healthy System

This project uses Docker and Docker Compose to run the application and related services.

## Prerequisites

- Docker Desktop
- Docker Compose

## File Structure

- `docker-compose.yml` - Main configuration for all services
- `docker-compose.override.yml` - Development overrides (automatically loaded)
- `docker-compose.dev.yml` - Simple configuration for development
- `docker-compose.prod.yml` - Production configuration
- `.env.example` - Environment variables template

## Usage

### 1. Development (Simple)

```powershell
# Run only API and Database
docker-compose -f docker-compose.dev.yml up -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop
docker-compose -f docker-compose.dev.yml down
```

### 2. Development (Full)

```powershell
# Create .env file from template
Copy-Item .env.example .env

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
Copy-Item .env.example .env.prod

# Update production values in .env.prod
# Especially DB_PASSWORD and API keys

# Deploy
docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d

# Or use Docker Swarm
docker stack deploy -c docker-compose.yml -c docker-compose.prod.yml healthy-stack
```

## Services

### healthy-api
- **Port**: 5000 (HTTP), 5001 (HTTPS)
- **Description**: Main .NET API
- **Health Check**: http://localhost:5000/health

### healthy-db
- **Port**: 1433
- **Description**: SQL Server Database
- **Credentials**: sa / YourStrong@Passw0rd (development)

### healthy-redis
- **Port**: 6379
- **Description**: Redis Cache
- **Usage**: Caching, Session storage

### healthy-nginx
- **Port**: 80 (HTTP), 443 (HTTPS)
- **Description**: Reverse proxy and load balancer
- **Config**: ./nginx/nginx.conf

## Useful Commands

```powershell
# Rebuild images
docker-compose build

# Run only database
docker-compose up healthy-db -d

# View logs for specific service
docker-compose logs -f healthy-api

# Connect to container
docker-compose exec healthy-api bash

# Cleanup volumes
docker-compose down -v

# View resource usage
docker stats

# Backup database
docker-compose exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE HealthyDB TO DISK = '/var/opt/mssql/backup/HealthyDB.bak'"
```

## Troubleshooting

### 1. Port conflicts
If ports are already in use, modify the ports in docker-compose files.

### 2. Permission issues
```powershell
# Windows: Run Docker Desktop as Administrator
# Linux/Mac: 
sudo chown -R $USER:$USER ./
```

### 3. Database connection issues
Check:
- Is healthy-db container running: `docker-compose ps`
- Connection string in appsettings.json
- Firewall settings

### 4. SSL Certificate issues
To create self-signed certificate for development:
```powershell
# Windows
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust

# Or use OpenSSL
mkdir nginx\ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout nginx\ssl\key.pem -out nginx\ssl\cert.pem
```

## Environment Variables

See `.env.example` file for all configurable environment variables.

Important variables:
- `DB_PASSWORD`: SQL Server password
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `JWT_SECRET_KEY`: Key for JWT authentication
- `ConnectionStrings__DefaultConnection`: Database connection string

## Monitoring

To add monitoring (Prometheus, Grafana):
```yaml
# Add to docker-compose.yml
  prometheus:
    image: prom/prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml

  grafana:
    image: grafana/grafana
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
```

## Backup & Restore

### Database Backup
```powershell
# Backup
docker-compose exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE HealthyDB TO DISK = '/var/opt/mssql/backup/HealthyDB.bak'"

# Restore
docker-compose exec healthy-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "RESTORE DATABASE HealthyDB FROM DISK = '/var/opt/mssql/backup/HealthyDB.bak'"
```

### Volume Backup
```powershell
# Backup volumes
docker run --rm -v healthy-system_healthy-db-data:/data -v ${PWD}:/backup alpine tar czf /backup/db-backup.tar.gz -C /data .

# Restore volumes
docker run --rm -v healthy-system_healthy-db-data:/data -v ${PWD}:/backup alpine tar xzf /backup/db-backup.tar.gz -C /data
```
