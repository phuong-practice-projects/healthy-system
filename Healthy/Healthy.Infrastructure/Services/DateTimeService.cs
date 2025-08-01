using Healthy.Application.Common.Interfaces;

namespace Healthy.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    
    public DateTime UtcNow => DateTime.UtcNow;
} 