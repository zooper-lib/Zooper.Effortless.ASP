namespace ZEA.Communications.Messaging.MassTransit.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TopicAttribute(string topicName) : Attribute
{
	public string TopicName { get; } = topicName;
}