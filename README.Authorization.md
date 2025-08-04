# 🔐 Healthy System - Authorization & Authentication

> **Complete Authorization System for Role-based Access Control with JWT Authentication**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Security](https://img.shields.io/badge/security-enabled-green.svg)]()

**Last Updated**: August 5, 2025  
**Version**: v1.0  
**Environment**: Developmentalthy System - Authorization

> **Complete Authorization System for Role-based và Ownership-based Access Control**

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Security](https://img.shields.io/badge/security-enabled-green.svg)]()

## 🎯 Overview

The Healthy System implements a comprehensive authentication and authorization system featuring:

- ✅ **JWT Token-based Authentication** with Bearer tokens
- ✅ **Custom Authorization Attributes** for declarative security
- ✅ **Role-based Access Control (RBAC)** with predefined roles
- ✅ **Ownership-based Authorization** for resource protection
- ✅ **Type-safe Role Constants** for consistency
- ✅ **GUID Comparison Helpers** for secure ID matching
- ✅ **Flexible Authorization Policies** for complex scenarios

## 🔑 Authentication System

### JWT Configuration
The system uses JWT (JSON Web Tokens) for stateless authentication:

```bash
# Current JWT Settings (Development)
Issuer: healthy-system-dev
Audience: healthy-system-api-dev
Secret Key: dev-super-secret-key-with-at-least-32-characters-for-development
Expiration: 24 hours (1440 minutes)
Refresh Token: 7 days
```

### Authentication Flow
1. **Login**: User provides credentials (email/password)
2. **Token Generation**: System validates and creates JWT token
3. **Token Usage**: Client includes token in Authorization header
4. **Token Validation**: System validates token on each request
5. **Authorization**: System checks user roles and permissions

## 🚀 Quick Start

### Basic Authentication Usage
```csharp
// 1. Login to get JWT token
POST /api/auth/login
{
    "email": "admin@healthysystem.com",
    "password": "Admin@123"
}

// Response includes JWT token
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "...",
    "expiresIn": 1440
}

// 2. Use token in subsequent requests
GET /api/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Authorization Attributes Usage
```csharp
using Healthy.Infrastructure.Authorization;
using Healthy.Infrastructure.Constants;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [UserOrAdmin] // User or Admin can access
    public IActionResult GetUsers() => Ok();

    [HttpGet("{userId}")]
    [AuthorizeOwnerOrAdmin("userId")] // Only owner or admin
    public IActionResult GetUser(string userId) => Ok();

    [HttpDelete("{userId}")]
    [AdminOnly] // Admin only
    public IActionResult DeleteUser(string userId) => Ok();
}
```

## 📦 Authorization Components

### 🎭 Custom Authorization Attributes
| Attribute | Purpose | Usage Example | Description |
|-----------|---------|---------------|-------------|
| `[AdminOnly]` | Admin role only | Controller/method level | Restricts access to Admin users only |
| `[UserOrAdmin]` | User OR Admin roles | General endpoints | Allows both User and Admin access |
| `[ModeratorOrAdmin]` | Moderator OR Admin | Content moderation | For content management features |
| `[AuthorizeRoles(...)]` | Custom role combinations | `[AuthorizeRoles("Manager", "Admin")]` | Flexible multiple role authorization |
| `[RequireAllRoles(...)]` | All specified roles required | `[RequireAllRoles("User", "Verified")]` | User must have ALL specified roles |
| `[AuthorizeOwnerOrAdmin("param")]` | Resource ownership check | `[AuthorizeOwnerOrAdmin("userId")]` | Owner of resource or admin can access |

### 🎯 Available Role Constants
```csharp
// Core system roles (currently implemented)
Roles.Admin         // Full system access - can manage all resources
Roles.User          // Basic user access - own resources only  
Roles.Moderator     // Content moderation access

// Extended roles (available for future use)
Roles.Manager       // Business operations management
Roles.SuperUser     // Advanced system operations
Roles.Verified      // Verified user status
Roles.Premium       // Premium feature access
```

### 🔧 Helper Extensions
```csharp
// Safe GUID comparison (case-insensitive, null-safe)
using Healthy.Infrastructure.Extensions;

string userId = "550e8400-e29b-41d4-a716-446655440000";
Guid targetGuid = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

// Safe comparison
bool isMatch = userId.ToGuid() == targetGuid; // true
```

## 📁 Current File Structure

```
Healthy.Infrastructure/
├── Authorization/
│   ├── AuthorizeRolesAttribute.cs          # ✅ Role-based authorization attributes
│   ├── AuthorizeOwnerOrAdminAttribute.cs   # ✅ Ownership-based authorization
│   └── AuthorizationPolicies.cs            # ✅ Policy configurations
├── Constants/
│   └── Roles.cs                           # ✅ Type-safe role constants
├── Extensions/
│   └── GuidExtensions.cs                  # ✅ GUID comparison helpers
└── Services/
    └── JwtService.cs                      # ✅ JWT token generation/validation

Healthy.Api/
├── Controllers/
│   ├── AuthController.cs                  # ✅ Authentication endpoints
│   ├── UsersController.cs                 # ✅ Example with [AdminOnly]
│   └── Base/
│       └── BaseController.cs              # ✅ Base controller with common auth
└── Extensions/
    └── AuthenticationExtensions.cs        # ✅ JWT configuration
```

## 🛡️ Security Model & Role Hierarchy

### Current Role Hierarchy (August 2025)
```
Admin           ──── Full System Access
├── User Management (CRUD)
├── Content Management  
├── System Configuration
└── All User Operations

Moderator       ──── Content Management
├── Article Management
├── Category Management
└── Content Review

User            ──── Basic Access
├── Own Profile Management
├── Health Data Tracking
├── Diary Entries
└── View Public Content
```

### Authentication Flow Details

#### 1. Login Process
```csharp
POST /api/auth/login
{
    "email": "admin@healthysystem.com", 
    "password": "Admin@123"
}

// Success Response (200 OK)
{
    "success": true,
    "message": "Login successful",
    "data": {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "refreshToken": "refresh_token_here",
        "expiresIn": 1440,
        "user": {
            "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
            "email": "admin@healthysystem.com",
            "firstName": "System",
            "lastName": "Administrator",
            "roles": ["Admin"]
        }
    }
}
```

#### 2. Token Validation
- **Bearer Token**: Included in Authorization header
- **Claims Validation**: User ID, roles, issuer, audience
- **Expiration Check**: 24-hour expiration (configurable)
- **Signature Verification**: HMAC SHA-256 with secret key

#### 3. Authorization Checks
- **Role-based**: Check if user has required role(s)
- **Ownership-based**: Verify user owns the resource
## 💻 Current Implementation Examples

### 1. UsersController (Current Live Example)
```csharp
[ApiController]
[Route("api/[controller]")]
[AdminOnly] // Entire controller requires Admin role
public class UsersController(IMediator mediator) : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin")] // Additional ASP.NET Core authorization
    public async Task<ActionResult<UsersListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null)
    {
        // Admin-only user management
    }
}
```

### 2. Mixed Authorization Patterns
```csharp
[ApiController]
[Route("api/[controller]")]
public class HealthDataController : ControllerBase
{
    [HttpGet]
    [UserOrAdmin] // Any authenticated user or admin
    public IActionResult GetPublicHealthTips() => Ok();

    [HttpGet("user/{userId}/records")]
    [AuthorizeOwnerOrAdmin("userId")] // Owner or admin only
    public IActionResult GetUserHealthRecords(string userId) => Ok();

    [HttpPost("reports")]
    [ModeratorOrAdmin] // Content moderation required
    public IActionResult CreateHealthReport() => Ok();
}
```

### 3. Ownership-based Authorization Example
```csharp
[HttpPut("profile/{userId}")]
[AuthorizeOwnerOrAdmin("userId")]
public async Task<IActionResult> UpdateProfile(string userId, UpdateProfileRequest request)
{
    // User can only update their own profile
    // Admin can update any profile
    // Automatic validation based on JWT claims
}
```

## � Security Features & Access Control

### Access Control Matrix
| Resource Type | Admin | Moderator | User | Owner | Anonymous |
|--------------|-------|-----------|------|-------|-----------|
| User Management | ✅ Full CRUD | ❌ | ❌ | ❌ | ❌ |
| Own Profile | ✅ | ❌ | ❌ | ✅ | ❌ |
| Health Data (Own) | ✅ | ❌ | ❌ | ✅ | ❌ |
| Health Data (Any) | ✅ | ❌ | ❌ | ❌ | ❌ |
| Content Moderation | ✅ | ✅ | ❌ | ❌ | ❌ |
| Public Content | ✅ | ✅ | ✅ | ✅ | ✅ |
| System Settings | ✅ | ❌ | ❌ | ❌ | ❌ |

### Current Security Implementation

#### JWT Token Claims
```json
{
  "sub": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",  // User ID
  "email": "admin@healthysystem.com",
  "role": ["Admin"],                               // User roles
  "iss": "healthy-system-dev",                    // Issuer
  "aud": "healthy-system-api-dev",                // Audience  
  "exp": 1723123456,                              // Expiration
  "iat": 1723037056                               // Issued at
}
```

#### Authorization Filter Logic
1. **Authentication Check**: Verify JWT token is valid
2. **Role Extraction**: Get user roles from token claims
3. **Role Validation**: Check if user has required role(s)
4. **Ownership Validation**: Compare user ID with resource owner (if applicable)
5. **Access Decision**: Allow/deny based on rules

## 🧪 Testing Authorization

### Test Users (Available in Development)
```bash
# Admin User
Email: admin@healthysystem.com
Password: Admin@123
Roles: ["Admin"]
Access: Full system access

# Regular User  
Email: user@healthysystem.com
Password: User@123  
Roles: ["User"]
Access: Own resources only
```

### Testing with Postman/Curl
```bash
# 1. Login to get token
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@healthysystem.com","password":"Admin@123"}'

# 2. Extract token from response
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# 3. Use token in protected endpoints  
curl -X GET http://localhost:5001/api/users \
  -H "Authorization: Bearer $TOKEN"
```

### Authorization Test Scenarios
| Test Case | Expected Result | Status Code |
|-----------|----------------|-------------|
| Access public endpoint without token | ✅ Success | 200 |
| Access protected endpoint without token | ❌ Unauthorized | 401 |
| Access admin endpoint with user token | ❌ Forbidden | 403 |
| Access admin endpoint with admin token | ✅ Success | 200 |
| Access own resource with valid ownership | ✅ Success | 200 |
| Access other user's resource | ❌ Forbidden | 403 |

## 🔧 Configuration & Setup

### JWT Configuration (Environment Variables)
```bash
# Required JWT Settings
JwtSettings__SecretKey=dev-super-secret-key-with-at-least-32-characters-for-development
JwtSettings__Issuer=healthy-system-dev
JwtSettings__Audience=healthy-system-api-dev
JwtSettings__ExpirationInMinutes=1440
JwtSettings__RefreshTokenExpirationInDays=7
```

### Startup Configuration
```csharp
// Program.cs - Already configured in Healthy System
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // JWT configuration
    });

builder.Services.AddAuthorization();

// Custom authorization services are auto-registered
```

## 🛠️ Troubleshooting

### Common Issues & Solutions

#### 1. **401 Unauthorized**
- **Cause**: Missing or invalid JWT token
- **Solution**: Ensure valid token in Authorization header
```bash
Authorization: Bearer your-jwt-token-here
```

#### 2. **403 Forbidden** 
- **Cause**: User doesn't have required role
- **Solution**: Check user roles in JWT claims
```bash
# Decode JWT token to verify roles
# Use jwt.io or similar tool
```

#### 3. **Token Expired**
- **Cause**: JWT token past expiration time
- **Solution**: Login again or use refresh token
```bash
# Check token expiration
# exp claim in JWT payload
```

#### 4. **Ownership Check Fails**
- **Cause**: User ID parameter name mismatch
- **Solution**: Ensure parameter name matches attribute
```csharp
// Parameter name must match
[AuthorizeOwnerOrAdmin("userId")]
public IActionResult GetUser(string userId) // ✅ Correct

[AuthorizeOwnerOrAdmin("userId")]  
public IActionResult GetUser(string id) // ❌ Wrong parameter name
```

### Debug Authorization Issues
```csharp
// Add logging to see authorization details
public void OnAuthorization(AuthorizationFilterContext context)
{
    var logger = context.HttpContext.RequestServices
        .GetRequiredService<ILogger<AuthorizeRolesAttribute>>();
    
    logger.LogInformation("Authorization check for {User} with roles {Roles}", 
        context.HttpContext.User.Identity.Name,
        string.Join(",", context.HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)));
}
```

## 📚 Additional Resources

### Related Documentation
- **[Database Schema](README.Database.md)** - User and Role entity details
- **[Environment Setup](README.Environment.md)** - JWT configuration
- **[API Documentation](README.md)** - Complete API reference

### Security Best Practices
1. **🔑 Strong Secrets**: Use complex JWT secret keys (32+ characters)
2. **⏰ Token Expiration**: Set appropriate expiration times
3. **🔄 Token Refresh**: Implement refresh token mechanism
4. **🛡️ HTTPS Only**: Use HTTPS in production
5. **📝 Audit Logging**: Log authorization decisions
6. **🔍 Input Validation**: Validate all user inputs
7. **🚫 Principle of Least Privilege**: Grant minimum required permissions

## 📝 Changelog

### August 5, 2025
- ✅ **Comprehensive documentation update**: Reflected current JWT implementation
- ✅ **Added live examples**: Included actual controller implementations
- ✅ **Enhanced security model**: Updated role hierarchy and access control matrix
- ✅ **Added testing guide**: Step-by-step testing instructions
- ✅ **Configuration details**: Current JWT settings and environment variables
- ✅ **Troubleshooting section**: Common issues and solutions
- ✅ **Best practices**: Security recommendations and implementation guidelines

### Current Implementation Status
- **JWT Authentication**: ✅ Fully implemented and tested
- **Custom Authorization Attributes**: ✅ Active and working
- **Role-based Access Control**: ✅ 3 roles implemented (Admin, User, Moderator)
- **Ownership-based Authorization**: ✅ Working with GUID comparison
- **Environment**: Development with 24-hour token expiration
- **Test Users**: Admin and User accounts available for testing

---

**🔐 Your Healthy System API is secured with robust authentication and authorization!**

*For technical support or questions, refer to the troubleshooting section above or check the related documentation.*
