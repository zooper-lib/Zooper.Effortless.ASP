using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Techniques.RailwayOrientedProgramming.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Either{TLeft,TRight}"/> to support Railway-Oriented Programming.
/// These methods allow for chaining operations on Either results, handling both synchronous 
/// and asynchronous scenarios, and propagating errors through the chain.
/// </summary>
public static class EitherExtensions
{
	/// <summary>
	/// Chains a synchronous operation to an <see cref="Either{T, TError}"/> result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="either">The input Either instance</param>
	/// <param name="bind">The function to apply to the success case</param>
	/// <returns>A new Either instance with the transformed success case or the original error</returns>
	public static Either<TResult, TError> Then<T, TResult, TError>(
		this Either<T, TError> either,
		Func<T, Either<TResult, TError>> bind)
		where TError : notnull
	{
		return either.Match(
			bind,
			Either<TResult, TError>.FromRight
		);
	}

	/// <summary>
	/// Chains a synchronous operation to an asynchronous <see cref="Either{T, TError}"/> result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="task">The input Task of Either instance</param>
	/// <param name="bind">The function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of Either instance with the transformed success case or the original error</returns>
	public async static Task<Either<TResult, TError>> ThenAsync<T, TResult, TError>(
		this Task<Either<T, TError>> task,
		Func<T, Either<TResult, TError>> bind,
		CancellationToken cancellationToken = default)
		where TError : notnull
	{
		var result = await task.ConfigureAwait(false);
		cancellationToken.ThrowIfCancellationRequested();
		return result.Match(
			bind,
			Either<TResult, TError>.FromRight
		);
	}

	/// <summary>
	/// Chains an asynchronous operation to an asynchronous <see cref="Either{T, TError}"/> result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="task">The input Task of Either instance</param>
	/// <param name="bind">The asynchronous function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of Either instance with the transformed success case or the original error</returns>
	public async static Task<Either<TResult, TError>> ThenAsync<T, TResult, TError>(
		this Task<Either<T, TError>> task,
		Func<T, Task<Either<TResult, TError>>> bind,
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
			e => Task.FromResult(Either<TResult, TError>.FromRight(e))
		);
	}

	/// <summary>
	/// Chains an asynchronous operation to a synchronous <see cref="Either{T, TError}"/> result, transforming the success case.
	/// If the input is an error, it's passed through unchanged.
	/// </summary>
	/// <typeparam name="T">The input success type</typeparam>
	/// <typeparam name="TResult">The output success type</typeparam>
	/// <typeparam name="TError">The error type</typeparam>
	/// <param name="either">The input Either instance</param>
	/// <param name="bind">The asynchronous function to apply to the success case</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A new Task of Either instance with the transformed success case or the original error</returns>
	public async static Task<Either<TResult, TError>> ThenAsync<T, TResult, TError>(
		this Either<T, TError> either,
		Func<T, Task<Either<TResult, TError>>> bind,
		CancellationToken cancellationToken = default)
		where TError : notnull
	{
		cancellationToken.ThrowIfCancellationRequested();
		return await either.Match(
			async t =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				return await bind(t).ConfigureAwait(false);
			},
			e => Task.FromResult(Either<TResult, TError>.FromRight(e))
		);
	}
}