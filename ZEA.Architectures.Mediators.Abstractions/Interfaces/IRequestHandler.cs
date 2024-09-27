namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	Task<TResponse> HandleAsync(
		TRequest request,
		CancellationToken cancellationToken = default);
}