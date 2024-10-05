namespace ZEA.Architectures.DDD.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DomainEventNameAttribute(
	string eventName,
	int version = -1) : Attribute
{
	public string EventName { get; } = eventName;
	public int Version { get; } = version;
}