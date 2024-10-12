namespace ZEA.Communications.Messaging.MassTransit.Attributes;

[AttributeUsage(
	AttributeTargets.Class,
	Inherited = false
)]
public sealed class QueueNameAttribute(string queueName) : Attribute
{
	public string QueueName { get; } = queueName;
}