using OneOf;
using ZEA.Techniques.ADTs.Helpers;
using ZEA.Techniques.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Techniques.RailwayOrientedProgramming.Implementations;

/// <inheritdoc cref="IEitherOneOfStep{TOutput,TError}"/>
public abstract class EitherOneOfStep<TOutput, TError> : IEitherOneOfStep<TOutput, TError>
	where TError : IOneOf
{
	/// <inheritdoc cref="IEitherOneOfStep{TResult,TError}.ExecuteAsync"/>
	public abstract Task<Either<TOutput, TError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <inheritdoc cref="IEitherOneOfStep{TParam,TResult,TError}"/>
public abstract class EitherOneOfStep<TInput, TOutput, TError> : IEitherOneOfStep<TInput, TOutput, TError>
	where TError : IOneOf
{
	/// <inheritdoc cref="IEitherOneOfStep{TParam,TResult,TError}.ExecuteAsync"/>
	public abstract Task<Either<TOutput, TError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}