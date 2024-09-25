namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

public interface INotificationHandler<in TNotification>
{
	Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}