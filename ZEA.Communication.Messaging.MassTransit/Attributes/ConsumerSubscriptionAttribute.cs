namespace ZEA.Communication.Messaging.MassTransit.Attributes;

/// <summary>
/// Attribute to specify the subscription name for a consumer.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ConsumerSubscriptionAttribute(string topicName, string subscriptionName) : Attribute
{
	public string TopicName { get; } = topicName;
	public string SubscriptionName { get; } = subscriptionName;
}