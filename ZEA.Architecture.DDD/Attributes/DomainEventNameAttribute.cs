namespace ZEA.Architecture.DDD.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DomainEventNameAttribute(string eventName) : Attribute
{
	public string EventName { get; } = eventName;
}