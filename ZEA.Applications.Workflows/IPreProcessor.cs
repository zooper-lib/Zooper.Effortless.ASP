namespace ZEA.Applications.Workflows;

/// <summary>
/// Marker interface for pre-processors.
/// </summary>
public interface IPreProcessor;

/// <summary>
/// Defines a pre-processor that can perform operations on a request before it reaches the main handler.
/// Common use cases include validation, authentication, and logging.
/// </summary>
/// <typeparam name="TRequest">The type of the request message.</typeparam>
/// <typeparam name="TResponse">The type of the response message.</typeparam>
public interface IPreProcessor<in TRequest, TResponse> : IPreProcessor
{
	/// <summary>
	/// Processes the incoming request. Returns null if the request passes pre-processing.
	/// Returns a TResponse containing error information if pre-processing fails.
	/// </summary>
	/// <param name="request">The request message to process.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The result is null if pre-processing succeeds;
	/// otherwise, a TResponse representing the error.
	/// </returns>
	Task<TResponse?> ProcessAsync(
		TRequest request,
		CancellationToken cancellationToken);
}