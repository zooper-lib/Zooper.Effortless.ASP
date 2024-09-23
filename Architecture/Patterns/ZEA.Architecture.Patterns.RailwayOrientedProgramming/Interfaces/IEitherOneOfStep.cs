using OneOf;
using ZEA.Architecture.Patterns.ADTs.Helpers;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace ZEA.Architecture.Patterns.RailwayOrientedProgramming.Interfaces;

/// <summary>
/// Represents a generic step that returns either a result of type <typeparamref name="TOutput"/> 
/// or an error of type <typeparamref name="TError"/>. The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TOutput">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
[Obsolete("IEitherOneOfStep<TOutput, TError> is deprecated. Please use IEitherOneOfStep.Generator<TOutput, TError> instead.", false)]
public interface IEitherOneOfStep<TOutput, TError> : IStep
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>.</returns>
	Task<Either<TOutput, TError>> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a generic step that processes input data of type <typeparamref name="TInput"/> 
/// and returns either a result of type <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>. 
/// The error type must implement the <see cref="IOneOf"/> interface.
/// </summary>
/// <typeparam name="TInput">The type of the input data.</typeparam>
/// <typeparam name="TOutput">The type of the result.</typeparam>
/// <typeparam name="TError">The type of the error, must implement <see cref="IOneOf"/>.</typeparam>
[Obsolete("IEitherOneOfStep<TInput, TOutput, TError> is deprecated. Please use IEitherOneOfStep.Processor<TInput, TOutput, TError> instead.", false)]
public interface IEitherOneOfStep<in TInput, TOutput, TError> : IStep
	where TError : IOneOf
{
	/// <summary>
	/// Executes the step asynchronously with the specified input data.
	/// </summary>
	/// <param name="data">The input data for the step.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation, containing either the result <typeparamref name="TOutput"/> or an error of type <typeparamref name="TError"/>.</returns>
	Task<Either<TOutput, TError>> ExecuteAsync(
		TInput data,
		CancellationToken cancellationToken);
}

/// <summary>
/// Serves as the base interface for all EitherOneOfStep variations.
/// Defines a contract for steps that perform operations resulting in either a successful outcome or an error.
/// </summary>
public interface IEitherOneOfStep : IStep
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// <para>Defines a step that generates a result of type <typeparamref name="TOutput"/> 
	/// or produces an error of type <typeparamref name="TError"/>.
	/// This step operates without requiring any input data.</para>
	/// <para>Use the Generator interface when you need to create or produce a result without any external input. For example, generating a report, creating a new entity, or initializing default settings.</para>
	/// </summary>
	/// <typeparam name="TOutput">The type of the generated result.</typeparam>
	/// <typeparam name="TError">The type of the error, which must implement <see cref="IOneOf"/>.</typeparam>
	public interface Generator<TOutput, TError> : IEitherOneOfStep
		where TError : IOneOf
	{

		/// <summary>
		/// Executes the generation step asynchronously.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>
		/// A task representing the asynchronous operation, containing either the generated result of type <typeparamref name="TOutput"/> 
		/// or an error of type <typeparamref name="TError"/>.
		/// </returns>
		Task<Either<TOutput, TError>> ExecuteAsync(CancellationToken cancellationToken);
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// <para>Defines a step that processes input data of type <typeparamref name="TInput"/> 
	/// and produces either a result of type <typeparamref name="TOutput"/> 
	/// or an error of type <typeparamref name="TError"/>.</para>
	/// <para>Use the Processor interface when you need to transform or handle input data to produce an output. This is suitable for scenarios like data transformation, business logic processing, or handling user input.</para>
	/// </summary>
	/// <typeparam name="TInput">The type of the input data to be processed.</typeparam>
	/// <typeparam name="TOutput">The type of the resulting output after processing.</typeparam>
	/// <typeparam name="TError">The type of the error, which must implement <see cref="IOneOf"/>.</typeparam>
	public interface Processor<in TInput, TOutput, TError> : IEitherOneOfStep
		where TError : IOneOf
	{
		/// <summary>
		/// Executes the processing step asynchronously with the specified input data.
		/// </summary>
		/// <param name="data">The input data to be processed.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>
		/// A task representing the asynchronous operation, containing either the processed result of type <typeparamref name="TOutput"/> 
		/// or an error of type <typeparamref name="TError"/>.
		/// </returns>
		Task<Either<TOutput, TError>> ExecuteAsync(
			TInput data,
			CancellationToken cancellationToken);
	}

	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// <para>Defines a step that transforms data of type <typeparamref name="T"/> 
	/// and returns either the transformed data of the same type <typeparamref name="T"/> 
	/// or an error of type <typeparamref name="TError"/>.</para>
	/// <para>Use the Transformer interface when you need to modify or convert data while maintaining the same data type. This is ideal for scenarios like data validation, formatting, or applying consistent transformations to data objects.</para>
	/// </summary>
	/// <typeparam name="T">The type of both the input and transformed data.</typeparam>
	/// <typeparam name="TError">The type of the error, which must implement <see cref="IOneOf"/>.</typeparam>
	public interface Transformer<T, TError> : Processor<T, T, TError>
		where TError : IOneOf;
}