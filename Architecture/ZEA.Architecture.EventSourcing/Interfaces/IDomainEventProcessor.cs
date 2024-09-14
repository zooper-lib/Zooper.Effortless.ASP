using ZEA.Architecture.DDD.Interfaces;

namespace ZEA.Architecture.EventSourcing.Interfaces;

/// <summary>
/// Defines the EventProcessor interface for the Aggregate.
/// This is used for implementing "Event Sourcing", and you will need to implement this interface for each Aggregate.
/// </summary>
/// <typeparam name="TAggregate">The Aggregate we are processing</typeparam>
public interface IDomainEventProcessor<TAggregate>
{
	/// <summary>
	/// Processes the <see cref="IDomainEvent"/> and returns the updated Aggregate.
	/// You can pass the Aggregate as null if it is a new Aggregate, probably when a "create" Event is processed.
	/// </summary>
	/// <param name="aggregate">The Aggregate, or null</param>
	/// <param name="event">The Event we are processing</param>
	/// <returns>The updated Aggregate</returns>
	TAggregate ProcessEvent(
		TAggregate? aggregate,
		IDomainEvent @event);
}