using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;

public class MediatrRequestHandlerAdapter<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
	: MediatR.IRequestHandler<MediatrRequestAdapter<TRequest, TResponse>, TResponse>
	where TRequest : IRequest<TResponse>
{
	public Task<TResponse> Handle(
		MediatrRequestAdapter<TRequest, TResponse> request,
		CancellationToken cancellationToken)
	{
		return handler.HandleAsync(request.InnerRequest, cancellationToken);
	}
}