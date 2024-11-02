using MediatR;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Pipeline behavior that executes pre-processing steps before the request is handled.
/// It uses `Either` to represent the outcome of each pre-processor.
/// If any pre-processor returns an error, the pipeline short-circuits and returns that error directly as the response.
/// </summary>
/// <typeparam name="TRequest">The type of the request message.</typeparam>
/// <typeparam name="TSuccessDto">The type of the success DTO.</typeparam>
/// <typeparam name="TError">The type representing the error.</typeparam>
public class PreProcessingBehavior<TRequest, TSuccessDto, TError>(
	IEnumerable<IPreProcessor<TRequest, TError>> preProcessors)
	: IPipelineBehavior<TRequest, Either<TSuccessDto, TError>>
	where TRequest : IRequest<Either<TSuccessDto, TError>>
{
	public async Task<Either<TSuccessDto, TError>> Handle(
		TRequest request,
		RequestHandlerDelegate<Either<TSuccessDto, TError>> next,
		CancellationToken cancellationToken)
	{
		foreach (var preProcessor in preProcessors)
		{
			var result = await preProcessor.ProcessAsync(request, cancellationToken);

			if (result.IsRight)
			{
				// Pre-processing failed, return the error response directly
				return Either<TSuccessDto, TError>.FromRight(result.Right!);
			}
		}

		// All pre-processing succeeded, proceed to the next behavior or handler
		return await next();
	}
}