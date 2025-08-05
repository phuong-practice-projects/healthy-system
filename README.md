# 📑 Healthy System - Documentation Hub

> **Complete documentation ecosystem for the Healthy System API - Your one-stop guide to build, deploy, and maintain a secure health tracking platform**

## � Quick Start

**New to the project?** Start here:
1. **[🛠️ Development Setup](README.SETUP.md)** - Complete setup guide from zero to running
2. **[🏗️ Architecture Overview](README.Architecture.md)** - Understand the Clean Architecture implementation
3. **[🔐 Authorization System](README.Authorization.md)** - Secure your endpoints with role-based access

---

## 📚 Core Documentation

### 🔐 Security & Authentication
- **[🔑 Authorization System](README.Authorization.md)** - Complete role-based access control guide
  - JWT authentication setup
  - Custom authorization attributes
  - Ownership-based permissions
  - Role constants and security patterns
- **[🛡️ Authentication Guide](README.Authentication.md)** - JWT configuration and token management
  - Token generation and validation
  - Environment variable configuration
  - Security best practices

### 🏗️ Architecture & Design  
- **[⚡ Clean Architecture](README.Architecture.md)** - CQRS implementation with MediatR
  - Layer separation principles
  - Domain-driven design patterns
  - Command/Query responsibility segregation
  - Dependency injection setup

### 🗄️ Database & Migrations
- **[🗃️ Database Design](README.Database.md)** - Entity relationships and schema
  - Entity relationship diagrams
  - Table structures and constraints
  - Seeding data and initial setup
- **[🔄 Migration Management](README.Migrations.md)** - Database version control
  - Migration creation and execution
  - Environment-specific migrations
  - Rollback strategies

### 🚀 Environment & Deployment
- **[⚙️ Environment Configuration](README.Environment.md)** - Multi-environment setup
  - Development, staging, production configs
  - Environment variable management
  - Configuration validation
- **[🐳 Docker Deployment](README.Docker.md)** - Containerization guide
  - Docker Compose setup
  - Production deployment strategies
  - Container orchestration

---

## � Quick Reference & Code Examples

### 🔐 Authorization Quick Start
**Secure your endpoints in seconds:**

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
| Attribute | Description | Usage |
|-----------|-------------|--------|
| `[AdminOnly]` | Admin role only | Restrict to administrators |
| `[UserOrAdmin]` | User OR Admin roles | Basic authenticated access |
| `[AuthorizeRoles(...)]` | Custom roles | Multiple role validation |
| `[AuthorizeOwnerOrAdmin("param")]` | Ownership-based | Resource ownership check |
| `[DevOnly]` | Development only | Development environment restriction |

### 🎭 Role Constants
```csharp
// Available in Healthy.Infrastructure.Constants.Roles
Roles.Admin        // System administrator
Roles.User         // Regular user
Roles.Moderator    // Content moderator  
Roles.Manager      // Manager role
Roles.SuperUser    // Super user privileges
Roles.Verified     // Verified user
Roles.Premium      // Premium subscription
```

### 🗄️ Database Quick Commands
```bash
# Apply migrations
dotnet ef database update --project Healthy.Infrastructure

# Create new migration
dotnet ef migrations add YourMigrationName --project Healthy.Infrastructure

# Reset database (development only)
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d
```

---

## 🏗️ Project Architecture

```
healthy-system/
├── 📁 Healthy/                           # Main .NET solution
│   ├── 🌐 Healthy.Api/                   # Web API & Controllers
│   │   ├── Controllers/                  # API endpoints
│   │   ├── Models/                       # Request/Response models
│   │   └── Extensions/                   # Service registrations
│   ├── 🧠 Healthy.Application/           # Business logic & CQRS
│   │   ├── UseCases/                     # Commands & Queries
│   │   ├── Common/Behaviors/             # Cross-cutting concerns
│   │   └── DependencyInjection.cs       # Service registration
│   ├── 🏛️ Healthy.Domain/                # Core business entities
│   │   ├── Entities/                     # Domain models
│   │   ├── Exceptions/                   # Domain exceptions
│   │   └── Common/                       # Base classes
│   ├── 🔧 Healthy.Infrastructure/        # External concerns
│   │   ├── Persistence/                  # Database context
│   │   ├── Authorization/                # Security system
│   │   ├── Services/                     # External services
│   │   └── Migrations/                   # EF migrations
│   └── 🧪 Healthy.Tests.Unit/            # Unit tests
├── 📁 config/                            # Configuration files
├── 📁 nginx/                             # Reverse proxy config
├── 📁 performance-tests/                 # Load testing
└── 📁 scripts/                           # Automation scripts
```

## 🔧 Development Workflow

### 🛠️ Initial Setup (First Time)
1. **Clone**: `git clone <repository-url>`
2. **Navigate**: `cd healthy-system`
3. **Environment**: Copy `.env.example` → `.env.development`
4. **Database**: `docker-compose -f docker-compose.dev.yml up -d`
5. **Migrate**: `dotnet ef database update --project Healthy.Infrastructure`
6. **Run**: `cd Healthy && dotnet run --project Healthy.Api`

### � Daily Development
1. **Pull latest**: `git pull origin main`
2. **Start services**: `docker-compose -f docker-compose.dev.yml up -d`
3. **Apply migrations**: `dotnet ef database update --project Healthy.Infrastructure`
4. **Start API**: `dotnet run --project Healthy.Api`
5. **Test**: Browse to `https://localhost:7071/swagger`

### 🚀 Production Deployment
1. **Build**: `docker-compose -f docker-compose.prod.yml build`
2. **Deploy**: `docker-compose -f docker-compose.prod.yml up -d`
3. **Verify**: Check health endpoints and logs

---

## 🎯 Key Features & Capabilities

### � Security Features
- ✅ **JWT Authentication** with Bearer tokens
- ✅ **Role-based Authorization** (RBAC) 
- ✅ **Ownership-based Access Control**
- ✅ **Environment-specific Security**
- ✅ **Secure Password Hashing** (BCrypt)
- ✅ **HTTPS/TLS Encryption**

### 📊 API Capabilities  
- 👥 **User Management** with role assignment
- � **Health Tracking** (body records, exercises, meals)
- 📰 **Content Management** (articles, columns)
- 📈 **Analytics Dashboard** with health summaries
- 🔍 **Advanced Filtering** and pagination
- � **RESTful API Design** with OpenAPI/Swagger

### 🏗️ Technical Stack
- ⚡ **Clean Architecture** with CQRS pattern
- 🎯 **MediatR** for command/query handling  
- 🗄️ **Entity Framework Core** with SQL Server
- 🐳 **Docker** containerization
- 🔄 **Database Migrations** with versioning
- 🧪 **Unit Testing** with xUnit

---

## 📖 Detailed Documentation Links

| Topic | File | Description |
|-------|------|-------------|
| **Complete Setup** | [README.SETUP.md](README.SETUP.md) | Step-by-step development environment setup |
| **Architecture** | [README.Architecture.md](README.Architecture.md) | Clean Architecture & CQRS implementation |
| **Authorization** | [README.Authorization.md](README.Authorization.md) | Security system with examples |
| **Authentication** | [README.Authentication.md](README.Authentication.md) | JWT configuration guide |
| **Database** | [README.Database.md](README.Database.md) | Entity relationships & design |
| **Migrations** | [README.Migrations.md](README.Migrations.md) | Database versioning & updates |
| **Environment** | [README.Environment.md](README.Environment.md) | Configuration management |
| **Docker** | [README.Docker.md](README.Docker.md) | Containerization & deployment |

---

## 🆘 Need Help?

### 🐛 Common Issues
- **Database connection failed**: Check Docker container status
- **Migration errors**: Ensure database is running and accessible
- **Authentication issues**: Verify JWT configuration in environment variables
- **Permission denied**: Check user roles and authorization attributes

### 📞 Support Channels
- **Documentation**: Start with relevant README files above
- **Issues**: Create GitHub issue with detailed description
- **Architecture Questions**: Refer to [README.Architecture.md](README.Architecture.md)
- **Security Concerns**: See [README.Authorization.md](README.Authorization.md)

---

**🚀 Ready to build a secure and scalable health tracking system!**

> **👉 Next Steps**: 
> - New developers: Start with [README.SETUP.md](README.SETUP.md)
> - Security focus: Begin with [README.Authorization.md](README.Authorization.md)  
> - Architecture deep-dive: Explore [README.Architecture.md](README.Architecture.md)
