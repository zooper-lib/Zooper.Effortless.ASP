using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Adapters;

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