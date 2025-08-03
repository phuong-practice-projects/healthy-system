using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Users.Commands.Login;

public class LoginCommandHandler(
    IApplicationDbContext context,
    IJwtService jwtService,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email
            var user = await context.Users
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
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Error = "Invalid email or password"
                };
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

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
            var token = jwtService.GenerateToken(userDto);
            var refreshToken = jwtService.GenerateRefreshToken();

            logger.LogInformation("User {Email} logged in successfully", request.Email);

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
            logger.LogError(ex, "Error occurred during login for user {Email}", request.Email);
            return new AuthResult
            {
                Succeeded = false,
                Error = "An error occurred during login"
            };
        }
    }
} 