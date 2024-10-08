using MassTransit;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Architectures.DDD.Abstractions.Interfaces;
using ZEA.Communications.Messaging.Abstractions;
using ZEA.Serializations.Abstractions.Interfaces;

namespace ZEA.Communications.Messaging.MassTransit.Implementations;

// ReSharper disable once UnusedType.Global
public class MassTransitEventPublisher(IBus bus, IJsonSerializer jsonSerializer) : IEventPublisher
{
	/// <inheritdoc/>
	[Obsolete("This method is obsolete. Use the PublishAsync method instead.")]
	public async Task Publish<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken) where TEvent : class, IEvent
	{
		await bus.Publish(@event, cancellationToken);
	}

	/// <inheritdoc/>
	public async Task PublishAsync<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken) where TEvent : class, IEvent
	{
		await bus.Publish(@event, cancellationToken);
	}

	/// <inheritdoc/>
	public async Task PublishAsync<TEvent>(
		TEvent @event,
		string metadataJson,
		CancellationToken cancellationToken) where TEvent : class, IEvent
	{
		if (string.IsNullOrWhiteSpace(metadataJson))
		{
			throw new ArgumentException("Metadata JSON cannot be null or empty.", nameof(metadataJson));
		}

		await bus.Publish(
			@event,
			context => { context.Headers.Set(MassTransitConstants.MetadataHeaderKey, metadataJson); },
			cancellationToken
		);
	}

	/// <inheritdoc/>
	public async Task PublishAsync<TEvent>(
		TEvent @event,
		Dictionary<string, object> metadata,
		CancellationToken cancellationToken) where TEvent : class, IEvent
	{
		await bus.Publish(
			@event,
			context =>
			{
				foreach (var kvp in metadata)
				{
					context.Headers.Set(kvp.Key, kvp.Value.ToString());
				}
			},
			cancellationToken
		);
	}

	/// <inheritdoc/>
	public async Task PublishAsync<TEvent>(
		TEvent @event,
		IMetadata metadata,
		CancellationToken cancellationToken) where TEvent : class, IEvent
	{
		var metadataJson = jsonSerializer.Serialize(metadata);

		await bus.Publish(
			@event,
			context => { context.Headers.Set(MassTransitConstants.MetadataHeaderKey, metadataJson); },
			cancellationToken
		);
	}
}