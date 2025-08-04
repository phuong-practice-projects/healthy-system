# 🔐 Healthy System - Authorization

> **Complete Authorization System for Role-based và Ownership-based Access Control**

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Security](https://img.shields.io/badge/security-enabled-green.svg)]()

## 🎯 Overview

Authorization system cung cấp **role-based** và **ownership-based** access control cho Healthy System API với:

- ✅ **Custom Authorization Attributes**
- ✅ **Type-safe Role Constants** 
- ✅ **GUID Comparison Helpers**
- ✅ **Environment-based Restrictions**
- ✅ **Controller & Method Level Support**

## 🚀 Quick Start

```csharp
using Healthy.Infrastructure.Authorization;
using Healthy.Infrastructure.Constants;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [UserOrAdmin] // User hoặc Admin có thể truy cập
    public IActionResult GetUsers() => Ok();

    [HttpGet("{userId}")]
    [AuthorizeOwnerOrAdmin("userId")] // Chỉ owner hoặc admin
    public IActionResult GetUser(string userId) => Ok();

    [HttpDelete("{userId}")]
    [AdminOnly] // Chỉ admin
    public IActionResult DeleteUser(string userId) => Ok();
}
```

## 📦 Components

### 🎭 Authorization Attributes
| Attribute | Purpose | Example |
|-----------|---------|---------|
| `[AdminOnly]` | Admin role only | Admin-exclusive endpoints |
| `[UserOrAdmin]` | User OR Admin | General protected endpoints |
| `[ModeratorOrAdmin]` | Moderator OR Admin | Content moderation |
| `[AuthorizeRoles(...)]` | Custom roles (ANY) | Flexible role combinations |
| `[RequireAllRoles(...)]` | Multiple roles (ALL) | High-security endpoints |
| `[AuthorizeOwnerOrAdmin("param")]` | Resource ownership | User-specific data |
| `[DevOnly]` | Development only | Debug/testing endpoints |

### 🎯 Role Constants
```csharp
Roles.Admin         // Full system access
Roles.User          // Basic user access
Roles.Moderator     // Content moderation
Roles.Manager       // Business operations
Roles.SuperUser     // System operations
Roles.Verified      // Verified users
Roles.Premium       // Premium features
```

### 🔧 Helper Extensions
```csharp
// Safe GUID comparison (case-insensitive)
userId.ToGuid() == targetGuid
```

## 📁 File Structure

```
├── Healthy.Infrastructure/
│   ├── Authorization/
│   │   ├── README.md                           # This file
│   │   ├── AuthorizeRolesAttribute.cs          # Role-based attributes
│   │   ├── AuthorizeOwnerOrAdminAttribute.cs   # Ownership-based attributes
│   │   └── AuthorizationPolicies.cs            # Policy configurations
│   ├── Constants/
│   │   └── Roles.cs                           # Role constants
│   └── Extensions/
│       └── GuidExtensions.cs                  # GUID helpers
├── docs/
│   ├── Authorization-Guide.md                  # Detailed usage guide
│   └── Role-Constants-Guide.md                # Role constants documentation
└── Examples/
    └── ExampleAuthController.cs               # Usage examples
```

## 🛡️ Security Model

### Role Hierarchy
```
SuperUser   ──┐
Admin       ──┼── Admin Level (Full Access)
              │
Manager     ──┐
Moderator   ──┼── Moderation Level (Content Management)
              │
Premium     ──┐
Verified    ──┼── User Level (Basic + Features)
User        ──┘
```

### Access Control Matrix
| Resource Type | Admin | Moderator | User | Owner |
|--------------|-------|-----------|------|-------|
| Any User Data | ✅ | ❌ | ❌ | ✅ |
| Own User Data | ✅ | ❌ | ✅ | ✅ |
| Content Moderation | ✅ | ✅ | ❌ | ❌ |
| System Settings | ✅ | ❌ | ❌ | ❌ |
| Public Data | ✅ | ✅ | ✅ | ✅ |

## 💡 Usage Patterns

### 1. **Controller-level Protection**
```csharp
[UserOrAdmin] // Áp dụng cho tất cả methods
public class ProtectedController : ControllerBase { }
```

### 2. **Method-level Protection**
```csharp
[HttpDelete("{id}")]
[AdminOnly] // Chỉ method này
public IActionResult Delete(int id) { }
```

### 3. **Ownership-based Access**
```csharp
[HttpGet("user/{userId}/data")]
[AuthorizeOwnerOrAdmin("userId")]
public IActionResult GetUserData(string userId) { }
```

### 4. **Multiple Role Requirements**
```csharp
// ANY role (OR logic)
[AuthorizeRoles(Roles.Manager, Roles.Admin)]

// ALL roles (AND logic)  
[RequireAllRoles(Roles.User, Roles.Verified)]
```

### 5. **Environment Restrictions**
```csharp
[HttpPost("seed-data")]
[AdminOnly]
[DevOnly] // Chỉ trong Development
public IActionResult SeedData() { }
```

## 🔒 Implementation Features

- **🔐 JWT Integration**: Works with JWT authentication
- **🎭 Flexible Roles**: Support custom role combinations  
- **👤 Ownership Check**: User can only access own resources
- **🌍 Environment Control**: Restrict by environment
- **📝 Type Safety**: IntelliSense support with constants
- **🚫 Error Handling**: Proper HTTP status codes (401, 403)
- **🔍 Case Insensitive**: Safe GUID comparisons

## 📚 Documentation

| Document | Description |
|----------|-------------|
| **[Authorization Guide](../../docs/Authorization-Guide.md)** | Complete usage guide with examples |
| **[Role Constants Guide](../../docs/Role-Constants-Guide.md)** | Role constants documentation |
| **[API README](README.md)** | Quick reference (this file) |

## 🚀 Getting Started

### 1. **Setup**
```csharp
// Import namespaces
using Healthy.Infrastructure.Authorization;
using Healthy.Infrastructure.Constants;
```

### 2. **Apply Authorization**
```csharp
[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public() => Ok(); // No auth

    [HttpGet("protected")]
    [UserOrAdmin]
    public IActionResult Protected() => Ok(); // Auth required
}
```

### 3. **Test with JWT**
```bash
# Include JWT token with role claims
Authorization: Bearer <jwt-token-with-roles>
```

## ⚠️ Requirements

- **Authentication**: JWT token required
- **Role Claims**: Token must contain `ClaimTypes.Role` 
- **User ID Claim**: Token must contain `ClaimTypes.NameIdentifier` for ownership checks

## 🔧 Configuration

No additional configuration required! The system works out-of-the-box with:
- JWT Authentication middleware
- Standard ASP.NET Core authorization

## 🎯 Best Practices

1. **Use Role Constants**: `Roles.Admin` instead of `"Admin"`
2. **Layer Security**: Apply both controller and method level auth
3. **Ownership Checks**: Use `[AuthorizeOwnerOrAdmin]` for user resources
4. **Test Thoroughly**: Verify all authorization scenarios
5. **Document Access**: Comment complex authorization logic

---

**🛡️ Your API is now secure and ready for production!**

> **Next Steps**: Check out the [Authorization Guide](../../docs/Authorization-Guide.md) for detailed examples and advanced usage.
