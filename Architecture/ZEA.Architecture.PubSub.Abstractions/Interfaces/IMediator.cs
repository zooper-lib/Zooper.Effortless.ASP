namespace ZEA.Architecture.PubSub.Abstractions.Interfaces;

public interface IMediator
{
	Task<TResponse> SendAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken = default)
		where TRequest : IRequest<TResponse>;

	Task PublishAsync<TNotification>(
		TNotification notification,
		CancellationToken cancellationToken = default)
		where TNotification : INotification;
}