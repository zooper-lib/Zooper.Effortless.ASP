using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Adapters;

public class MediatrNotificationAdapter<TNotification>(TNotification innerNotification) : MediatR.INotification
	where TNotification : INotification
{
	public TNotification InnerNotification { get; } = innerNotification;
}