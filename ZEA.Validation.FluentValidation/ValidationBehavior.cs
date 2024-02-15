using FluentValidation;
using MediatR;

namespace ZEA.Validation.FluentValidation;

/// <summary>
///     A behavior for MediatR that validates the request.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (!validators.Any()) return await next();

		var context = new ValidationContext<TRequest>(request);
		var errorsDictionary = validators.Select(x => x.Validate(context))
			.SelectMany(x => x.Errors)
			.Where(x => x != null)
			.ToList();

		if (errorsDictionary.Count != 0) throw new ValidationException(errorsDictionary);

		return await next();
	}
}