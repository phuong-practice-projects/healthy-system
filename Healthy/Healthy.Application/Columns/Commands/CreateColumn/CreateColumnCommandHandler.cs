using MediatR;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Columns.Commands.CreateColumn;

public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateColumnCommandHandler> _logger;

    public CreateColumnCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateColumnCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var column = new Column
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Category = request.Category,
                ImageUrl = request.ImageUrl,
                Tags = request.Tags,
                IsPublished = request.IsPublished,
                CreatedAt = DateTime.UtcNow
            };

            _context.Columns.Add(column);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Column {Id} created successfully", column.Id);
            return Result<Guid>.Success(column.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating column");
            return Result<Guid>.Failure("An error occurred while creating the column");
        }
    }
}
