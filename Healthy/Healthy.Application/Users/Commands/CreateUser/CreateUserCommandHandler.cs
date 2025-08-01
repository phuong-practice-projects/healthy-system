using MediatR;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IDateTime _dateTime;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateUserCommandHandler> logger,
        IDateTime dateTime)
    {
        _context = context;
        _logger = logger;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user with email already exists
            var existingUser = _context.Set<User>()
                .FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);

            if (existingUser != null)
            {
                return Result<Guid>.Failure($"User with email '{request.Email}' already exists.");
            }

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // In production, use a proper password hashing service
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsActive = true
            };

            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", request.Email);
            return Result<Guid>.Failure("An error occurred while creating the user.");
        }
    }
} 