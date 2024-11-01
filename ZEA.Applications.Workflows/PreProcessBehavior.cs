using MediatR;

namespace ZEA.Applications.Workflows;

public class PreProcessingBehavior<TRequest, TResponse>(IEnumerable<IPreProcessor<TRequest, TResponse>> preProcessors)
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
		}

		// Pre-processing succeeded, proceed to the next behavior or handler
		return await next();
	}
}