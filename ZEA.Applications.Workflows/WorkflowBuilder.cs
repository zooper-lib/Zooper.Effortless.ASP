using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Represents a builder for a workflow that processes a request and
/// either succeeds with a <typeparamref name="TSuccess"/> result
/// or fails with a <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request input.</typeparam>
/// <typeparam name="TPayload">The type of the payload used to carry intermediate data.</typeparam>
/// <typeparam name="TSuccess">The type of the success result.</typeparam>
/// <typeparam name="TError">The type of the error result.</typeparam>
public sealed class WorkflowBuilder<TRequest, TPayload, TSuccess, TError>
{
	private readonly Func<TRequest, TPayload> _contextFactory;
	private readonly Func<TPayload, TSuccess> _resultSelector;

	private readonly List<Func<TRequest, CancellationToken, Task<Either<TError, TRequest>>>> _preSteps;
	private readonly List<Func<TPayload, CancellationToken, Task<Either<TError, TPayload>>>> _steps;

	/// <summary>
	/// Initializes a new instance of the <see cref="WorkflowBuilder{TRequest, TContext, TSuccess, TError}"/> class.
	/// </summary>
	/// <param name="contextFactory">
	/// Factory function that takes a request of type <typeparamref name="TRequest"/>
	/// and produces a context of type <typeparamref name="TPayload"/>.
	/// </param>
	/// <param name="resultSelector">
	/// Selector function that converts the final <typeparamref name="TPayload"/>
	/// into a success result of type <typeparamref name="TSuccess"/>.
	/// </param>
	public WorkflowBuilder(
		Func<TRequest, TPayload> contextFactory,
		Func<TPayload, TSuccess> resultSelector)
	{
		_contextFactory = contextFactory;
		_resultSelector = resultSelector;

		_preSteps = [];
		_steps = [];
	}

	/// <summary>
	/// Adds a single asynchronous pre-step that operates on the raw <typeparamref name="TRequest"/>.
	/// </summary>
	/// <param name="preStep">
	/// A function that receives <typeparamref name="TRequest"/> and returns
	/// either an updated request or an error, asynchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddPreStep(
		Func<TRequest, CancellationToken, Task<Either<TError, TRequest>>> preStep)
	{
		_preSteps.Add(preStep);
		return this;
	}

	/// <summary>
	/// Adds a single synchronous pre-step that operates on the raw <typeparamref name="TRequest"/>.
	/// </summary>
	/// <param name="preStep">
	/// A function that receives <typeparamref name="TRequest"/> and returns
	/// either an updated request or an error, synchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddPreStep(Func<TRequest, Either<TError, TRequest>> preStep)
	{
		_preSteps.Add(
			(
				req,
				_) => Task.FromResult(preStep(req))
		);
		return this;
	}

	/// <summary>
	/// Adds multiple asynchronous pre-steps that operate on the raw <typeparamref name="TRequest"/>.
	/// </summary>
	/// <param name="preSteps">
	/// One or more functions that each take <typeparamref name="TRequest"/> and return
	/// either an updated request or an error, asynchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddPreSteps(
		params Func<TRequest, CancellationToken, Task<Either<TError, TRequest>>>[] preSteps)
	{
		_preSteps.AddRange(preSteps);
		return this;
	}

	/// <summary>
	/// Adds multiple synchronous pre-steps that operate on the raw <typeparamref name="TRequest"/>.
	/// </summary>
	/// <param name="preSteps">
	/// One or more functions that each take <typeparamref name="TRequest"/> and return
	/// either an updated request or an error, synchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddPreSteps(params Func<TRequest, Either<TError, TRequest>>[] preSteps)
	{
		foreach (var step in preSteps)
		{
			_preSteps.Add(
				(
					req,
					_) => Task.FromResult(step(req))
			);
		}

		return this;
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
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> UseStep(
		Func<TPayload, CancellationToken, Task<Either<TError, TPayload>>> step)
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
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> UseStep(Func<TPayload, Either<TError, TPayload>> step)
	{
		_steps.Add(
			(
				ctx,
				_) => Task.FromResult(step(ctx))
		);
		return this;
	}

	/// <summary>
	/// Adds multiple asynchronous steps to the workflow, each operating on <typeparamref name="TPayload"/>.
	/// </summary>
	/// <param name="steps">
	/// One or more functions that each transform the payload into an <see cref="Either{TError, TPayload}"/>,
	/// asynchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddSteps(
		params Func<TPayload, CancellationToken, Task<Either<TError, TPayload>>>[] steps)
	{
		_steps.AddRange(steps);
		return this;
	}

	/// <summary>
	/// Adds multiple synchronous steps to the workflow, each operating on <typeparamref name="TPayload"/>.
	/// </summary>
	/// <param name="steps">
	/// One or more functions that each transform the payload into an <see cref="Either{TError, TPayload}"/>,
	/// synchronously.
	/// </param>
	/// <returns>
	/// The current <see cref="WorkflowBuilder{TRequest, TPayload, TSuccess, TError}"/> instance for fluent chaining.
	/// </returns>
	public WorkflowBuilder<TRequest, TPayload, TSuccess, TError> AddSteps(params Func<TPayload, Either<TError, TPayload>>[] steps)
	{
		foreach (var step in steps)
		{
			_steps.Add(
				(
					ctx,
					_) => Task.FromResult(step(ctx))
			);
		}

		return this;
	}

	/// <summary>
	/// Executes the workflow by creating the initial payload from the request,
	/// running each pre-step in sequence on the request, then
	/// running each normal step in sequence on the payload,
	/// and finally converting the resulting payload to a success value.
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
		// Run pre-steps on the raw TRequest
		foreach (var preStep in _preSteps)
		{
			var preResult = await preStep(request, token).ConfigureAwait(false);

			if (preResult.IsLeft)
				return preResult.Left ?? throw new ArgumentNullException(nameof(preResult), "Pre-Step returned a null TError.");

			request = preResult.Right ?? throw new ArgumentNullException(nameof(preResult), "Pre-Step returned a null TRequest.");
		}

		// Create the initial context
		var ctx = _contextFactory(request)
		          ?? throw new ArgumentNullException(nameof(request), "Context factory returned null.");

		// Run each step in sequence
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

		// Convert the final context to a success result
		return _resultSelector(ctx);
	}
}