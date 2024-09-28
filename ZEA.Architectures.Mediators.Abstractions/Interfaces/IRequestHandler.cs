namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

/// <summary>
/// Defines a handler for a request of type <typeparamref name="TRequest"/> and response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled, which implements <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response expected from the handler.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	/// <summary>
	/// Asynchronously handles the request and returns a response.
	/// </summary>
	/// <param name="request">The request instance.</param>
	/// <param name="cancellationToken">Optional cancellation token to cancel the request handling.</param>
	/// <returns>A task representing the asynchronous operation, with the response from the handler.</returns>
	Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}