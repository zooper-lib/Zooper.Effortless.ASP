namespace ZEA.Data.Modelling;

public abstract class Id<T>(T value) : ValueObject
{
	private T Value { get; } = value;

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}

	// Implicit conversion to T
	public static implicit operator T(Id<T> id)
	{
		return id.Value;
	}
}