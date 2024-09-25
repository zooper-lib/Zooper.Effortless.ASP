using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;

public class MediatrNotificationAdapter<TNotification>(TNotification innerNotification) : MediatR.INotification
	where TNotification : INotification
{
	public TNotification InnerNotification { get; } = innerNotification;
}