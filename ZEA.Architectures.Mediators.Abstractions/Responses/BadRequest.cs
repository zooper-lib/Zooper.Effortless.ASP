namespace ZEA.Architectures.Mediators.Abstractions.Responses;

[Obsolete("Use a type of ZEA.Techniques.ADTs.Errors.LogicalErrors instead.")]
public record BadRequest(IEnumerable<ErrorDetails> Errors)
{
	public BadRequest(params ErrorDetails[] errors) : this(errors.AsEnumerable()) { }
}