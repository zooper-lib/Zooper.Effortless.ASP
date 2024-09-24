using IMediator = ZEA.Architecture.PubSub.Abstractions.Interfaces.IMediator;
using INotification = ZEA.Architecture.PubSub.Abstractions.Interfaces.INotification;

namespace ZEA.Architecture.PubSub.Mediat;

public class MediatRMediatorAdapter : IMediator
{
	private readonly MediatR.IMediator _mediator;

	public MediatRMediatorAdapter(MediatR.IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task<TResponse> SendAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken = default)
		where TRequest : Abstractions.Interfaces.IRequest<TResponse>
	{
		var adapterRequest = new MediatRRequestAdapter<TRequest, TResponse>(request);
		return _mediator.Send(adapterRequest, cancellationToken);
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