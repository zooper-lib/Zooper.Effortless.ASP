using ZEA.Techniques.ADTs.Errors;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Techniques.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TOutput"/> 
/// and returns either the processed data or a <see cref="LogicalError"/>.
/// </summary>
/// <typeparam name="TOutput">The type of the response data.</typeparam>
public interface IEitherStep<TOutput> : IStep
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the processed <typeparamref name="TOutput"/> or a <see cref="LogicalError"/>.</returns>
	Task<Either<TOutput, LogicalError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TInput"/> 
/// and returns either a result of type <typeparamref name="TOutput"/> or a <see cref="LogicalError"/>.
/// </summary>
/// <typeparam name="TInput">The type of the input data.</typeparam>
/// <typeparam name="TOutput">The type of the output result.</typeparam>
public interface IEitherStep<in TInput, TOutput> : IStep
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the output <typeparamref name="TOutput"/> or a <see cref="LogicalError"/>.</returns>
	Task<Either<TOutput, LogicalError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}