using ZEA.Architectures.Mediators.Abstractions.Interfaces;
using IMediator = ZEA.Architectures.Mediators.Abstractions.Interfaces.IMediator;
using INotification = ZEA.Architectures.Mediators.Abstractions.Interfaces.INotification;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Adapters;

public class MediatrMediatorAdapter(MediatR.IMediator mediator) : IMediator
{
	public Task<TResponse> SendAsync<TResponse>(
		IRequest<TResponse> request,
		CancellationToken cancellationToken = default)
	{
		var adapterRequest = new MediatrRequestAdapter<IRequest<TResponse>, TResponse>(request);
		return mediator.Send(adapterRequest, cancellationToken);
	}

	public Task<TResponse> SendAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken = default)
		where TRequest : IRequest<TResponse>
	{
		var adapterRequest = new MediatrRequestAdapter<TRequest, TResponse>(request);
		return mediator.Send(adapterRequest, cancellationToken);
	}

	public Task PublishAsync<TNotification>(
		TNotification notification,
		CancellationToken cancellationToken = default)
		where TNotification : INotification
	{
		var adapterNotification = new MediatrNotificationAdapter<TNotification>(notification);
		return mediator.Publish(adapterNotification, cancellationToken);
	}
}