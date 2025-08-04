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
- **Entities**: Business objects with identity (User, Role, UserRole)
- **Value Objects**: Immutable objects representing concepts (Email, PhoneNumber)
- **Domain Events**: Events that occur within the domain
- **Domain Services**: Business logic that doesn't belong to a specific entity
- **Interfaces**: Contracts for domain services

**Dependencies**: None (pure business logic)

### 2. Application Layer (`Healthy.Application`)
**Purpose**: Contains use cases, application services, and orchestration logic.

**Key Components**:
- **Commands**: Write operations (CreateUserCommand, UpdateUserCommand)
- **Queries**: Read operations (GetUsersQuery, GetUserByIdQuery)
- **Handlers**: Command and Query handlers implementing business logic
- **DTOs**: Data Transfer Objects for API responses
- **Validators**: FluentValidation rules for commands
- **Interfaces**: Contracts for external services
- **Behaviors**: MediatR pipeline behaviors (Validation, Logging)

**Dependencies**: Domain Layer

### 3. Infrastructure Layer (`Healthy.Infrastructure`)
**Purpose**: Contains external concerns like database, external APIs, file system.

**Key Components**:
- **Persistence**: Entity Framework DbContext and configurations
- **Services**: Implementation of application interfaces
- **External APIs**: Integration with third-party services
- **Configuration**: Database connections, external service settings

**Dependencies**: Application Layer, Domain Layer

### 4. Presentation Layer (`Healthy.Api`)
**Purpose**: Contains controllers, API endpoints, and presentation logic.

**Key Components**:
- **Controllers**: API endpoints with Swagger documentation
- **Middleware**: Request/response processing
- **Configuration**: Dependency injection setup
- **Swagger**: API documentation

**Dependencies**: Application Layer

## CQRS Pattern Implementation

### Commands (Write Operations)
Commands represent write operations and follow these conventions:

```csharp
public record CreateUserCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
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
    
    return services;
}
```

## Swagger Documentation

The API includes comprehensive Swagger documentation with:

- **Operation Descriptions**: Detailed descriptions for each endpoint
- **Request/Response Examples**: Sample data for testing
- **Response Codes**: All possible HTTP status codes
- **Authentication**: JWT token support (when implemented)
- **Validation**: Automatic validation error documentation

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

### Soft Delete Pattern
All entities implement soft delete using the `IsDeleted` flag:

```csharp
public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
    // ... other properties
}
```

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

## Testing Strategy

### Unit Tests
- **Domain Logic**: Test business rules and domain events
- **Application Logic**: Test command/query handlers
- **Validation**: Test FluentValidation rules

### Integration Tests
- **Database Operations**: Test Entity Framework operations
- **API Endpoints**: Test complete request/response cycles
- **External Services**: Test third-party integrations

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

### 4. Validation Strategy
- **Input Validation**: FluentValidation for commands
- **Business Rules**: Domain entities enforce business rules
- **Error Handling**: Consistent error responses

## Module Organization

Each module (User, Health, etc.) follows this structure:

```
ModuleName/
├── Commands/
│   ├── CreateModule/
│   ├── UpdateModule/
│   └── DeleteModule/
├── Queries/
│   ├── GetModules/
│   └── GetModuleById/
└── DTOs/
    └── ModuleDto.cs
```

## Future Enhancements

1. **Event Sourcing**: Implement event sourcing for audit trails
2. **Caching**: Add Redis caching for frequently accessed data
3. **Authentication**: Implement JWT authentication
4. **Authorization**: Add role-based access control
5. **Background Jobs**: Implement Hangfire for background processing
6. **Monitoring**: Add Application Insights for monitoring
7. **Rate Limiting**: Implement API rate limiting
8. **API Versioning**: Add API versioning support

## Getting Started

1. **Build the Solution**:
   ```bash
   dotnet build
   ```

2. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**:
   Navigate to `https://localhost:5001` for the Swagger documentation.

## Contributing

When adding new features:

1. **Domain Layer**: Add entities and domain logic
2. **Application Layer**: Add commands/queries and handlers
3. **Infrastructure Layer**: Add persistence and external services
4. **Presentation Layer**: Add controllers and API endpoints
5. **Tests**: Add unit and integration tests

Follow the existing patterns and conventions for consistency. 