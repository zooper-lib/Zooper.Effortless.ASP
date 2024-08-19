using OneOf;

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// <para>Represents a step in the Railway Oriented Programming (ROP) pattern.</para>
/// <para></para>
/// <para>Note: The <see cref="TContext"/> is the context which is used in the whole ROP call.</para>
/// <para>Note: The <see cref="TError"/> is the error object which is used in the whole ROP call.</para>
/// </summary>
/// <typeparam name="TContext">The Context holding all the data.</typeparam>
/// <typeparam name="TError">An object of a OneOf type which holds all the possible errors</typeparam>
public interface IStep<TContext, TError> where TError : IOneOf
{
	Task<OneOf<TContext, TError>> ExecuteAsync(
		TContext context,
		CancellationToken cancellationToken);
}