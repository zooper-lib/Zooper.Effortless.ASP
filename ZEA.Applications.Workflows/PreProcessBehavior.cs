using MediatR;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

public class PreProcessBehavior<TRequest, TResponse, TError> : IPipelineBehavior<TRequest, Either<TResponse, TError>>
	where TRequest : IRequest<Either<TResponse, TError>>, IPreProcessBehavior<TRequest, TError>
{
	public async Task<Either<TResponse, TError>> Handle(
		TRequest request,
		RequestHandlerDelegate<Either<TResponse, TError>> next,
		CancellationToken cancellationToken)
	{
		var preProcessResult = await request.ExecuteAsync(request, cancellationToken);

		if (preProcessResult.IsRight)
		{
			// Pre-processing failed, return the error
			return Either<TResponse, TError>.FromRight(preProcessResult.Right!);
		}

		// Pre-processing succeeded, proceed to the next behavior or handler
		return await next();
	}
}