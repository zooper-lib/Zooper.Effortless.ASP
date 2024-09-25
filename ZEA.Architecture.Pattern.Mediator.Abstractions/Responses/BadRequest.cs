namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Responses;

[Obsolete("Use a type of ZEA.Architecture.Patterns.ADTs.Errors.LogicalErrors instead.")]
public record BadRequest(IEnumerable<ErrorDetails> Errors)
{
	public BadRequest(params ErrorDetails[] errors) : this(errors.AsEnumerable()) { }
}