namespace ZEA.Architecture.Pattern.StrongTypes.Interfaces;

/// <summary>
/// A derived record for strong types encapsulating a value of type <typeparamref name="TValue"/>.
/// This version supports comparisons. The encapsulated value must implement both <see cref="IComparable{TValue}"/> and <see cref="IEquatable{TValue}"/>.
/// This derived record provides comparison functionality, allowing strong types to be compared based on their values using comparison operators.
/// </summary>
/// <typeparam name="TValue">The type of the encapsulated value, must implement <see cref="IComparable{TValue}"/> and <see cref="IEquatable{TValue}"/>.</typeparam>
/// <typeparam name="T">The type of the derived record itself, used for enforcing the strong type pattern in derived records.</typeparam>
public abstract record StrongTypeComparableRecord<TValue, T>(TValue Value)
	: StrongTypeRecord<TValue, T>(Value)
	where TValue : IComparable<TValue>, IEquatable<TValue>
	where T : StrongTypeComparableRecord<TValue, T>;