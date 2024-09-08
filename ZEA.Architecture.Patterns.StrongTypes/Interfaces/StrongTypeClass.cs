namespace ZEA.Architecture.Patterns.StrongTypes.Interfaces;

public abstract class StrongTypeClass<TValue, T>(TValue value)
	where TValue : struct, IComparable<TValue>, IEquatable<TValue>
	where T : StrongTypeClass<TValue, T>, new()
{
	// ReSharper disable once MemberCanBePrivate.Global
	public TValue Value { get; init; } = value;

	// Factory method that calls validation if available
	public static T Create(TValue value)
	{
		return new T
		{
			Value = value
		};
	}

	public override bool Equals(object? obj)
	{
		if (obj is StrongTypeClass<TValue, T> other)
		{
			return Value.Equals(other.Value);
		}

		return false;
	}

	public override int GetHashCode() => Value.GetHashCode();

	public override string ToString() => Value.ToString() ?? string.Empty;

	public static bool operator ==(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right)
	{
		if (left is null && right is null) return true;
		if (left is null || right is null) return false;

		return left.Value.Equals(right.Value);
	}

	public static bool operator !=(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right) => !(left == right);

	public static bool operator <(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right)
	{
		if (left is null || right is null) throw new ArgumentNullException();

		return left.Value.CompareTo(right.Value) < 0;
	}

	public static bool operator >(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right)
	{
		if (left is null || right is null) throw new ArgumentNullException();

		return left.Value.CompareTo(right.Value) > 0;
	}
}