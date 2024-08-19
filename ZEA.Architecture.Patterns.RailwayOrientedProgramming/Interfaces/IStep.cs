using OneOf;
using ZEA.Architecture.Patterns.ADTs.Errors;
using ZEA.Architecture.Patterns.ADTs.Helpers;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// Marker Interface
/// </summary>
public interface IStep;

/// <summary>
/// Represents a generic step that returns a result of type <typeparamref name="TResponse"/>.
/// The result must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TResponse">The type of the response, must implement <see cref="IOneOf"/>.</typeparam>
public abstract class OneOfStep<TResponse> : IStep
	where TResponse : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing a <typeparamref name="TResponse"/>.</returns>
	public abstract Task<TResponse> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TData"/> 
/// and returns a response of type <typeparamref name="TResponse"/>. 
/// The response must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TData">The type of the input data.</typeparam>
/// <typeparam name="TResponse">The type of the response, must implement <see cref="IOneOf"/>.</typeparam>
public abstract class OneOfStep<TData, TResponse> : IStep
	where TResponse : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing a <typeparamref name="TResponse"/>.</returns>
	public abstract Task<TResponse> ExecuteAsync(
		TData data,
		CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TData"/> 
/// and returns either the processed data or a <see cref="LogicalError"/>.
/// </summary>
/// <typeparam name="TData">The type of the input data.</typeparam>
public abstract class EitherStep<TData> : IStep
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the processed <typeparamref name="TData"/> or a <see cref="LogicalError"/>.</returns>
	public abstract Task<Either<TData, LogicalError>> ExecuteAsync(
		TData data,
		CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TInput"/> 
/// and returns either a result of type <typeparamref name="TOutput"/> or a <see cref="LogicalError"/>.
/// </summary>
/// <typeparam name="TInput">The type of the input data.</typeparam>
/// <typeparam name="TOutput">The type of the output result.</typeparam>
public abstract class EitherStep<TInput, TOutput> : IStep
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the output <typeparamref name="TOutput"/> or a <see cref="LogicalError"/>.</returns>
	public abstract Task<Either<TOutput, LogicalError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that returns either a result of type <typeparamref name="TResult"/> 
/// or an error of type <typeparamref name="TError"/>. The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
public abstract class EitherOneOfStep<TResult, TError> : IStep 
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TResult"/> or an error of type <typeparamref name="TError"/>.</returns>
	public abstract Task<Either<TResult, TError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TParam"/> 
/// and returns either a result of type <typeparamref name="TResult"/> or an error of type <typeparamref name="TError"/>. 
/// The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TParam">The type of the input data.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
public abstract class EitherOneOfStep<TParam, TResult, TError> : IStep 
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TResult"/> or an error of type <typeparamref name="TError"/>.</returns>
	public abstract Task<Either<TResult, TError>> ExecuteAsync(
		TParam data,
		CancellationToken cancellationToken);
}