namespace ZEA.Validation.FluentValidation;

public interface IValidationRule
{
	bool IsValid();

	string Message { get; }
}