// ReSharper disable UnusedType.Global

namespace Zooper.Effortless.ASP.Data.Modelling;

/// <summary>
///     A Value Object is an immutable object that contains attributes but has no conceptual identity.
///     They are often used to represent descriptors, like quantities, dates, or money.
///     Two Value Objects with the same properties can be considered equal.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
	public bool Equals(ValueObject? other)
	{
		return Equals((object?)other);
	}

	protected abstract IEnumerable<object> GetEqualityComponents();

	public override bool Equals(object? obj)
	{
		if (obj is null || obj.GetType() != GetType()) return false;

		var valueObject = (ValueObject)obj;

		return GetEqualityComponents()
			.SequenceEqual(valueObject.GetEqualityComponents());
	}

	public override int GetHashCode()
	{
		return GetEqualityComponents()
			.Select(x => x?.GetHashCode() ?? 0)
			.Aggregate(
				(
					x,
					y) => x ^ y
			);
	}

	public static bool operator ==(
		ValueObject a,
		ValueObject b)
	{
		return Equals(
			a,
			b
		);
	}

	public static bool operator !=(
		ValueObject a,
		ValueObject b)
	{
		return !Equals(
			a,
			b
		);
	}
}