# Clean Architecture & CQRS Implementation

This document describes the Clean Architecture implementation for the Healthy System project using CQRS pattern with MediatR.

## Architecture Overview

The project follows Clean Architecture principles with the following layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                      │
│                    (Healthy.Api)                          │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                       │
│                 (Healthy.Application)                     │
├─────────────────────────────────────────────────────────────┤
│                   Domain Layer                            │
│                   (Healthy.Domain)                        │
├─────────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                       │
│                (Healthy.Infrastructure)                   │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

### 1. Domain Layer (`Healthy.Domain`)
**Purpose**: Contains business logic, entities, value objects, domain events, and domain services.

**Key Components**:
- **Entities**: Business objects with identity (User, Role, UserRole, BodyRecord, Exercise, Meal, Diary, Column, Category)
- **Value Objects**: Immutable objects representing concepts (Email, PhoneNumber)
- **Domain Events**: Events that occur within the domain
- **Domain Services**: Business logic that doesn't belong to a specific entity
- **Interfaces**: Contracts for domain services
- **Exceptions**: Domain-specific exceptions (NotFoundException, BadRequestException, etc.)

**Dependencies**: None (pure business logic)

### 2. Application Layer (`Healthy.Application`)
**Purpose**: Contains use cases, application services, and orchestration logic.

**Key Components**:
- **Commands**: Write operations (CreateUserCommand, UpdateUserCommand, etc.)
- **Queries**: Read operations (GetUsersQuery, GetUserByIdQuery, etc.)
- **Handlers**: Command and Query handlers implementing business logic
- **DTOs**: Data Transfer Objects for API responses
- **Validators**: FluentValidation rules for commands
- **Interfaces**: Contracts for external services (ICurrentUserService, IDateTime, IJwtService)
- **Behaviors**: MediatR pipeline behaviors (Validation, Logging)
- **Models**: Common models (CurrentUserInfo, AuthResult, etc.)
- **Helpers**: Utility classes (PaginationHelper, ComparisonHelper)

**Dependencies**: Domain Layer

### 3. Infrastructure Layer (`Healthy.Infrastructure`)
**Purpose**: Contains external concerns like database, external APIs, file system, authentication, and authorization.

**Key Components**:
- **Persistence**: Entity Framework DbContext and configurations
- **Services**: Implementation of application interfaces (CurrentUserService, DateTimeService, JwtService)
- **Authorization**: Role-based access control and custom authorization attributes
- **Middleware**: Global exception handling and user authorization
- **Constants**: Role definitions and system constants
- **Extensions**: Database, error response, and middleware extensions

**Dependencies**: Application Layer, Domain Layer

### 4. Presentation Layer (`Healthy.Api`)
**Purpose**: Contains controllers, API endpoints, and presentation logic.

**Key Components**:
- **Controllers**: API endpoints with Swagger documentation
- **Middleware**: Request/response processing
- **Configuration**: Dependency injection setup
- **Extensions**: Authentication, CORS, Swagger, and environment extensions
- **Swagger**: API documentation

**Dependencies**: Application Layer

## Implemented Modules

The system currently implements the following modules:

### 1. Users Module
- **Authentication**: JWT-based authentication with login/register
- **User Management**: CRUD operations for users
- **Role Management**: Role-based access control
- **Current User**: Context-aware user information

### 2. Body Records Module
- **Health Tracking**: Weight, body measurements tracking
- **Graph Data**: Historical data visualization
- **CRUD Operations**: Create, read, update, delete body records

### 3. Exercises Module
- **Exercise Management**: CRUD operations for exercises
- **Workout Tracking**: Exercise logging and history
- **Filtering**: Search and filter exercises

### 4. Meals Module
- **Meal Management**: CRUD operations for meals
- **Nutrition Tracking**: Meal history and nutrition data
- **Validation**: Comprehensive meal validation

### 5. Diaries Module
- **Diary Management**: CRUD operations for diaries
- **Content Management**: Rich text diary entries
- **User Association**: User-specific diary entries

### 6. Columns Module
- **Content Management**: CRUD operations for columns/articles
- **Publishing**: Column publishing and management
- **Authorization**: Role-based column access

### 7. Dashboard Module
- **Health Summary**: Overview of health metrics
- **Achievements**: User achievement tracking
- **Analytics**: Health data analytics

### 8. Categories Module
- **Category Management**: CRUD operations for categories
- **Content Organization**: Categorization system

## Authentication & Authorization

### JWT Authentication
The system implements JWT-based authentication with the following features:

```csharp
// JWT Configuration
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
}
```

### Role-Based Authorization
The system implements comprehensive role-based access control:

```csharp
// Available Roles
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Moderator = "Moderator";
    public const string Manager = "Manager";
    public const string SuperUser = "SuperUser";
    public const string Verified = "Verified";
    public const string Premium = "Premium";
}
```

### Authorization Attributes
Custom authorization attributes for easy role-based access:

```csharp
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

## CQRS Pattern Implementation

### Commands (Write Operations)
Commands represent write operations and follow these conventions:

```csharp
public record CreateUserCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    // ... other properties
}
```

### Queries (Read Operations)
Queries represent read operations and return DTOs:

```csharp
public record GetUsersQuery : IRequest<PaginatedList<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    // ... other properties
}
```

### Handlers
Handlers implement the business logic for commands and queries:

```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
    }
}
```

## MediatR Pipeline

The application uses MediatR for implementing the CQRS pattern with the following pipeline behaviors:

### 1. Validation Behavior
Automatically validates commands using FluentValidation:

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Validates requests before processing
}
```

### 2. Logging Behavior
Logs all requests and responses:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Logs request/response information
}
```

## Middleware Implementation

### Global Exception Handling
Centralized exception handling for consistent error responses:

```csharp
public class GlobalExceptionMiddleware
{
    // Handles all unhandled exceptions
    // Returns consistent error responses
}
```

### User Authorization Middleware
Custom middleware for user context and authorization:

```csharp
public class UserAuthorizationMiddleware
{
    // Sets up current user context
    // Handles authorization logic
}
```

## Dependency Injection

### Application Layer Registration
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Register MediatR
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
    
    // Register AutoMapper
    services.AddAutoMapper(assembly);
    
    // Register FluentValidation
    services.AddValidatorsFromAssembly(assembly);
    
    // Register Behaviors
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    
    return services;
}
```

### Infrastructure Layer Registration
```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Register DbContext
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    
    // Register Services
    services.AddScoped<IDateTime, DateTimeService>();
    services.AddScoped<ICurrentUserService, CurrentUserService>();
    services.AddScoped<IJwtService, JwtService>();
    
    // Register Authorization
    services.AddAuthorization(options => AuthorizationPolicies.ConfigurePolicies(options));
    
    return services;
}
```

## Database Design

### Entity Configuration
Entities are configured using Entity Framework Fluent API:

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        // ... other configurations
    }
}
```

### Entity Relationships
The system implements the following entity relationships:

- **User ↔ UserRole ↔ Role**: Many-to-many relationship for role management
- **User → BodyRecord**: One-to-many for health tracking
- **User → Exercise**: One-to-many for exercise tracking
- **User → Meal**: One-to-many for meal tracking
- **User → Diary**: One-to-many for diary entries
- **User → Column**: One-to-many for content management
- **Category → Column**: One-to-many for content categorization

## Validation

### FluentValidation Rules
Commands are validated using FluentValidation:

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");
        // ... other rules
    }
}
```

## Error Handling

### Result Pattern
All operations return a `Result<T>` object:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Domain Exceptions
Custom domain exceptions for specific error scenarios:

```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) 
        : base($"{name} ({key}) was not found") { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
```

## Testing Strategy

### Unit Tests (`Healthy.Tests.Unit`)
- **Domain Logic**: Test business rules and domain events
- **Application Logic**: Test command/query handlers
- **Validation**: Test FluentValidation rules
- **Authorization**: Test authorization policies and attributes

### Integration Tests
- **Database Operations**: Test Entity Framework operations
- **API Endpoints**: Test complete request/response cycles
- **Authentication**: Test JWT authentication flow
- **Authorization**: Test role-based access control

### Test Structure
```
Healthy.Tests.Unit/
├── Api/
│   └── Controllers/
├── Application/
│   ├── BodyRecords/
│   ├── Columns/
│   ├── Diaries/
│   ├── Exercises/
│   ├── Meals/
│   └── Users/
├── Domain/
│   └── Entities/
├── Integration/
└── Common/
    └── TestHelper.cs
```

## API Documentation

### Swagger Implementation
The API includes comprehensive Swagger documentation with:

- **Operation Descriptions**: Detailed descriptions for each endpoint
- **Request/Response Examples**: Sample data for testing
- **Response Codes**: All possible HTTP status codes
- **Authentication**: JWT token support
- **Validation**: Automatic validation error documentation
- **Authorization**: Role-based access documentation

### API Endpoints
Current implemented endpoints:

- **Authentication**: `/api/auth/login`, `/api/auth/register`
- **Users**: `/api/users` (CRUD operations)
- **Body Records**: `/api/bodyrecords` (health tracking)
- **Exercises**: `/api/exercises` (workout tracking)
- **Meals**: `/api/meals` (nutrition tracking)
- **Diaries**: `/api/diaries` (personal diary)
- **Columns**: `/api/columns` (content management)
- **Dashboard**: `/api/dashboard` (health summary)

## Best Practices

### 1. Separation of Concerns
- Each layer has a specific responsibility
- Dependencies flow inward (Domain has no dependencies)
- Business logic is isolated in the Domain layer

### 2. CQRS Benefits
- **Commands**: Optimized for write operations
- **Queries**: Optimized for read operations
- **Scalability**: Can scale read/write operations independently
- **Performance**: Can use different data models for reads/writes

### 3. MediatR Benefits
- **Decoupling**: Controllers don't depend on business logic
- **Testability**: Easy to mock and test handlers
- **Pipeline**: Centralized cross-cutting concerns
- **Flexibility**: Easy to add new behaviors

### 4. Security Best Practices
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Fine-grained access control
- **Input Validation**: Comprehensive validation at multiple layers
- **Error Handling**: Secure error responses without information leakage

### 5. Validation Strategy
- **Input Validation**: FluentValidation for commands
- **Business Rules**: Domain entities enforce business rules
- **Error Handling**: Consistent error responses

## Module Organization

Each module follows this structure:

```
ModuleName/
├── Commands/
│   ├── CreateModule/
│   │   ├── CreateModuleCommand.cs
│   │   ├── CreateModuleCommandHandler.cs
│   │   └── CreateModuleCommandValidator.cs
│   ├── UpdateModule/
│   └── DeleteModule/
├── Queries/
│   ├── GetModules/
│   │   ├── GetModulesQuery.cs
│   │   └── GetModulesQueryHandler.cs
│   └── GetModule/
└── DTOs/
    └── ModuleDto.cs
```

## Environment Configuration

### Development Environment
- **Database**: SQL Server with seed data
- **Authentication**: JWT with development keys
- **CORS**: Permissive CORS for development
- **Logging**: Detailed logging for debugging

### Production Environment
- **Database**: SQL Server with production configuration
- **Authentication**: JWT with secure keys
- **CORS**: Restricted CORS for security
- **HTTPS**: Enforced HTTPS redirection

## Performance Considerations

### 1. Database Optimization
- **Indexing**: Proper database indexing for queries
- **Pagination**: Efficient pagination for large datasets
- **Query Optimization**: Optimized Entity Framework queries

### 2. Caching Strategy
- **Response Caching**: HTTP response caching
- **Memory Caching**: In-memory caching for frequently accessed data
- **Distributed Caching**: Redis for distributed scenarios

### 3. API Performance
- **Async/Await**: Non-blocking operations throughout
- **Compression**: Response compression for large payloads
- **Rate Limiting**: API rate limiting for abuse prevention

## Future Enhancements

1. **Event Sourcing**: Implement event sourcing for audit trails
2. **Caching**: Add Redis caching for frequently accessed data
3. **Background Jobs**: Implement Hangfire for background processing
4. **Monitoring**: Add Application Insights for monitoring
5. **Rate Limiting**: Implement API rate limiting
6. **API Versioning**: Add API versioning support
7. **Real-time Features**: Add SignalR for real-time updates
8. **File Upload**: Implement file upload for images and documents
9. **Notifications**: Add email and push notification system
10. **Analytics**: Advanced health analytics and reporting

## Getting Started

1. **Build the Solution**:
   ```bash
   dotnet build
   ```

2. **Setup Environment**:
   ```bash
   # Copy environment file
   cp .env.example .env.development
   
   # Update environment variables
   JWT_SECRET=your-secret-key
   JWT_ISSUER=HealthySystem
   JWT_AUDIENCE=HealthySystemUsers
   ```

3. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**:
   Navigate to `https://localhost:5001` for the Swagger documentation.

## Contributing

When adding new features:

1. **Domain Layer**: Add entities and domain logic
2. **Application Layer**: Add commands/queries and handlers
3. **Infrastructure Layer**: Add persistence and external services
4. **Presentation Layer**: Add controllers and API endpoints
5. **Tests**: Add unit and integration tests
6. **Documentation**: Update API documentation

Follow the existing patterns and conventions for consistency.

## Security Checklist

- [x] JWT Authentication implemented
- [x] Role-based authorization
- [x] Input validation with FluentValidation
- [x] Secure password hashing
- [x] HTTPS enforcement in production
- [x] CORS configuration
- [x] Global exception handling
- [x] Secure error responses
- [ ] Rate limiting (planned)
- [ ] API versioning (planned)
- [ ] Audit logging (planned) 