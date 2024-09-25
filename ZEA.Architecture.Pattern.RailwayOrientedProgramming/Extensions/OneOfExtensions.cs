using OneOf;

namespace ZEA.Architecture.Pattern.RailwayOrientedProgramming.Extensions;

/// <summary>
/// Provides extension methods for OneOf&lt;T, TError&gt; to support Railway-Oriented Programming.
/// These methods allow for chaining operations on OneOf results, handling both synchronous 
/// and asynchronous scenarios, and propagating errors through the chain.
/// </summary>
public static class OneOfExtensions
{
	/// <summary>
	/// Chains a synchronous operation to a OneOf result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="oneOf">The input OneOf instance</param>
	/// <param name="bind">The function to apply to the success case</param>
	/// <returns>A new OneOf instance with the transformed success case or the original error</returns>
	public static OneOf<TResult, TError> Then<T, TResult, TError>(
		this OneOf<T, TError> oneOf,
		Func<T, OneOf<TResult, TError>> bind)
		where TError : notnull
	{
		return oneOf.Match(
			bind,
			OneOf<TResult, TError>.FromT1
		);
	}

	/// <summary>
	/// Chains a synchronous operation to an asynchronous OneOf result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="task">The input Task of OneOf instance</param>
	/// <param name="bind">The function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of OneOf instance with the transformed success case or the original error</returns>
	public async static Task<OneOf<TResult, TError>> ThenAsync<T, TResult, TError>(
		this Task<OneOf<T, TError>> task,
		Func<T, OneOf<TResult, TError>> bind,
		CancellationToken cancellationToken = default)
		where TError : notnull
	{
		var result = await task.ConfigureAwait(false);
		cancellationToken.ThrowIfCancellationRequested();
		return result.Match(
			bind,
			OneOf<TResult, TError>.FromT1
		);
	}

	/// <summary>
	/// Chains an asynchronous operation to an asynchronous OneOf result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="task">The input Task of OneOf instance</param>
	/// <param name="bind">The asynchronous function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of OneOf instance with the transformed success case or the original error</returns>
	public async static Task<OneOf<TResult, TError>> ThenAsync<T, TResult, TError>(
		this Task<OneOf<T, TError>> task,
		Func<T, Task<OneOf<TResult, TError>>> bind,
		CancellationToken cancellationToken = default)
		where TError : notnull
	{
		var result = await task.ConfigureAwait(false);
		cancellationToken.ThrowIfCancellationRequested();
		return await result.Match(
			async t =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				return await bind(t).ConfigureAwait(false);
			},
			e => Task.FromResult(OneOf<TResult, TError>.FromT1(e))
		);
	}

	/// <summary>
	/// Chains an asynchronous operation to a synchronous OneOf result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="oneOf">The input OneOf instance</param>
	/// <param name="bind">The asynchronous function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of OneOf instance with the transformed success case or the original error</returns>
	public async static Task<OneOf<TResult, TError>> ThenAsync<T, TResult, TError>(
		this OneOf<T, TError> oneOf,
		Func<T, Task<OneOf<TResult, TError>>> bind,
		CancellationToken cancellationToken = default)
		where TError : notnull
	{
		cancellationToken.ThrowIfCancellationRequested();
		return await oneOf.Match(
			async t =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				return await bind(t).ConfigureAwait(false);
			},
			e => Task.FromResult(OneOf<TResult, TError>.FromT1(e))
		);
	}
}