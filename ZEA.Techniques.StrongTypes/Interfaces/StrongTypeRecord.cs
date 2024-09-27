namespace ZEA.Techniques.StrongTypes.Interfaces;

/// <summary>
/// A base record for strong types encapsulating a value of type <typeparamref name="TValue"/>.
/// This version does not support comparisons. The encapsulated value must implement <see cref="IEquatable{TValue}"/>.
/// This base record provides basic equality functionality, ensuring that two instances of a strong type are compared based on their encapsulated values.
/// </summary>
/// <typeparam name="TValue">The type of the encapsulated value, must implement <see cref="IEquatable{TValue}"/>.</typeparam>
/// <typeparam name="T">The type of the derived record itself, used for enforcing the strong type pattern in derived records.</typeparam>
public abstract record StrongTypeRecord<TValue, T>(TValue Value)
	where TValue : IEquatable<TValue>
	where T : StrongTypeRecord<TValue, T>
{
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

	/// <summary>
	/// Returns a string representation of the encapsulated value.
	/// </summary>
	/// <returns>The string representation of the value.</returns>
	public override string? ToString() => Value.ToString();
}