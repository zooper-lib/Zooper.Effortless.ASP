namespace ZEA.Communication.Messaging.MassTransit.Attributes;

[AttributeUsage(
	AttributeTargets.Class,
	Inherited = false
)]
public sealed class ExchangeNameAttribute(string exchangeName) : Attribute
{
	public string ExchangeName { get; } = exchangeName;
}