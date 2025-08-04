using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Queries.GetDiaries;

public class GetDiariesQueryHandler : IRequestHandler<GetDiariesQuery, DiaryListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetDiariesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DiaryListResponse> Handle(GetDiariesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Diaries
            .AsNoTracking()
            .Where(d => d.UserId.ToString().ToLower() == request.UserId.ToString().ToLower());

        if (request.StartDate.HasValue)
        {
            query = query.Where(d => d.DiaryDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(d => d.DiaryDate <= request.EndDate.Value);
        }

        if (request.IsPrivate.HasValue)
        {
            query = query.Where(d => d.IsPrivate == request.IsPrivate.Value);
        }

        // Get total count first
        var totalItems = await query.CountAsync(cancellationToken);

        var diaries = await query
            .OrderByDescending(d => d.DiaryDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DiaryDto
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                Tags = d.Tags,
                Mood = d.Mood,
                IsPrivate = d.IsPrivate,
                DiaryDate = d.DiaryDate
            })
            .ToListAsync(cancellationToken);

        return DiaryListResponse.Create(diaries, totalItems, request.Page, request.PageSize);
    }
}
