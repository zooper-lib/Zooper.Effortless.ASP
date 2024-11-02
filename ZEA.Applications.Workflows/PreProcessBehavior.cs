using MediatR;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Pipeline behavior that executes pre-processing steps before the request is handled.
/// It allows for validation, authorization, or any custom logic to occur.
/// If any pre-processor returns a non-null response, the pipeline short-circuits and returns that response immediately.
/// </summary>
/// <typeparam name="TRequest">The type of the request message.</typeparam>
/// <typeparam name="TResponse">The type of the response message.</typeparam>
public class PreProcessingBehavior<TRequest, TResponse>(
	IEnumerable<IPreProcessor<TRequest, TResponse>> preProcessors)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		foreach (var preProcessor in preProcessors)
		{
			var result = await preProcessor.ProcessAsync(request, cancellationToken);

			if (result != null)
			{
				// Pre-processing failed, return the error response
				return result;
			}
			// Pre-processing succeeded, continue to the next pre-processor
		}

		// All pre-processing succeeded, proceed to the next behavior or handler
		return await next();
	}
}