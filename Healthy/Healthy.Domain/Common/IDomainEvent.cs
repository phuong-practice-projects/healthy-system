namespace Healthy.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
} 