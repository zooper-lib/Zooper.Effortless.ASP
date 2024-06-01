namespace ZEA.Architecture.PubSub.Abstractions.Responses;

public record BadRequest(IEnumerable<ErrorDetails> Errors)
{
	public BadRequest(params ErrorDetails[] errors) : this(errors.AsEnumerable()) { }
}