using OneOf;
using ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Implementations;

/// <inheritdoc cref="IOneOfStep{TResponse}"/>
public abstract class OneOfStep<TOutput> : IOneOfStep<TOutput>
	where TOutput : IOneOf
{
	/// <inheritdoc cref="IOneOfStep{TResponse}.ExecuteAsync"/>
	public abstract Task<TOutput> ExecuteAsync(CancellationToken cancellationToken);
}

/// <inheritdoc cref="IOneOfStep{TData,TResponse}"/>
public abstract class OneOfStep<TInput, TOutput> : IOneOfStep<TInput, TOutput>
	where TOutput : IOneOf
{
	/// <inheritdoc cref="IOneOfStep{TData,TResponse}.ExecuteAsync"/>
	public abstract Task<TOutput> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}