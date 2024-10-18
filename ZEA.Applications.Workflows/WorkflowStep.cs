using OneOf;
using ZEA.Techniques.ADTs.Helpers;
using ZEA.Techniques.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Represents the base class for defining a step in a workflow that processes context
/// and returns either the updated context or an error.
/// </summary>
/// <typeparam name="TContext">The type of the context shared across workflow steps.</typeparam>
/// <typeparam name="TError">The type of the error returned on failure.</typeparam>
public abstract class WorkflowStep<TContext, TError> : IEitherOneOfStep.Transformer<TContext, TError> where TError : IOneOf
{
	/// <summary>
	/// Executes the workflow step logic, processing the provided context and returning either an updated context or an error.
	/// </summary>
	/// <param name="context">The workflow context at the current step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that resolves to either an updated context or an error.</returns>
	public abstract Task<Either<TContext, TError>> ExecuteAsync(TContext context, CancellationToken cancellationToken);

	/// <summary>
	/// Helper method to return a successful result containing the updated context.
	/// </summary>
	/// <param name="context">The updated workflow context.</param>
	/// <returns>An Either type representing success with the provided context.</returns>
	protected Either<TContext, TError> Success(TContext context) => Either<TContext, TError>.FromLeft(context);

	/// <summary>
	/// Helper method to return a failure result containing the error.
	/// </summary>
	/// <param name="error">The error that caused the failure.</param>
	/// <returns>An Either type representing failure with the provided error.</returns>
	protected Either<TContext, TError> Failure(TError error) => Either<TContext, TError>.FromRight(error);
}