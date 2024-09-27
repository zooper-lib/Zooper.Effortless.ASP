namespace ZEA.Techniques.StrongTypes.Interfaces;

/// <summary>
/// A base class for strong types encapsulating a value of type <typeparamref name="TValue"/>.
/// This version supports comparisons. The encapsulated value must implement <see cref="IComparable{TValue}"/> and <see cref="IEquatable{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the encapsulated value, must implement <see cref="IComparable{TValue}"/> and <see cref="IEquatable{TValue}"/>.</typeparam>
/// <typeparam name="T">The type of the derived class itself, used for enforcing the strong type pattern.</typeparam>
public abstract class StrongTypeComparableClass<TValue, T>(TValue value) : StrongTypeClass<TValue, T>(value)
	where TValue : IComparable<TValue>, IEquatable<TValue>
	where T : StrongTypeComparableClass<TValue, T>, new()
{
	/// <summary>
	/// Determines whether the left strong type is less than the right strong type by comparing their values.
	/// </summary>
	public static bool operator <(
		StrongTypeComparableClass<TValue, T>? left,
		StrongTypeComparableClass<TValue, T>? right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		return left.Value.CompareTo(right.Value) < 0;
	}

	/// <summary>
	/// Determines whether the left strong type is greater than the right strong type by comparing their values.
	/// </summary>
	public static bool operator >(
		StrongTypeComparableClass<TValue, T>? left,
		StrongTypeComparableClass<TValue, T>? right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		return left.Value.CompareTo(right.Value) > 0;
	}

	/// <summary>
	/// Determines whether the left strong type is less than or equal to the right strong type by comparing their values.
	/// </summary>
	public static bool operator <=(
		StrongTypeComparableClass<TValue, T>? left,
		StrongTypeComparableClass<TValue, T>? right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		return left.Value.CompareTo(right.Value) <= 0;
	}

	/// <summary>
	/// Determines whether the left strong type is greater than or equal to the right strong type by comparing their values.
	/// </summary>
	public static bool operator >=(
		StrongTypeComparableClass<TValue, T>? left,
		StrongTypeComparableClass<TValue, T>? right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		return left.Value.CompareTo(right.Value) >= 0;
	}
}