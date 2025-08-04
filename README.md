# 📑 Healthy System - Documentation Index

> **Complete documentation for the Healthy System API**

## 🔐 Authorization & Security

- **[Authorization System](README.Authorization.md)** - Complete authorization guide
- **[Authorization Quick Reference](Authorization-QuickRef.md)** - Quick reference card
- **[Authorization Guide](docs/Authorization-Guide.md)** - Detailed usage examples
- **[Role Constants Guide](docs/Role-Constants-Guide.md)** - Role constants documentation
- **[JWT Configuration](JWT-Configuration.md)** - JWT setup and configuration
- **[Security Guidelines](SECURITY.md)** - Security best practices

## 🗄️ Database & Data

- **[Database Relationships](README.Database-Relationships.md)** - Entity relationships
- **[Migrations Guide](README.Migrations.md)** - Database migrations
- **[Current User System](README.CurrentUser.md)** - User context management

## 🚀 Development & Deployment

- **[Environment Setup](README.Environment.md)** - Environment configuration
- **[Docker Setup](README.Docker.md)** - Docker deployment
- **[CI/CD Pipeline](README.CI-CD.md)** - Continuous integration

## 📋 Quick Links

### 🔐 Authorization Quick Start
```csharp
using Healthy.Infrastructure.Authorization;
using Healthy.Infrastructure.Constants;

[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    [HttpGet("admin")]
    [AdminOnly]
    public IActionResult AdminEndpoint() => Ok();

    [HttpGet("user/{userId}/data")]
    [AuthorizeOwnerOrAdmin("userId")]
    public IActionResult GetUserData(string userId) => Ok();
}
```

### 🎯 Available Authorization Attributes
- `[AdminOnly]` - Admin role only
- `[UserOrAdmin]` - User OR Admin roles
- `[AuthorizeRoles(Roles.Manager, Roles.Admin)]` - Custom roles
- `[AuthorizeOwnerOrAdmin("userId")]` - Ownership-based
- `[DevOnly]` - Development environment only

### 🎭 Role Constants
```csharp
Roles.Admin, Roles.User, Roles.Moderator, 
Roles.Manager, Roles.SuperUser, Roles.Verified, Roles.Premium
```

## 🏗️ Project Structure

```
healthy-system/
├── Healthy/                           # Main solution
│   ├── Healthy.Api/                   # Web API layer
│   ├── Healthy.Application/           # Application logic
│   ├── Healthy.Domain/                # Domain entities
│   ├── Healthy.Infrastructure/        # Infrastructure & Authorization
│   └── Healthy.Tests.Unit/            # Unit tests
├── docs/                              # Detailed documentation
├── config/                            # Configuration files
├── nginx/                             # Nginx configuration
└── performance-tests/                 # Load testing
```

## 🔧 Development Setup

1. **Prerequisites**: .NET 9.0, Docker, SQL Server
2. **Clone repository**: `git clone <repo-url>`
3. **Setup environment**: Copy `.env.example` to `.env.development`
4. **Start database**: `docker-compose -f docker-compose.dev.yml up -d`
5. **Run API**: `cd Healthy && dotnet run`

## 📚 Architecture

- **Clean Architecture** with Domain, Application, Infrastructure layers
- **CQRS with MediatR** for command/query separation
- **JWT Authentication** with role-based authorization
- **Entity Framework Core** with SQL Server
- **Docker containerization** for development and production

## 🛡️ Security Features

- ✅ JWT-based authentication
- ✅ Role-based access control (RBAC)
- ✅ Ownership-based authorization
- ✅ Environment-based restrictions
- ✅ Type-safe authorization attributes
- ✅ Secure password hashing (BCrypt)

## 📊 API Features

- 👥 User management with roles
- 📝 Health tracking (body records, exercises, meals)
- 📰 Content management (columns, articles)
- 📈 Dashboard with health summaries
- 🔍 Filtering and pagination
- 📊 Health analytics

---

**🚀 Ready to build secure and scalable health tracking API!**

> **Next Steps**: Start with [Authorization System](README.Authorization.md) to secure your endpoints.
