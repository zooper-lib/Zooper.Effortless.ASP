using OneOf;
using ZEA.Architecture.Patterns.ADTs.Helpers;
using ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Implementations;

/// <inheritdoc cref="IEitherOneOfStep{TResult,TError}"/>
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