// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global

namespace Zooper.Effortless.ASP.Data.Modelling;

/// <summary>
///     The Aggregate Root is the primary entity through which we interact with the Aggregate.
///     It's responsible for enforcing the invariants (rules) of the Aggregate and encapsulates
///     access to its members.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
	protected AggregateRoot() { }

	protected AggregateRoot(TId id) : base(id) { }
}