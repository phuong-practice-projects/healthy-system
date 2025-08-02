# JWT Authentication Guide

## Overview

The Healthy System uses JWT (JSON Web Tokens) for authentication and authorization. This guide explains how to use the authentication system.

## Configuration

### JWT Settings

JWT configuration is stored in `appsettings.json`:

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

**Important**: Change the `SecretKey` in production to a secure, randomly generated key.

## API Endpoints

### Authentication Endpoints

#### 1. Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "password123",
  "confirmPassword": "password123",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-01-01",
  "gender": "Male"
}
```

**Response:**
```json
{
  "succeeded": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "roles": ["User"]
  }
}
```

#### 2. Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "password123"
}
```

**Response:** Same as register response.

### Protected Endpoints

#### 1. Get Current User Profile
```http
GET /api/users/me
Authorization: Bearer {your-jwt-token}
```

**Response:**
```json
{
  "id": "user-guid",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "fullName": "John Doe",
  "roles": ["User"]
}
```

#### 2. Get All Users (Admin Only)
```http
GET /api/users
Authorization: Bearer {your-jwt-token}
```

**Note:** This endpoint requires the "Admin" role.

## Using JWT Tokens

### 1. Include Token in Requests

Add the JWT token to the `Authorization` header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 2. Token Structure

The JWT token contains the following claims:
- `sub` (Subject): User ID
- `email`: User's email address
- `name`: User's full name
- `given_name`: User's first name
- `family_name`: User's last name
- `role`: User's roles (multiple roles possible)

### 3. Token Expiration

- **Access Token**: 60 minutes (configurable)
- **Refresh Token**: 7 days (configurable)

## Authorization

### Role-Based Authorization

The system supports role-based authorization:

#### Available Roles
- **Admin**: Full system access
- **User**: Standard user access
- **Moderator**: Content management access

#### Using Authorization Attributes

```csharp
[Authorize] // Requires authentication
[Authorize(Roles = "Admin")] // Requires Admin role
[Authorize(Roles = "Admin,Moderator")] // Requires Admin OR Moderator role
```

### Example Protected Controller

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class UsersController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")] // Only Admin can access
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        // Implementation
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        // Any authenticated user can access
    }
}
```

## Swagger UI

The API includes Swagger UI with JWT authentication support:

1. **Access Swagger UI**: Navigate to the root URL (e.g., `https://localhost:7001`)
2. **Authenticate**: Click the "Authorize" button
3. **Enter Token**: Use the format `Bearer {your-jwt-token}`
4. **Test Endpoints**: All protected endpoints will now work

## Error Responses

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "trace-id"
}
```

### 403 Forbidden
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "trace-id"
}
```

## Security Best Practices

1. **Use HTTPS**: Always use HTTPS in production
2. **Secure Secret Key**: Use a strong, randomly generated secret key
3. **Token Expiration**: Keep token expiration times reasonable
4. **Refresh Tokens**: Implement refresh token rotation
5. **Password Hashing**: Use proper password hashing (BCrypt recommended)
6. **Rate Limiting**: Implement rate limiting for auth endpoints
7. **Audit Logging**: Log authentication events

## Testing

### Using curl

```bash
# Register a user
curl -X POST "https://localhost:7001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@example.com",
    "password": "password123",
    "confirmPassword": "password123"
  }'

# Login
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'

# Access protected endpoint
curl -X GET "https://localhost:7001/api/users/me" \
  -H "Authorization: Bearer {your-jwt-token}"
```

### Using Postman

1. **Register/Login**: Send POST request to `/api/auth/register` or `/api/auth/login`
2. **Copy Token**: Copy the `token` from the response
3. **Set Authorization**: In the request headers, add:
   - Key: `Authorization`
   - Value: `Bearer {your-token}`
4. **Test Endpoints**: Send requests to protected endpoints

## Troubleshooting

### Common Issues

1. **401 Unauthorized**: Token is missing, invalid, or expired
2. **403 Forbidden**: User doesn't have the required role
3. **Token Expired**: Get a new token by logging in again
4. **Invalid Token Format**: Ensure token starts with "Bearer "

### Debugging

1. **Check Token**: Decode JWT token at jwt.io to verify claims
2. **Check Roles**: Ensure user has the required roles
3. **Check Expiration**: Verify token hasn't expired
4. **Check Configuration**: Verify JWT settings in appsettings.json 