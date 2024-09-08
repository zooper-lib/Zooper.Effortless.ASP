namespace ZEA.Validation.Abstractions.Interfaces;

public interface IValidator<in T>
{
	IEnumerable<string> Validate(T instance);
}