namespace ZEA.Architectures.DDD.Modelling.Records;

public abstract record AggregateRootRecord<TId>(TId Id) : EntityRecord<TId>(Id) where TId : notnull;