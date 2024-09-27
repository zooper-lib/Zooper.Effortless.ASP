using MassTransit;
using ZEA.Communications.Messaging.Abstractions;

namespace ZEA.Communications.Messaging.MassTransit;

// ReSharper disable once ClassNeverInstantiated.Global
public class MassTransitMessagePublisher(IBus bus) : IMessagePublisher
{
	public async Task PublishAsync<TMessage>(
		TMessage message,
		CancellationToken cancellationToken) where TMessage : class, IMessage
	{
		await bus.Publish(
			message,
			cancellationToken
		);
	}
}