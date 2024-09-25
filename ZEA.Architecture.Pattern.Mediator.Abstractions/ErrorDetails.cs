namespace ZEA.Architecture.Pattern.Mediator.Abstractions;

[Obsolete("Use a type of ZEA.Architecture.Pattern.ADTs.Errors.LogicalErrors instead.")]
public sealed record ErrorDetails(
	string? Code,
	string? Message);