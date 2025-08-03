# JWT Authorization and CurrentUser Service

## Overview

I have created a simple JWT authorization system with CurrentUser service to manage current user information.

## Components created:

### 1. CurrentUserInfo Model
- File: `Healthy.Application/Common/Models/CurrentUserInfo.cs`
- Contains complete user information: UserId, UserName, Email, FullName, Roles, Claims, etc.

### 2. ICurrentUserService Interface (updated)
- File: `Healthy.Application/Common/Interfaces/ICurrentUserService.cs`
- Added many methods to access user information

### 3. CurrentUserService Implementation (updated)
- File: `Healthy.Infrastructure/Services/CurrentUserService.cs`
- Implements lazy loading and caching within request scope

### 4. BaseController
- File: `Healthy.Api/Controllers/BaseController.cs`
- Base class for all controllers with utility methods

### 5. TestController (Demo)
- File: `Healthy.Api/Controllers/TestController.cs`
- Demo endpoints to test functionality

## Usage:

### 1. In Controller:
```csharp
public class MyController : BaseController
{
    [HttpGet]
    [Authorize]
    public ActionResult GetSomething()
    {
        // Check authentication
        if (!IsAuthenticated())
            return Unauthorized();

        // Get user ID
        var userId = GetCurrentUserId();
        
        // Get complete information
        var userInfo = CurrentUser.GetCurrentUserInfo();
        
        // Check role
        if (HasRole("Admin"))
        {
            // Admin logic
        }

        return Ok();
    }
}
```

### 2. In Service (via DI):
```csharp
public class MyService
{
    private readonly ICurrentUserService _currentUser;

    public MyService(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public void DoSomething()
    {
        if (_currentUser.IsAuthenticated)
        {
            var userId = _currentUser.UserId;
            var roles = _currentUser.Roles;
        }
    }
}
```

## Test Endpoints:

1. `GET /api/test/current-user-info` - Display current user information (requires login)
2. `GET /api/test/anonymous` - Test endpoint for anonymous user
3. `GET /api/test/admin-only` - Only admin can access

## Notes:

- CurrentUserService is registered with **Scoped** lifetime
- User information is lazy loaded and cached throughout the request
- JWT token validation still uses ASP.NET Core's built-in middleware
- BaseController provides helper methods for easy user info access
