using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.UseCases.Users.Commands.Register;

public class RegisterCommandHandler(
    IApplicationDbContext context,
    IJwtService jwtService,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            var existingUser = await context.Users
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
            var userRole = await context.Roles
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password,12), // In real app, use proper hashing
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            // Assign default user role
            var userRoleEntity = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = userRole.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.UserRoles.Add(userRoleEntity);

            await context.SaveChangesAsync(cancellationToken);

            // Create user DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = [userRole.Name]
            };

            // Generate JWT token
            var token = jwtService.GenerateToken(userDto);
            var refreshToken = jwtService.GenerateRefreshToken();

            logger.LogInformation("User {Email} registered successfully", request.Email);

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
            logger.LogError(ex, "Error occurred during registration for user {Email}", request.Email);
            return new AuthResult
            {
                Succeeded = false,
                Error = "An error occurred during registration"
            };
        }
    }
} 