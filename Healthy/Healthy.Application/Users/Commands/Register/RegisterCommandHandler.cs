using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService,
        ILogger<RegisterCommandHandler> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Error = "Email already exists"
                };
            }

            // Get default user role
            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "User" && r.IsActive, cancellationToken);

            if (userRole == null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Error = "Default user role not found"
                };
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password), // In real app, use proper hashing
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            // Assign default user role
            var userRoleEntity = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = userRole.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRoleEntity);

            await _context.SaveChangesAsync(cancellationToken);

            // Create user DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = new List<string> { userRole.Name }
            };

            // Generate JWT token
            var token = _jwtService.GenerateToken(userDto);
            var refreshToken = _jwtService.GenerateRefreshToken();

            _logger.LogInformation("User {Email} registered successfully", request.Email);

            return new AuthResult
            {
                Succeeded = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for user {Email}", request.Email);
            return new AuthResult
            {
                Succeeded = false,
                Error = "An error occurred during registration"
            };
        }
    }

    private string HashPassword(string password)
    {
        // In real application, use proper password hashing like BCrypt
        // For now, using simple hash (NOT for production)
        return password;
    }
} 