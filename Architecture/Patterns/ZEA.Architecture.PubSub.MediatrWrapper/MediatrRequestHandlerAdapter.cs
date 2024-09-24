namespace ZEA.Architecture.PubSub.MediatrWrapper;

public class MediatrRequestHandlerAdapter<TRequest, TResponse>(Abstractions.Interfaces.IRequestHandler<TRequest, TResponse> handler)
	: MediatR.IRequestHandler<MediatrRequestAdapter<TRequest, TResponse>, TResponse>
	where TRequest : Abstractions.Interfaces.IRequest<TResponse>
{
	public Task<TResponse> Handle(
		MediatrRequestAdapter<TRequest, TResponse> request,
		CancellationToken cancellationToken)
	{
		return handler.HandleAsync(request.InnerRequest, cancellationToken);
	}
}