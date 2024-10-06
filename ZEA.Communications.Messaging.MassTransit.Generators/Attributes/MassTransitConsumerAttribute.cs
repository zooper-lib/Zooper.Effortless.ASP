using System;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MassTransitConsumerAttribute(
	string entityName,
	string subscriptionName,
	string queueName) : Attribute
{
	public string EntityName { get; } = entityName;
	public string SubscriptionName { get; } = subscriptionName;
	public string QueueName { get; } = queueName;
}