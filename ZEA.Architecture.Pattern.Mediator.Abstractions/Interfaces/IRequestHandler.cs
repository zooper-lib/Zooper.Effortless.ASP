namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	Task<TResponse> HandleAsync(
		TRequest request,
		CancellationToken cancellationToken = default);
}