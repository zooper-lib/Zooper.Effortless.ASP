using ZEA.Communication.Messaging.Abstractions;

namespace ZEA.Architecture.EventSourcing.Interfaces;

/// <summary>
/// Defines the EventProcessor interface for the Aggregate.
/// This is used for implementing "Event Sourcing", and you will need to implement this interface for each Aggregate.
/// </summary>
/// <typeparam name="TAggregate">The Aggregate we are processing</typeparam>
public interface IEventProcessor<TAggregate>
{
	/// <summary>
	/// Processes the Event and returns the updated Aggregate.
	/// You can pass the Aggregate as null if it is a new Aggregate, probably when a "create" Event is processed.
	/// </summary>
	/// <param name="aggregate">The Aggregate, or null</param>
	/// <param name="event">The Event we are processing</param>
	/// <returns>The updated Aggregate</returns>
	TAggregate ProcessEvent(
		TAggregate? aggregate,
		IEvent @event);
}