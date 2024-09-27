using OneOf;
using ZEA.Techniques.RailwayOrientedProgramming.Interfaces;

namespace ZEA.Techniques.RailwayOrientedProgramming.Implementations;

/// <inheritdoc cref="IOneOfStep{TOutput}"/>
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