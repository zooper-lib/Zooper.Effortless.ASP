using ZEA.Communications.Messaging.MassTransit.Builders;
using AzureServiceBusBuilder = ZEA.Communications.Messaging.MassTransit.AzureServiceBus.Builders.AzureServiceBusBuilder;

namespace ZEA.Communications.Messaging.MassTransit.AzureServiceBus.Extensions;

/// <summary>
/// Extension methods for configuring MassTransit with Azure Service Bus.
/// </summary>
public static class AzureServiceBusExtensions
{
	/// <summary>
	/// Configures MassTransit to use Azure Service Bus.
	/// </summary>
	/// <param name="builder">The MassTransit builder.</param>
	/// <param name="connectionString">The Azure Service Bus connection string.</param>
	/// <param name="configure">An optional action to configure the AzureServiceBusBuilder.</param>
	/// <returns>The MassTransit builder.</returns>
	public static MassTransitBuilder UseAzureServiceBus(
		this MassTransitBuilder builder,
		string connectionString,
		Action<AzureServiceBusBuilder>? configure = null)
	{
		var azureServiceBusBuilder = new AzureServiceBusBuilder(connectionString);
		configure?.Invoke(azureServiceBusBuilder);
		builder.TransportBuilder = azureServiceBusBuilder;
		return builder;
	}
}