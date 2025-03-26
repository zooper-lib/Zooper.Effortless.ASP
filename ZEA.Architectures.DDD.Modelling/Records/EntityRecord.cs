// ReSharper disable UnusedMember.Global

namespace ZEA.Architectures.DDD.Modelling.Records;

public abstract record EntityRecord<TId>(TId Id) where TId : notnull
{
	public TId Id { get; protected set; } = Id;
}