namespace ZEA.Architecture.PubSub.Mediat;

public class MediatRRequestHandlerAdapter<TRequest, TResponse> : MediatR.IRequestHandler<MediatRRequestAdapter<TRequest, TResponse>, TResponse>
	where TRequest : Abstractions.Interfaces.IRequest<TResponse>
{
	private readonly Abstractions.Interfaces.IRequestHandler<TRequest, TResponse> _handler;

	public MediatRRequestHandlerAdapter(Abstractions.Interfaces.IRequestHandler<TRequest, TResponse> handler)
	{
		_handler = handler;
	}

	public Task<TResponse> Handle(
		MediatRRequestAdapter<TRequest, TResponse> request,
		CancellationToken cancellationToken)
	{
		return _handler.HandleAsync(request.InnerRequest, cancellationToken);
	}
}