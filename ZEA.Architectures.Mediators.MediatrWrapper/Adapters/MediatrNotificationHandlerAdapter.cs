using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Adapters;

public class MediatrNotificationHandlerAdapter<TNotification>(INotificationHandler<TNotification> handler)
	: MediatR.INotificationHandler<MediatrNotificationAdapter<TNotification>>
	where TNotification : INotification
{
	public Task Handle(
		MediatrNotificationAdapter<TNotification> notification,
		CancellationToken cancellationToken)
	{
		return handler.HandleAsync(notification.InnerNotification, cancellationToken);
	}
}