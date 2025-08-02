# JWT Configuration Guide

## Overview
This guide explains how JWT settings have been moved from `appsettings.json` to environment variables for better security and configuration management.

## What Changed

### Before
JWT settings were hardcoded in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "HealthySystem",
    "Audience": "HealthySystemUsers",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### After
JWT settings are now configured via environment variables:
- `JwtSettings__SecretKey` → `JWT_SECRET_KEY`
- `JwtSettings__Issuer` → `JWT_ISSUER`
- `JwtSettings__Audience` → `JWT_AUDIENCE`
- `JwtSettings__ExpirationInMinutes` → `JWT_EXPIRATION_MINUTES`
- `JwtSettings__RefreshTokenExpirationInDays` → `JWT_REFRESH_TOKEN_EXPIRATION_DAYS`

## Configuration Files

### 1. `.env` (Main environment file)
```env
JWT_SECRET_KEY=your-super-secret-key-with-at-least-32-characters
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7
```

### 2. `.env.example` (Template file)
Contains example values and documentation for all environment variables.

### 3. `env.development` (Development specific)
Contains development-specific JWT settings with a development-only secret key.

## Docker Compose Integration

### Development
- `docker-compose.dev.yml` uses `env.development` file
- Contains development-safe JWT settings

### Production
- `docker-compose.yml` and `docker-compose.override.yml` use `.env` file
- Uses environment variable substitution: `${JWT_SECRET_KEY}`

## Usage in Different Environments

### Local Development (without Docker)
1. Create `.env` file from `.env.example`
2. Update JWT settings as needed
3. The .NET application will automatically read these environment variables

### Docker Development
```bash
# Uses env.development automatically
docker-compose -f docker-compose.dev.yml up -d
```

### Docker Production
```bash
# Uses .env file
docker-compose up -d
```

### Production Deployment
1. Create production `.env` file with secure values
2. Use strong, randomly generated JWT secret key (at least 32 characters)
3. Set appropriate issuer and audience for your domain

## Security Best Practices

### 1. JWT Secret Key
- **Minimum 32 characters**
- Use cryptographically secure random string
- Different for each environment
- Never commit to version control

### 2. Generate Secure Key
```bash
# Using OpenSSL
openssl rand -base64 32

# Using PowerShell
[System.Web.Security.Membership]::GeneratePassword(32, 0)

# Using Node.js
node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
```

### 3. Environment-Specific Values
- **Development**: Use recognizable but secure keys
- **Staging**: Use production-like but different keys
- **Production**: Use strongly generated, unique keys

## Code Changes Required

### If using IConfiguration in your code:
```csharp
// Old way (still works)
var secretKey = configuration["JwtSettings:SecretKey"];

// New way (recommended)
var secretKey = configuration["JWT_SECRET_KEY"] ?? configuration["JwtSettings:SecretKey"];
```

### Using IOptions pattern:
```csharp
// Configure in Program.cs or Startup.cs
services.Configure<JwtSettings>(options =>
{
    options.SecretKey = configuration["JWT_SECRET_KEY"] ?? configuration["JwtSettings:SecretKey"];
    options.Issuer = configuration["JWT_ISSUER"] ?? configuration["JwtSettings:Issuer"];
    options.Audience = configuration["JWT_AUDIENCE"] ?? configuration["JwtSettings:Audience"];
    // ... other settings
});
```

## Troubleshooting

### 1. JWT not working after changes
- Verify environment variables are set correctly
- Check Docker compose files are using correct env files
- Ensure no typos in environment variable names

### 2. Environment variables not loading
- Check `.env` file exists and has correct format
- Verify Docker compose `env_file` configuration
- Make sure no BOM (Byte Order Mark) in .env files

### 3. Development vs Production keys
- Development uses `env.development`
- Production uses `.env`
- Verify you're using the correct compose file

## Files Modified
- ✅ `appsettings.json` - Removed hardcoded JWT values
- ✅ `.env.example` - Added JWT environment variables
- ✅ `.env` - Added JWT configuration
- ✅ `env.development` - Added development JWT settings
- ✅ `docker-compose.yml` - Added JWT environment variables
- ✅ `docker-compose.override.yml` - Added JWT environment variables

## Next Steps
1. Generate secure JWT secret keys for each environment
2. Update your JWT service/middleware to use the new configuration
3. Test the application in both development and production modes
4. Document the new environment variables for your team
