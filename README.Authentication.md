# JWT Authentication Configuration Guide

## Overview
This guide explains the JWT authentication configuration in the Healthy System. The application uses ASP.NET Core configuration patterns with environment variables for flexible deployment across different environments.

## Current JWT Configuration

### JWT Settings Model
```csharp
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 60;
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
```

### Environment Variable Support
The application supports two formats with fallback logic:

**Primary Format (ASP.NET Core Standard):**
- `JwtSettings__SecretKey`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JWT_EXPIRY_HOURS` (converted to minutes)

**Fallback Format (Simple Environment Variables):**
- `JWT_SECRET`
- `JWT_ISSUER`
- `JWT_AUDIENCE`

## Configuration Files

### 1. `.env.development` (Development Environment)
Current development configuration:
```env
# JWT Configuration (ASP.NET Core format)
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=HealthySystem
JwtSettings__Audience=HealthySystemUsers

# Alternative format also supported
JWT_EXPIRY_HOURS=24
```

### 2. `.env.example` (Template file)
Example configuration showing both supported formats:
```env
# JWT Configuration (Primary - ASP.NET Core format)
JwtSettings__SecretKey=your-super-secret-key-with-at-least-32-characters
JwtSettings__Issuer=HealthySystem
JwtSettings__Audience=HealthySystemUsers

# JWT Configuration (Alternative format)
JWT_SECRET=your-super-secret-key-with-at-least-32-characters
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRY_HOURS=24
```

### 3. Fallback Configuration
The system includes built-in fallbacks for development:
- **Default Secret**: `dev-super-secret-key-with-at-least-32-characters-for-development`
- **Default Issuer**: `HealthySystem`
- **Default Audience**: `HealthySystemUsers`
- **Default Expiration**: 60 minutes
- **Refresh Token Expiration**: 7 days

## Docker Compose Integration

### Development Environment
- Uses `docker-compose.dev.yml` with `.env.development` file
- JWT configuration loaded automatically via environment variables
- Development-safe JWT settings with recognizable secret key

### Production Environment  
- Uses `docker-compose.prod.yml` with environment variable substitution
- Requires secure `.env` file with production JWT settings
- Environment variables injected into containers

## JWT Authentication Flow

### 1. Token Generation
```csharp
public string GenerateToken(UserDto user)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FullName),
        new(ClaimTypes.GivenName, user.FirstName),
        new(ClaimTypes.Surname, user.LastName)
    };

    // Add roles to claims
    foreach (var role in user.Roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
        Issuer = _jwtSettings.Issuer,
        Audience = _jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey)),
            SecurityAlgorithms.HmacSha256Signature)
    };

    return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
}
```

### 2. Token Validation
- **Issuer Validation**: Validates token issuer matches configuration
- **Audience Validation**: Validates token audience matches configuration  
- **Lifetime Validation**: Ensures token hasn't expired
- **Signature Validation**: Verifies token signature using secret key
- **Clock Skew**: Set to zero for precise timing

## Usage in Different Environments

### Local Development (without Docker)
1. Ensure `.env.development` exists with JWT configuration
2. Application automatically loads environment variables on startup
3. Uses fallback values if environment variables are missing

### Docker Development
```powershell
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# Check JWT configuration in logs
docker-compose -f docker-compose.dev.yml logs api | Select-String "JWT"
```

### Docker Production
```powershell
# Start production environment
docker-compose -f docker-compose.prod.yml up -d

# Verify JWT configuration
docker-compose -f docker-compose.prod.yml logs api
```

### Environment Variable Loading Order
1. **JwtSettings__*** (ASP.NET Core format) - Primary
2. **JWT_*** (Simple format) - Fallback  
3. **Default values** - Final fallback for development

## Security Best Practices

### 1. JWT Secret Key Requirements
- **Minimum 32 characters** for security
- Use cryptographically secure random generation
- Different secret for each environment (dev, staging, prod)
- **Never commit secrets to version control**

### 2. Generate Secure Keys
```powershell
# Using PowerShell (Windows)
[System.Web.Security.Membership]::GeneratePassword(64, 0)

# Using OpenSSL (Linux/macOS)
openssl rand -base64 48

# Using Node.js (Cross-platform)
node -e "console.log(require('crypto').randomBytes(48).toString('base64'))"
```

### 3. Environment-Specific Recommendations
- **Development**: Recognizable key for debugging (`dev-super-secret-key...`)
- **Staging**: Production-like but different key
- **Production**: Strongly generated, unique 48+ character key

### 4. Token Security Features
- **HTTPS Only** in production (`RequireHttpsMetadata = true`)
- **No Clock Skew** for precise expiration (`ClockSkew = TimeSpan.Zero`)
- **Short Expiration** (60 minutes default, configurable)
- **Refresh Token Support** (7 days expiration)

## Configuration Implementation

### JWT Service Integration
The application uses `IOptions<JwtSettings>` pattern for configuration injection:

```csharp
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IOptions<JwtSettings> jwtSettings, ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }
    // ... implementation
}
```

### Authentication Configuration
Located in `AuthenticationExtensions.cs` with environment variable fallback:

```csharp
services.Configure<JwtSettings>(options =>
{
    var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                    Environment.GetEnvironmentVariable("JwtSettings__SecretKey") ??
                    "dev-super-secret-key-with-at-least-32-characters-for-development";

    options.SecretKey = secretKey;
    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                     Environment.GetEnvironmentVariable("JwtSettings__Issuer") ??
                     "HealthySystem";
    // ... other configurations
});
```

### Token Validation Parameters
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidIssuer = _jwtSettings.Issuer,
    ValidateAudience = true,
    ValidAudience = _jwtSettings.Audience,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
```

## Testing JWT Authentication

### 1. Login Endpoint
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "user": {
      "id": 1,
      "email": "user@example.com",
      "fullName": "John Doe",
      "roles": ["User"]
    }
  }
}
```

### 2. Protected Endpoint Testing
```http
GET /api/users/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Refresh Token Endpoint
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64-encoded-refresh-token"
}
```

### 4. JWT Token Debugging
Check application logs for JWT configuration details:
```
Generating JWT token with settings:
  SecretKey length: 64
  SecretKey preview: dev-super-...
  Issuer: HealthySystem
  Audience: HealthySystemUsers
  ExpirationInMinutes: 60
```

## Troubleshooting

### 1. JWT Authentication Issues
**Symptoms**: 401 Unauthorized responses, token validation failures
**Solutions**:
- Verify JWT secret key is at least 32 characters
- Check environment variables are loaded correctly
- Ensure token hasn't expired (check ExpirationInMinutes)
- Validate Issuer and Audience match configuration

### 2. Environment Variable Loading Issues
**Symptoms**: Application uses fallback values instead of configured values
**Solutions**:
- Check `.env.development` file exists and has correct format
- Verify no BOM (Byte Order Mark) in environment files
- Ensure Docker compose `env_file` configuration is correct
- Check for typos in environment variable names (case-sensitive)

### 3. Development vs Production Configuration
**Symptoms**: Different behavior between environments
**Solutions**:
- Development uses `.env.development` file
- Production uses environment-specific `.env` file
- Verify correct Docker compose file is being used
- Check HTTPS requirements differ between environments

### 4. Token Validation Debugging
```csharp
// Add to JwtService for debugging
_logger.LogInformation("Token validation failed. Secret length: {Length}", 
    _jwtSettings.SecretKey?.Length ?? 0);
```

### 5. Common Error Messages
- **"Unable to read environment variable"**: Check file permissions and format
- **"Invalid token signature"**: Secret key mismatch between generation and validation
- **"Token expired"**: Check system clock and ExpirationInMinutes setting
- **"Invalid issuer/audience"**: Configuration mismatch in JWT settings

## Files and Components

### Core Files
- ✅ `Healthy.Infrastructure/Services/JwtService.cs` - JWT token generation and validation
- ✅ `Healthy.Api/Extensions/AuthenticationExtensions.cs` - JWT configuration setup
- ✅ `Healthy.Application/Common/Models/JwtSettings.cs` - JWT configuration model
- ✅ `.env.development` - Development JWT environment variables
- ✅ `.env.example` - Template with JWT configuration examples

### Authentication Flow
1. **Login Request** → `AuthController.Login()`
2. **User Validation** → Database verification
3. **Token Generation** → `JwtService.GenerateToken()`
4. **Token Response** → JWT + Refresh Token returned
5. **Protected Requests** → JWT Bearer token validation
6. **Token Refresh** → `AuthController.RefreshToken()`

## Current Status (Updated August 5, 2025)
- ✅ JWT authentication fully implemented with ASP.NET Core configuration
- ✅ Environment variable support with fallback logic
- ✅ Development and production configurations separated
- ✅ Token generation and validation working
- ✅ Refresh token mechanism implemented
- ✅ Role-based claims integration
- ✅ HTTPS configuration for production security
