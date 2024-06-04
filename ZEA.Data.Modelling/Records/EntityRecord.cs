namespace ZEA.Data.Modelling.Records;

public abstract record EntityRecord<TId>(TId Id) where TId : notnull;