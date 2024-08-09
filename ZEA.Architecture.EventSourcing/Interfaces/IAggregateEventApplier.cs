using ZEA.Architecture.DDD.Interfaces;

namespace ZEA.Architecture.EventSourcing.Interfaces;

/// <summary>
/// Defines the interface for applying a specific version of a domain event to an aggregate.
/// </summary>
/// <typeparam name="TEvent">The type of the event</typeparam>
/// <typeparam name="TAggregate">The type of the aggregate being processed</typeparam>
public interface IAggregateEventApplier<in TEvent, TAggregate>
	where TEvent : IDomainEvent
{
	TAggregate Apply(
		TAggregate? aggregate,
		TEvent domainEvent);
}