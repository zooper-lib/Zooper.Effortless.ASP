namespace ZEA.Communications.Messaging.MassTransit.Attributes;

/// <summary>
/// Attribute to specify the subscription name for a consumer.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
// ReSharper disable once ClassNeverInstantiated.Global
public class ConsumerSubscriptionAttribute(string? subscriptionName = null) : Attribute
{
	public string? SubscriptionName { get; } = subscriptionName;
}