using OneOf;
using ZEA.Architecture.Patterns.ADTs.Helpers;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// Represents a generic step that returns either a result of type <typeparamref name="TOutput"/> 
/// or an error of type <typeparamref name="TError"/>. The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TOutput">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
public interface IEitherOneOfStep<TOutput, TError> : IStep
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>.</returns>
	Task<Either<TOutput, TError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TInput"/> 
/// and returns either a result of type <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>. 
/// The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TInput">The type of the input data.</typeparam>
/// <typeparam name="TOutput">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
public interface IEitherOneOfStep<in TInput, TOutput, TError> : IStep
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>.</returns>
	Task<Either<TOutput, TError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}