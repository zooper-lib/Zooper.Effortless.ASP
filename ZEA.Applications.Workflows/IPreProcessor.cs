using ZEA.Techniques.ADTs;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Marker interface for pre-processors.
/// </summary>
public interface IPreProcessor;

/// <summary>
/// Defines a pre-processor that can perform operations on a request before it reaches the main handler.
/// It returns either a `Success` object if pre-processing passes or a `TError` if it fails.
/// </summary>
/// <typeparam name="TRequest">The type of the request message.</typeparam>
/// <typeparam name="TError">The type representing the error.</typeparam>
public interface IPreProcessor<in TRequest, TError> : IPreProcessor
{
	/// <summary>
	/// Processes the incoming request. Returns a `Success` if the request passes pre-processing.
	/// Returns a `TError` if pre-processing fails.
	/// </summary>
	/// <param name="request">The request message to process.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>
	/// A task representing the asynchronous operation. The result is an `Either<Success, TError>`,
	/// indicating success or failure.
	/// </returns>
	Task<Either<Success, TError>> ProcessAsync(
		TRequest request,
		CancellationToken cancellationToken);
}