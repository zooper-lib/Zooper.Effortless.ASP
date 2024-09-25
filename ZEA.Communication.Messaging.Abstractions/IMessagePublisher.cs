namespace ZEA.Communication.Messaging.Abstractions;

public interface IMessagePublisher : IPublisher
{
	Task PublishAsync<TMessage>(
		TMessage message,
		CancellationToken cancellationToken) where TMessage : class, IMessage;
}