using IMediator = ZEA.Architecture.PubSub.Abstractions.Interfaces.IMediator;
using INotification = ZEA.Architecture.PubSub.Abstractions.Interfaces.INotification;

namespace ZEA.Architecture.PubSub.MediatrWrapper;

public class MediatrMediatorAdapter(MediatR.IMediator mediator) : IMediator
{
	public Task<TResponse> SendAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken = default)
		where TRequest : Abstractions.Interfaces.IRequest<TResponse>
	{
		var adapterRequest = new MediatrRequestAdapter<TRequest, TResponse>(request);
		return mediator.Send(adapterRequest, cancellationToken);
	}

	public Task PublishAsync<TNotification>(
		TNotification notification,
		CancellationToken cancellationToken = default)
		where TNotification : INotification
	{
		// Implement if needed
		throw new NotImplementedException();
	}
}