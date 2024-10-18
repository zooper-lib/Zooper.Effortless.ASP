using MediatR;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Represents the base class for defining a workflow handler that processes a request
/// and returns either a successful response or an error. Implements MediatR's IRequestHandler interface.
/// </summary>
/// <typeparam name="TRequest">The type of the workflow request.</typeparam>
/// <typeparam name="TContext">The type of the context shared across workflow steps.</typeparam>
/// <typeparam name="TResponse">The type of the response returned on successful execution.</typeparam>
/// <typeparam name="TError">The type of the error returned on failure.</typeparam>
public abstract class Workflow<TRequest, TContext, TResponse, TError> : IRequestHandler<TRequest, Either<TResponse, TError>>
	where TRequest : WorkflowRequest<TResponse, TError>
{
	/// <summary>
	/// Handles the workflow execution for the given request and returns either a response or an error.
	/// </summary>
	/// <param name="request">The workflow request.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that resolves to either a successful response or an error.</returns>
	public abstract Task<Either<TResponse, TError>> Handle(
		TRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Helper method to return a successful result containing the response.
	/// </summary>
	/// <param name="response">The successful response.</param>
	/// <returns>An Either type representing success with the provided response.</returns>
	protected Either<TResponse, TError> Success(TResponse response) => Either<TResponse, TError>.FromLeft(response);

	/// <summary>
	/// Helper method to return a failure result containing the error.
	/// </summary>
	/// <param name="error">The error that caused the failure.</param>
	/// <returns>An Either type representing failure with the provided error.</returns>
	protected Either<TResponse, TError> Failure(TError error) => Either<TResponse, TError>.FromRight(error);
}