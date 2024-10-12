using ZEA.Communications.Messaging.MassTransit.Builders;
using RabbitMqBuilder = ZEA.Communications.Messaging.MassTransit.RabbitMq.Builders.RabbitMqBuilder;

namespace ZEA.Communications.Messaging.MassTransit.RabbitMq.Extensions;

/// <summary>
/// Extension methods for configuring MassTransit with RabbitMQ.
/// </summary>
public static class RabbitMqExtensions
{
	/// <summary>
	/// Configures MassTransit to use RabbitMQ.
	/// </summary>
	/// <param name="builder">The MassTransit builder.</param>
	/// <param name="host">The RabbitMQ host address.</param>
	/// <param name="username">The RabbitMQ username.</param>
	/// <param name="password">The RabbitMQ password.</param>
	/// <param name="configure">An optional action to configure the RabbitMqBuilder.</param>
	/// <returns>The MassTransit builder.</returns>
	public static MassTransitBuilder ConfigureRabbitMq(
		this MassTransitBuilder builder,
		string host,
		string username,
		string password,
		Action<RabbitMqBuilder>? configure = null)
	{
		var rabbitMqBuilder = new RabbitMqBuilder(host, username, password);
		configure?.Invoke(rabbitMqBuilder);
		builder.TransportBuilder = rabbitMqBuilder;
		return builder;
	}
}