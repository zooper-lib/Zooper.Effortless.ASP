using OneOf;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// Represents a generic step that returns a result of type <typeparamref name="TOutput"/>.
/// The result must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TOutput">The type of the response, must implement <see cref="IOneOf"/>.</typeparam>
public interface IOneOfStep<TOutput> : IStep
	where TOutput : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing a <typeparamref name="TOutput"/>.</returns>
	Task<TOutput> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TInput"/> 
/// and returns a response of type <typeparamref name="TOutput"/>. 
/// The response must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TInput">The type of the input data.</typeparam>
/// <typeparam name="TOutput">The type of the response, must implement <see cref="IOneOf"/>.</typeparam>
public interface IOneOfStep<in TInput, TOutput> : IStep
	where TOutput : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing a <typeparamref name="TOutput"/>.</returns>
	Task<TOutput> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}