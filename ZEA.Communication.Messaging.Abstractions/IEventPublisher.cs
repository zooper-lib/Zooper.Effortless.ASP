// ReSharper disable UnusedType.Global

using ZEA.Architecture.DDD.Interfaces;

namespace ZEA.Communication.Messaging.Abstractions;

/// <summary>
/// Represents an event publisher capable of publishing events.
/// </summary>
public interface IEventPublisher : IPublisher
{
	/// <summary>
	/// Publishes the specified event.
	/// </summary>
	/// <typeparam name="TEvent">The type of the event.</typeparam>
	/// <param name="event">The event to be published.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous publish operation.</returns>
	Task Publish<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken) where TEvent : class, IEvent;
}