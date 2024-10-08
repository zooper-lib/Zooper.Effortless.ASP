namespace ZEA.Communications.Messaging.MassTransit.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsumerAttribute(
	string entityName,
	string subscriptionName) : Attribute
{
	public string EntityName { get; } = entityName;
	public string SubscriptionName { get; } = subscriptionName;
}