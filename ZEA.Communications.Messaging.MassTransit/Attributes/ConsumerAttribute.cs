namespace ZEA.Communications.Messaging.MassTransit.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsumerAttribute(
	string channelName,
	string endpointName) : Attribute
{
	/// <summary>
	/// The name of the messaging channel (e.g., topic, exchange).
	/// </summary>
	public string ChannelName { get; } = channelName;

	/// <summary>
	/// The name of the endpoint (e.g., subscription, queue) where the consumer listens.
	/// </summary>
	public string EndpointName { get; } = endpointName;
}