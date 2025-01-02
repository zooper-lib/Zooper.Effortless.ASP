using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Represents a builder for a workflow that processes a request and
/// either succeeds with a <typeparamref name="TSuccess"/> result
/// or fails with a <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request input.</typeparam>
/// <typeparam name="TContext">The type of the context used to carry intermediate data.</typeparam>
/// <typeparam name="TSuccess">The type of the success result.</typeparam>
/// <typeparam name="TError">The type of the error result.</typeparam>
public sealed class WorkflowBuilder<TRequest, TContext, TSuccess, TError>
{
	private readonly Func<TRequest, TContext> _contextFactory;
	private readonly Func<TContext, TSuccess> _resultSelector;
	private readonly List<Func<TContext, CancellationToken, Task<Either<TError, TContext>>>> _steps;

	/// <summary>
	/// Initializes a new instance of the <see cref="WorkflowBuilder{TRequest, TContext, TSuccess, TError}"/> class.
	/// </summary>
	/// <param name="contextFactory">
	/// Factory function that takes a request of type <typeparamref name="TRequest"/>
	/// and produces a context of type <typeparamref name="TContext"/>.
	/// </param>
	/// <param name="resultSelector">
	/// Selector function that converts the final <typeparamref name="TContext"/>
	/// into a success result of type <typeparamref name="TSuccess"/>.
	/// </param>
	public WorkflowBuilder(
		Func<TRequest, TContext> contextFactory,
		Func<TContext, TSuccess> resultSelector)
	{
		_contextFactory = contextFactory;
		_resultSelector = resultSelector;
		_steps = [];
	}

	/// <summary>
	/// Adds an asynchronous step to the workflow.
	/// Each step receives the current context and a <see cref="CancellationToken"/>.
	/// It returns either an updated context or an error.
	/// </summary>
	/// <param name="step">
	/// A function that transforms the context into an <see cref="Either{TError, TContext}"/>
	/// asynchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TContext, TSuccess, TError}"/>
	/// instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TContext, TSuccess, TError> UseStep(
		Func<TContext, CancellationToken, Task<Either<TError, TContext>>> step)
	{
		_steps.Add(step);
		return this;
	}

	/// <summary>
	/// Adds a synchronous step to the workflow for convenience.
	/// </summary>
	/// <param name="step">
	/// A function that synchronously transforms the context into
	/// an <see cref="Either{TError, TContext}"/>.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TContext, TSuccess, TError}"/>
	/// instance for fluent chaining.
	/// </returns>
	/// <remarks>
	/// Uses <see cref="Task.FromResult{TResult}(TResult)"/> to avoid warnings 
	/// about lack of 'await' in an <c>async</c> method.
	/// </remarks>
	public WorkflowBuilder<TRequest, TContext, TSuccess, TError> UseStep(Func<TContext, Either<TError, TContext>> step)
	{
		_steps.Add(
			(
				ctx,
				_) => Task.FromResult(step(ctx))
		);
		return this;
	}

	/// <summary>
	/// Executes the workflow by creating the initial context from the request,
	/// running each step in sequence, and finally converting the resulting
	/// context to a success value.
	/// </summary>
	/// <param name="request">The request object of type <typeparamref name="TRequest"/>.</param>
	/// <param name="token">A <see cref="CancellationToken"/> for optional cancellation.</param>
	/// <returns>
	/// An <see cref="Either{TError, TSuccess}"/> containing either an error
	/// or the success result.
	/// </returns>
	public async Task<Either<TError, TSuccess>> RunAsync(
		TRequest request,
		CancellationToken token)
	{
		var ctx = _contextFactory(request)
		          ?? throw new ArgumentNullException(nameof(request), "Context factory returned null.");

		foreach (var step in _steps)
		{
			var result = await step(ctx, token).ConfigureAwait(false);

			if (result.IsLeft)
			{
				return result.Left
				       ?? throw new ArgumentNullException(nameof(result), "Step returned a null TError.");
			}

			ctx = result.Right
			      ?? throw new ArgumentNullException(nameof(result), "Step returned a null TContext.");
		}

		return _resultSelector(ctx);
	}
}