using System;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MassTransitConsumerAttribute(
	string entityName,
	string subscriptionName) : Attribute
{
	public string EntityName { get; } = entityName;
	public string SubscriptionName { get; } = subscriptionName;
}