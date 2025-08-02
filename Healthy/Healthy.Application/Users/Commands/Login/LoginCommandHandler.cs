using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, cancellationToken);

            if (user == null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Error = "Invalid email or password"
                };
            }

            // Verify password (in real app, use proper password hashing)
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Error = "Invalid email or password"
                };
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            // Create user DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            // Generate JWT token
            var token = _jwtService.GenerateToken(userDto);
            var refreshToken = _jwtService.GenerateRefreshToken();

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            return new AuthResult
            {
                Succeeded = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should get from JWT settings
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user {Email}", request.Email);
            return new AuthResult
            {
                Succeeded = false,
                Error = "An error occurred during login"
            };
        }
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // In real application, use proper password hashing like BCrypt
        // For now, using simple comparison (NOT for production)
        return password == passwordHash;
    }
} 