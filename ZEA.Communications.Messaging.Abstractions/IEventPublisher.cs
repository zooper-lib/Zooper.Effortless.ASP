// ReSharper disable UnusedType.Global

using ZEA.Architectures.DDD.Abstractions.Interfaces;

namespace ZEA.Communications.Messaging.Abstractions;

/// <summary>
/// Represents an event publisher capable of publishing events.
/// </summary>
public interface IEventPublisher : IPublisher
{
	/// <summary>
	/// Publishes the event.
	/// </summary>
	/// <typeparam name="TEvent">The type of the event.</typeparam>
	/// <param name="event">The event to be published.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous publish operation.</returns>
	[Obsolete("Use 'PublishAsync' instead.")]
	Task Publish<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken) where TEvent : class, IEvent;

	/// <summary>
	/// Publishes the event asynchronously.
	/// </summary>
	/// <typeparam name="TEvent">The type of the event.</typeparam>
	/// <param name="event">The event to be published.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous publish operation.</returns>
	Task PublishAsync<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken) where TEvent : class, IEvent;

	/// <summary>
	/// Publishes the event with metadata.
	/// </summary>
	/// <param name="event"></param>
	/// <param name="metadata"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TEvent"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TEvent>(
		TEvent @event,
		string metadata,
		CancellationToken cancellationToken) where TEvent : class, IEvent;

	/// <summary>
	/// Publishes the event with metadata.
	/// </summary>
	/// <param name="event"></param>
	/// <param name="metadata"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TEvent"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TEvent>(
		TEvent @event,
		Dictionary<string, dynamic> metadata,
		CancellationToken cancellationToken) where TEvent : class, IEvent;
}