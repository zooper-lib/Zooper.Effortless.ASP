using ZEA.Architecture.Patterns.ADTs.Errors;
using ZEA.Architecture.Patterns.ADTs.Helpers;
using ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Implementations;

/// <inheritdoc cref="IEitherStep{TData}"/>
public abstract class EitherStep<TOutput> : IEitherStep<TOutput>
{
	/// <inheritdoc cref="IEitherStep{TData}.ExecuteAsync"/>
	public abstract Task<Either<TOutput, LogicalError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <inheritdoc cref="IEitherStep{TInput,TOutput}"/>
public abstract class EitherStep<TInput, TOutput> : IEitherStep<TInput, TOutput>
{
	/// <inheritdoc cref="IEitherStep{TInput,TOutput}.ExecuteAsync"/>
	public abstract Task<Either<TOutput, LogicalError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}