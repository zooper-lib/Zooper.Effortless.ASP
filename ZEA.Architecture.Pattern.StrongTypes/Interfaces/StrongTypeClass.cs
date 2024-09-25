namespace ZEA.Architecture.Pattern.StrongTypes.Interfaces;

/// <summary>
/// A base class for strong types encapsulating a value of type <typeparamref name="TValue"/>.
/// This version does not support comparisons. The encapsulated value must implement <see cref="IEquatable{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the encapsulated value, must implement <see cref="IEquatable{TValue}"/>.</typeparam>
/// <typeparam name="T">The type of the derived class itself, used for enforcing the strong type pattern.</typeparam>
public abstract class StrongTypeClass<TValue, T>(TValue value)
	where TValue : IEquatable<TValue>
	where T : StrongTypeClass<TValue, T>
{
	/// <summary>
	/// Gets the encapsulated value.
	/// </summary>
	// ReSharper disable once MemberCanBeProtected.Global
	public TValue Value { get; } = value;

	/// <summary>
	/// Creates a new instance of the strong type by invoking the provided constructor.
	/// </summary>
	/// <param name="value">The value to encapsulate in the strong type.</param>
	/// <param name="constructor">A delegate that constructs the specific type <typeparamref name="T"/>.</param>
	/// <returns>A new instance of <typeparamref name="T"/> encapsulating the provided <paramref name="value"/>.</returns>
	// ReSharper disable once UnusedMember.Global
	protected static T Create(
		TValue value,
		Func<TValue, T> constructor)
	{
		return constructor(value);
	}

	/// <inheritdoc />
	public override bool Equals(object? obj)
	{
		if (obj is StrongTypeClass<TValue, T> other)
		{
			return Value.Equals(other.Value);
		}

		return false;
	}

	/// <inheritdoc />
	public override int GetHashCode() => Value.GetHashCode();

	/// <inheritdoc />
	public override string ToString() => Value.ToString() ?? string.Empty;

	/// <summary>
	/// Determines whether two strong types are equal by comparing their values.
	/// </summary>
	public static bool operator ==(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right)
	{
		if (left is null && right is null) return true;
		if (left is null || right is null) return false;

		return left.Value.Equals(right.Value);
	}

	/// <summary>
	/// Determines whether two strong types are not equal by comparing their values.
	/// </summary>
	public static bool operator !=(
		StrongTypeClass<TValue, T>? left,
		StrongTypeClass<TValue, T>? right) => !(left == right);
}