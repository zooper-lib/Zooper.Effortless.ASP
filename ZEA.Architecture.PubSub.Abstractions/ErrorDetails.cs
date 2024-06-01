namespace ZEA.Architecture.PubSub.Abstractions;

public sealed record ErrorDetails(
	string? Code,
	string? Message);