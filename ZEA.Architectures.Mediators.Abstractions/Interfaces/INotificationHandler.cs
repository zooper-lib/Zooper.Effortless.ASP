namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

public interface INotificationHandler<in TNotification>
{
	Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}