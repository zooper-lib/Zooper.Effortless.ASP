using FluentValidation;
using MediatR;

namespace Zooper.Effortless.ASP.Validation.FluentValidation;

/// <summary>
///     A behavior for MediatR that validates the request.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (!_validators.Any()) return await next();

		var context = new ValidationContext<TRequest>(request);
		var errorsDictionary = _validators.Select(x => x.Validate(context))
			.SelectMany(x => x.Errors)
			.Where(x => x != null)
			.ToList();

		if (errorsDictionary.Count != 0) throw new ValidationException(errorsDictionary);

		return await next();
	}
}