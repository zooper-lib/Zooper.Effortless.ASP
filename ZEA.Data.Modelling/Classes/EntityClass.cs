// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace ZEA.Data.Modelling.Classes;

/// <summary>
///     An Entity (or Entity Object) is a business object that has a unique identity.
///     Its identity allows it to remain distinct from other Entities even if all its
///     attributes are otherwise identical.
/// </summary>
public abstract class EntityClass<TId> : IEquatable<EntityClass<TId>> where TId : notnull
{
	public TId Id { get; init; }

#pragma warning disable CS8618
	protected EntityClass() { }
#pragma warning restore CS8618

	protected EntityClass(TId id)
	{
		Id = id;
	}

	public bool Equals(EntityClass<TId>? other)
	{
		return Equals((object?)other);
	}

	public override bool Equals(object? obj)
	{
		return obj is EntityClass<TId> entity && Id.Equals(entity.Id);
	}

	public static bool operator ==(
		EntityClass<TId> left,
		EntityClass<TId> right)
	{
		return Equals(
			left,
			right
		);
	}

	public static bool operator !=(
		EntityClass<TId> left,
		EntityClass<TId> right)
	{
		return !Equals(
			left,
			right
		);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}