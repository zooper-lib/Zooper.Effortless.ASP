// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global

namespace ZEA.Architectures.DDD.Modelling.Classes;

/// <summary>
///     The Aggregate Root is the primary entity through which we interact with the Aggregate.
///     It's responsible for enforcing the invariants (rules) of the Aggregate and encapsulates
///     access to its members.
/// </summary>
public abstract class 
	AggregateRootClass<TId> : EntityClass<TId> where TId : notnull
{
	protected AggregateRootClass() { }

	protected AggregateRootClass(TId id) : base(id) { }
}

