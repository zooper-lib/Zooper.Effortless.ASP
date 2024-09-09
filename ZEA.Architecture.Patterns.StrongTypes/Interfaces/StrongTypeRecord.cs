namespace ZEA.Architecture.Patterns.StrongTypes.Interfaces;

public abstract record StrongTypeRecord<TValue, T>(TValue Value)
	where TValue : IComparable<TValue>, IEquatable<TValue>
	where T : StrongTypeRecord<TValue, T>
{
	// Factory method to create an instance using a constructor delegate
	protected static T Create(
		TValue value,
		Func<TValue, T> constructor)
	{
		return constructor(value);
	}

	public override string? ToString() => Value.ToString();
}