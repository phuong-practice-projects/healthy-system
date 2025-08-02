# Migration Management Guide

## EF Core Migration Commands

### 1. Add Migration
```bash
dotnet ef migrations add [MigrationName] --project Healthy.Infrastructure --startup-project Healthy.Api
```

### 2. Update Database
```bash
dotnet ef database update --project Healthy.Infrastructure --startup-project Healthy.Api
```

### 3. Remove Last Migration
```bash
dotnet ef migrations remove --project Healthy.Infrastructure --startup-project Healthy.Api
```

### 4. List Migrations
```bash
dotnet ef migrations list --project Healthy.Infrastructure --startup-project Healthy.Api
```

### 5. Generate SQL Script
```bash
dotnet ef migrations script --project Healthy.Infrastructure --startup-project Healthy.Api
```

### 6. Drop Database
```bash
dotnet ef database drop --project Healthy.Infrastructure --startup-project Healthy.Api
```

## Fluent API Configuration

All entity configurations are in `Healthy.Infrastructure/Persistence/Configurations/`:
- `UserConfiguration.cs` - User entity configuration
- `RoleConfiguration.cs` - Role entity configuration  
- `UserRoleConfiguration.cs` - UserRole entity configuration

## Best Practices

1. **Always use descriptive migration names**
   ```bash
   dotnet ef migrations add AddUserTable
   dotnet ef migrations add AddEmailIndexToUsers
   ```

2. **Review migration files before applying**
   - Check the generated SQL in the migration file
   - Ensure indexes and constraints are correct

3. **Use separate migrations for different features**
   - Don't combine multiple changes in one migration
   - Keep migrations focused and atomic

4. **Test migrations in development first**
   - Always test migrations before applying to production
   - Use `dotnet ef migrations script` to review SQL

5. **Backup before applying migrations in production**
   - Always backup database before applying migrations
   - Test rollback procedures 