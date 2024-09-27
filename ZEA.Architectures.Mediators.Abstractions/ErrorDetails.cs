namespace ZEA.Architectures.Mediators.Abstractions;

[Obsolete("Use a type of ZEA.Techniques.ADTs.Errors.LogicalErrors instead.")]
public sealed record ErrorDetails(
	string? Code,
	string? Message);