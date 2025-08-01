using MediatR;

namespace Healthy.Domain.Common;

public abstract class BaseDomainEvent : INotification, IDomainEvent
{
    public DateTime OccurredOn { get; protected set; }
    
    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
} 