namespace ZEA.Communications.Messaging.MassTransit.Attributes;

/// <summary>
/// Specifies the messaging channel associated with a message class.
/// This attribute is used to define the channel name (e.g., topic, exchange, queue)
/// for messages in a messaging system, allowing for consistent and centralized configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ChannelAttribute(string channelName) : Attribute
{
	/// <summary>
	/// Gets the name of the channel associated with the message.
	/// </summary>
	public string ChannelName { get; } = channelName;
}