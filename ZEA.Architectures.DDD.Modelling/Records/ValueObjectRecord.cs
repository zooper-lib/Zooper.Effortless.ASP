using FluentValidation;

namespace ZEA.Architectures.DDD.Modelling.Records;

public abstract record ValueObjectRecord
{
	// ReSharper disable once UnusedMember.Global
	public void Validate()
	{
		var context = new ValidationContext<ValueObjectRecord>(this);
		var validator = GetValidator();
		var validationResult = validator?.Validate(context);

		if (validationResult is { IsValid: false })
		{
			throw new ValidationException(validationResult.Errors);
		}
	}

	// ReSharper disable once VirtualMemberNeverOverridden.Global
	protected virtual IValidator? GetValidator() => null;

	protected abstract IEnumerable<object?> GetEqualityComponents();

	public override int GetHashCode()
	{
		return GetEqualityComponents()
			.Select(x => x?.GetHashCode() ?? 0)
			.Aggregate((x, y) => x ^ y);
	}
}