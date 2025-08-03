# Database Migration Guide

## Overview
This guide explains how to manage Entity Framework Core migrations for the Healthy System project. The project uses **IDesignTimeDbContextFactory** for automatic connection string handling and includes detailed commands for both CMD/PowerShell and Visual Studio Package Manager Console.

## Quick Start

### Method 1: Automated Script (Recommended)
```powershell
# Run the automated PowerShell script
.\scripts\update-database.ps1

```

### Method 2: Using Package Manager Console (Visual Studio)

```powershell
# 1. Set Healthy.Api as Startup Project (right-click project → Set as Startup Project)
# 2. Open Package Manager Console (Tools → NuGet Package Manager → Package Manager Console)
# 3. Set Default project to "Healthy.Infrastructure" in the dropdown
# 4. Start database container first
docker-compose -f docker-compose.dev.yml up healthy-db -d

# 5. Run migration (connection string is handled automatically)
Update-Database
```

### Method 3: Using .NET CLI / CMD

```cmd
rem Start database container first
docker-compose -f docker-compose.dev.yml up healthy-db -d

rem Wait for database to be ready (10 seconds)
timeout /t 10

rem Run migration (connection string is handled automatically by DesignTimeDbContextFactory)
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

## Migration Management Commands

### Adding New Migration

#### Package Manager Console (Visual Studio):
```powershell
# Prerequisites:
# 1. Set Healthy.Api as Startup Project
# 2. Set Healthy.Infrastructure as Default project in PMC dropdown
# 3. Ensure database container is running

# Add new migration
Add-Migration YourMigrationName

# Examples:
Add-Migration AddUserProfileTable
Add-Migration UpdateRolePermissions
Add-Migration AddIndexesToUserTable
```

#### CMD / PowerShell CLI:
```cmd
rem Ensure database container is running
docker-compose -f docker-compose.dev.yml up healthy-db -d

rem Add new migration
dotnet ef migrations add YourMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Examples:
dotnet ef migrations add AddUserProfileTable --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
dotnet ef migrations add UpdateRolePermissions --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Applying Migrations (Update Database)

#### Package Manager Console:
```powershell
# Apply all pending migrations
Update-Database

# Apply migrations up to a specific migration
Update-Database -Migration "MigrationName"

# Apply migrations with verbose output
Update-Database -Verbose
```

#### CMD / PowerShell CLI:
```cmd
rem Apply all pending migrations
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Apply migrations up to a specific migration
dotnet ef database update MigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Apply migrations with verbose output
dotnet ef database update --verbose --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Reverting Migrations

#### Package Manager Console:
```powershell
# Revert to a specific migration (this will undo migrations after the specified one)
Update-Database -Migration "PreviousMigrationName"

# Revert to initial state (remove all migrations from database)
Update-Database -Migration 0

# Example: Revert to InitialCreate migration
Update-Database -Migration "InitialCreateWithSeedData"
```

#### CMD / PowerShell CLI:
```cmd
rem Revert to a specific migration
dotnet ef database update PreviousMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Revert to initial state (remove all migrations from database)
dotnet ef database update 0 --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Example: Revert to InitialCreate migration
dotnet ef database update InitialCreateWithSeedData --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Removing Migrations (From Code)

#### Package Manager Console:
```powershell
# Remove the last migration (only if it hasn't been applied to database yet)
Remove-Migration

# Remove multiple migrations (run multiple times)
Remove-Migration
Remove-Migration
```

#### CMD / PowerShell CLI:
```cmd
rem Remove the last migration (only if it hasn't been applied to database yet)
dotnet ef migrations remove --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Remove with force (use with caution)
dotnet ef migrations remove --force --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Listing Migrations

#### Package Manager Console:
```powershell
# List all migrations
Get-Migration

# List migrations with details
Get-Migration -Verbose
```

#### CMD / PowerShell CLI:
```cmd
rem List all migrations
dotnet ef migrations list --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem List migrations with details
dotnet ef migrations list --verbose --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Generating SQL Scripts

#### Package Manager Console:
```powershell
# Generate SQL script for all migrations
Script-Migration

# Generate SQL script from specific migration to latest
Script-Migration -From "FromMigrationName"

# Generate SQL script between two migrations
Script-Migration -From "FromMigrationName" -To "ToMigrationName"

# Generate script and save to file
Script-Migration -Output "migration-script.sql"
```

#### CMD / PowerShell CLI:
```cmd
rem Generate SQL script for all migrations
dotnet ef migrations script --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Generate SQL script from specific migration to latest
dotnet ef migrations script FromMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Generate SQL script between two migrations
dotnet ef migrations script FromMigrationName ToMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Generate script and save to file
dotnet ef migrations script --output migration-script.sql --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

## Complete Workflow Examples

### Example 1: Adding a New Entity and Migration

**Scenario**: Adding a new `Product` entity

#### Step 1: Create the Entity (in Healthy.Domain/Entities/)
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

#### Step 2: Update DbContext (in Healthy.Infrastructure/Persistence/ApplicationDbContext.cs)
```csharp
public DbSet<Product> Products { get; set; }
```

#### Step 3: Add Migration
**Using Package Manager Console:**
```powershell
Add-Migration AddProductEntity
```

**Using CMD:**
```cmd
dotnet ef migrations add AddProductEntity --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Step 4: Review Generated Migration
Check the generated file in `Healthy.Infrastructure/Migrations/` and verify the SQL operations.

#### Step 5: Apply Migration
**Using Package Manager Console:**
```powershell
Update-Database
```

**Using CMD:**
```cmd
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Example 2: Rolling Back a Migration

**Scenario**: Need to rollback the last 2 migrations

#### Step 1: List Current Migrations
**Using Package Manager Console:**
```powershell
Get-Migration
```

**Using CMD:**
```cmd
dotnet ef migrations list --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Step 2: Identify Target Migration
From the output, identify the migration you want to rollback to (e.g., `AddMealEntity`).

#### Step 3: Rollback Database
**Using Package Manager Console:**
```powershell
Update-Database -Migration "AddMealEntity"
```

**Using CMD:**
```cmd
dotnet ef database update AddMealEntity --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Step 4: Remove Migration Files (Optional)
If you want to remove the migration files from code:

**Using Package Manager Console:**
```powershell
Remove-Migration
Remove-Migration
```

**Using CMD:**
```cmd
dotnet ef migrations remove --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
dotnet ef migrations remove --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Example 3: Production Deployment Workflow

#### Step 1: Generate Production SQL Script
**Using Package Manager Console:**
```powershell
Script-Migration -Output "production-migration.sql"
```

**Using CMD:**
```cmd
dotnet ef migrations script --output production-migration.sql --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Step 2: Review SQL Script
Manually review the generated SQL script for any issues.

#### Step 3: Backup Production Database
```sql
-- Connect to production SQL Server
BACKUP DATABASE [HealthyDB_Prod] 
TO DISK = 'C:\Backups\HealthyDB_Prod_Backup.bak'
```

#### Step 4: Apply Script to Production
Execute the SQL script against the production database during maintenance window.

### Example 4: Development Environment Reset

**Scenario**: Need to completely reset development database

#### Step 1: Stop and Remove Database Container
```cmd
docker-compose -f docker-compose.dev.yml down -v
```

#### Step 2: Start Fresh Database Container
```cmd
docker-compose -f docker-compose.dev.yml up healthy-db -d
```

#### Step 3: Wait for Database to Be Ready
```cmd
timeout /t 15
```

#### Step 4: Apply All Migrations
**Using Package Manager Console:**
```powershell
Update-Database
```

**Using CMD:**
```cmd
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

## Database Schema

### Current Tables
- **Users**: User account information with BCrypt password hashing
- **Roles**: System roles (Admin, User, Moderator)
- **UserRoles**: User-Role relationships with soft delete support
- **__EFMigrationsHistory**: Migration tracking

### Seed Data (Included in Migrations)
The following data is automatically seeded via Fluent API:

**Roles:**
- **Admin** (ID: 11111111-1111-1111-1111-111111111111): System Administrator with full access
- **User** (ID: 22222222-2222-2222-2222-222222222222): Regular user with basic access
- **Moderator** (ID: 33333333-3333-3333-3333-333333333333): Moderator with limited admin access

**Default Users:**
- **Admin**: admin@healthysystem.com / Admin@123 (ID: aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa)
- **Test User**: user@healthysystem.com / User@123 (ID: bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb)

**Password Hashing**: BCrypt with cost factor 11

## Architecture Configuration

### IDesignTimeDbContextFactory
The project uses **ApplicationDbContextFactory** for automatic connection string handling:

- **Location**: `Healthy.Infrastructure/Factories/ApplicationDbContextFactory.cs`
- **Benefits**: Automatically handles connection string conversion between Docker and localhost
- **Automatic**: EF Core automatically uses this factory for migrations and design-time operations

### Connection String Handling
The factory automatically handles different environments:

1. **Docker Environment**: Uses `healthy-db` as server name
2. **Local Development**: Converts `healthy-db` to `localhost` for migrations
3. **Fallback**: Uses hardcoded localhost connection string

**Current Configuration**:
- **Server**: localhost:1433 (for migrations)
- **Database**: HealthyDB_Dev
- **User**: sa
- **Password**: Dev@Passw0rd123

### Migration Assembly
- **Migrations Location**: `Healthy.Infrastructure` project
- **Factory Configuration**: Automatically detected by EF Core
- **Commands**: Can be run from any directory

## Quick Reference Commands

### Essential Commands Cheat Sheet

#### Package Manager Console (Visual Studio):
```powershell
# Prerequisites: Set Healthy.Api as Startup Project, Healthy.Infrastructure as Default project

# Add migration
Add-Migration MigrationName

# Update database
Update-Database

# Rollback to specific migration
Update-Database -Migration "MigrationName"

# Remove last migration (if not applied)
Remove-Migration

# List migrations
Get-Migration

# Generate SQL script
Script-Migration -Output "script.sql"
```

#### CMD / PowerShell:
```cmd
rem Prerequisites: Ensure database container is running

rem Add migration
dotnet ef migrations add MigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Update database
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Rollback to specific migration
dotnet ef database update MigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Remove last migration (if not applied)
dotnet ef migrations remove --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem List migrations
dotnet ef migrations list --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Generate SQL script
dotnet ef migrations script --output script.sql --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### Development Environment Setup

#### Visual Studio Package Manager Console:
1. **Set Projects**: 
   - Right-click `Healthy.Api` → Set as Startup Project
   - In PMC dropdown, select `Healthy.Infrastructure`
2. **Start Database**: `docker-compose -f docker-compose.dev.yml up healthy-db -d`
3. **Run Commands**: Use PMC commands above

#### CMD / PowerShell Setup:
1. **Start Database**: `docker-compose -f docker-compose.dev.yml up healthy-db -d`
2. **Wait for Ready**: `timeout /t 10`
3. **Run Commands**: Use CMD commands above (connection string handled automatically)

## Entity Framework Commands

### Method 1: Package Manager Console (Recommended for Visual Studio)

Open Package Manager Console in Visual Studio (`Tools > NuGet Package Manager > Package Manager Console`) and set **Default project** to `Healthy.Infrastructure`:

```powershell
# List migrations
Get-Migration

# Add new migration
Add-Migration MigrationName

# Apply migrations to database
Update-Database

# Remove last migration (if not applied to database)
Remove-Migration

# Generate SQL script
Script-Migration

# Update to specific migration
Update-Database -Migration "MigrationName"

# Generate script between migrations
Script-Migration -From "FromMigration" -To "ToMigration"
```

**Important**: Make sure to:
1. Set `Healthy.Api` as **Startup Project** (right-click → Set as Startup Project)
2. Set `Healthy.Infrastructure` as **Default project** in Package Manager Console dropdown
3. Ensure environment variables are set before running commands

### Method 2: .NET CLI Commands

All EF Core commands should be run with the correct project references:

```powershell
# Navigate to Infrastructure project (contains migrations)
cd Healthy.Infrastructure

# List migrations
dotnet ef migrations list --startup-project ../Healthy.Api

# Add new migration
dotnet ef migrations add MigrationName --startup-project ../Healthy.Api

# Apply migrations to database
dotnet ef database update --startup-project ../Healthy.Api

# Remove last migration (if not applied to database)
dotnet ef migrations remove --startup-project ../Healthy.Api

# Generate SQL script
dotnet ef migrations script --startup-project ../Healthy.Api
```

### Database Management

```powershell
# Create database and apply all migrations
dotnet ef database update --startup-project ../Healthy.Api

# Reset database (drop and recreate)
dotnet ef database drop --startup-project ../Healthy.Api
dotnet ef database update --startup-project ../Healthy.Api

# Check migration status
dotnet ef migrations list --startup-project ../Healthy.Api
```

## Docker Database Management

### Container Operations
```powershell
# Start database container
docker-compose up -d database

# View database logs
docker-compose logs database

# Stop database container
docker-compose down database

# Connect to database container
docker exec -it healthy-system-database-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -C
```

### Database Verification
```sql
-- List databases
SELECT name FROM sys.databases;

-- Use the Healthy System database
USE HealthySystemDb;

-- List tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check seed data
SELECT * FROM Roles;
SELECT * FROM Users;
SELECT * FROM UserRoles;

-- Verify migrations applied
SELECT * FROM __EFMigrationsHistory;
```
## Troubleshooting

### 1. Database Connection Issues

#### Check Container Status:
```cmd
rem Check if container is running
docker ps | findstr healthy-db

rem Check container logs
docker-compose -f docker-compose.dev.yml logs healthy-db

rem Restart database container
docker-compose -f docker-compose.dev.yml restart healthy-db
```

#### Test Connection:
```cmd
rem Connect to database container manually
docker exec -it healthy-system-healthy-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123" -Q "SELECT 1"
```

### 2. Migration Errors

#### Check Migration Status:
**Package Manager Console:**
```powershell
Get-Migration
```

**CMD:**
```cmd
dotnet ef migrations list --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Fix Migration Issues:
**Package Manager Console:**
```powershell
# Remove problematic migration (if not applied to database)
Remove-Migration

# Create new migration
Add-Migration FixedMigrationName

# Apply migration
Update-Database
```

**CMD:**
```cmd
rem Remove problematic migration (if not applied to database)
dotnet ef migrations remove --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Create new migration
dotnet ef migrations add FixedMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj

rem Apply migration
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### 3. Build Errors

#### Check Project Build:
```cmd
rem Build Infrastructure project
dotnet build Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj

rem Build API project
dotnet build Healthy\Healthy.Api\Healthy.Api.csproj

rem Build entire solution
dotnet build Healthy\Healthy.sln
```

### 4. Factory Configuration Issues

#### Verify Factory is Working:
The `ApplicationDbContextFactory` automatically handles connection strings. If you see connection issues:

1. **Check Factory File**: Ensure `Healthy.Infrastructure/Factories/ApplicationDbContextFactory.cs` exists
2. **Verify Dependencies**: Check that required NuGet packages are installed
3. **Test Factory**: The factory automatically converts `healthy-db` to `localhost` for migrations

### 5. Database Reset (Complete Fresh Start)

#### Full Environment Reset:
```cmd
rem Stop and remove all containers and volumes
docker-compose -f docker-compose.dev.yml down -v

rem Start fresh database container
docker-compose -f docker-compose.dev.yml up healthy-db -d

rem Wait for database to be ready
timeout /t 15

rem Apply all migrations
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

### 6. Common Error Solutions

#### Error: "Project file does not exist"
**Solution**: Use full paths in commands:
```cmd
dotnet ef database update --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### Error: "No DbContext was found"
**Solution**: Ensure `ApplicationDbContextFactory` exists and projects are built successfully.

#### Error: "A network-related error occurred"
**Solution**: 
1. Ensure Docker container is running
2. Wait for SQL Server to be fully ready (can take 15-30 seconds)
3. Check container logs for SQL Server startup issues

#### Error: "Migration has already been applied"
**Solution**: This is normal - it means database is up to date.

### 7. Verification Commands

#### Verify Database Content:
```sql
-- Connect to database and verify tables
USE HealthyDB_Dev;

-- List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check migration history
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;

-- Verify seed data
SELECT COUNT(*) as RoleCount FROM Roles;
SELECT COUNT(*) as UserCount FROM Users;
SELECT COUNT(*) as UserRoleCount FROM UserRoles;
```

## Best Practices

### 1. Migration Naming Conventions
- **Use descriptive names**: `AddUserProfile`, `UpdateRolePermissions`, `AddIndexesToUserTable`
- **Include date for major changes**: `20250803_AddUserProfile`
- **Use PascalCase**: `AddProductEntity`, `UpdateUserEmailIndex`
- **Be specific**: Instead of `UpdateUser` use `AddUserPhoneNumberField`

### 2. Development Workflow Best Practices

#### Before Creating Migration:
1. **Update entity models** in `Healthy.Domain/Entities/`
2. **Update DbContext** if adding new DbSet
3. **Build solution** to ensure no compilation errors
4. **Review changes** to ensure they're correct

#### Creating Migration:
**Visual Studio (Recommended for development):**
```powershell
# 1. Set Healthy.Api as Startup Project
# 2. Open Package Manager Console
# 3. Set Healthy.Infrastructure as Default project
Add-Migration DescriptiveMigrationName
```

**Command Line:**
```cmd
dotnet ef migrations add DescriptiveMigrationName --project Healthy\Healthy.Infrastructure\Healthy.Infrastructure.csproj --startup-project Healthy\Healthy.Api\Healthy.Api.csproj
```

#### After Creating Migration:
1. **Review generated migration** in `Healthy.Infrastructure/Migrations/`
2. **Verify SQL operations** are correct
3. **Test on development database** before committing
4. **Commit migration files** to version control

### 3. Database Management

#### Development Environment:
- **Always test migrations** on development database first
- **Use automated scripts** for consistent deployment
- **Keep development database in sync** with latest migrations
- **Reset development database** periodically for clean state

#### Production Deployment:
1. **Generate SQL scripts** for production deployment
2. **Review scripts manually** before applying
3. **Backup production database** before applying migrations
4. **Apply during maintenance windows** to minimize downtime
5. **Verify application functionality** after migration

### 4. Error Prevention

#### Common Mistakes to Avoid:
- ❌ **Don't edit applied migrations** - create new ones instead
- ❌ **Don't delete migration files** that have been applied to database
- ❌ **Don't apply untested migrations** to production
- ❌ **Don't ignore migration warnings** - review them carefully

#### Good Practices:
- ✅ **Always backup before applying migrations** to production
- ✅ **Test migrations on development** environment first
- ✅ **Review generated SQL** before applying to production
- ✅ **Use descriptive migration names** for easier tracking
- ✅ **Keep migrations small and focused** on single changes

### 5. Team Collaboration

#### When Working in Team:
1. **Pull latest changes** before creating new migrations
2. **Communicate about database changes** with team members
3. **Resolve migration conflicts** by rebasing and recreating migrations
4. **Document breaking changes** in migration comments
5. **Coordinate deployment** of database changes across environments

## Project Structure
```
healthy-system/
├── scripts/
│   └── update-database.ps1           # Automated migration script
├── update-database.bat               # Batch file for Windows
├── Healthy.Infrastructure/
│   ├── Factories/
│   │   └── ApplicationDbContextFactory.cs  # Design-time DbContext factory
│   ├── Migrations/                   # EF Core migrations
│   │   ├── 20250802075047_InitialCreateWithSeedData.cs
│   │   ├── 20250802090333_AddColumnEntity.cs
│   │   ├── 20250802095835_AddMyRecordEntities.cs
│   │   ├── 20250802191201_AddMealEntity.cs
│   │   ├── 20250803083425_UpdateBaseEntitiesAndAuditFields.cs
│   │   └── ApplicationDbContextModelSnapshot.cs
│   ├── Persistence/
│   │   └── ApplicationDbContext.cs   # DbContext with Fluent API seeding
│   └── DependencyInjection.cs       # Service registration
├── Healthy.Api/
│   ├── appsettings.json             # Base configuration
│   └── appsettings.Development.json # Development configuration with connection string
├── docker-compose.dev.yml           # Development Docker configuration
└── README.Migrations.md            # This documentation
```

## Current Database Configuration

### Connection String Details:
- **Server**: localhost:1433 (for migrations from host)
- **Database**: HealthyDB_Dev
- **Username**: sa
- **Password**: Dev@Passw0rd123
- **Options**: TrustServerCertificate=true

### Docker Container:
- **Container Name**: healthy-system-healthy-db-1
- **Image**: mcr.microsoft.com/mssql/server:2022-latest
- **Port Mapping**: 1433:1433
- **Volume**: healthy-db-dev-data

## Available Scripts

### PowerShell Script (Recommended):
```powershell
# Run automated migration script
.\scripts\update-database.ps1
```

### Batch File:
```cmd
rem Run batch file for migration
.\update-database.bat
```

### Manual Commands:
See sections above for detailed Package Manager Console and CMD commands.
