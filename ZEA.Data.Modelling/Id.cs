using ZEA.Data.Modelling.Records;

namespace ZEA.Data.Modelling;

public abstract record Id<T>(T Value) : ValueObjectRecord
{
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