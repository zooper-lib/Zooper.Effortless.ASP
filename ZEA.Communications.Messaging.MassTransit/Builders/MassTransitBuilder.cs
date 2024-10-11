using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.Abstractions;
using ZEA.Communications.Messaging.MassTransit.Implementations;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

public class MassTransitBuilder(IServiceCollection services)
{
	/// <summary>
	/// Gets or sets the transport builder to use for configuring MassTransit.
	/// </summary>
	public ITransportBuilder? TransportBuilder { get; set; }

	[Obsolete("This method is obsolete. Use the ITTransportBuilder interface instead.")]
	public MassTransitBuilder UseAzureServiceBus(
		string connectionString,
		Action<AzureServiceBusBuilder> configure)
	{
		var azureServiceBusBuilder = new AzureServiceBusBuilder(connectionString);
		configure(azureServiceBusBuilder);
		TransportBuilder = azureServiceBusBuilder;
		return this;
	}

	[Obsolete("This method is obsolete. Use the ITTransportBuilder interface instead.")]
	public MassTransitBuilder UseRabbitMq(
		string host,
		string username,
		string password,
		Action<RabbitMqBuilder> configure)
	{
		var rabbitMqBuilder = new RabbitMqBuilder(
			host,
			username,
			password
		);
		configure(rabbitMqBuilder);
		TransportBuilder = rabbitMqBuilder;
		return this;
	}

	/// <summary>
	/// Adds a message publisher to the service collection.
	/// </summary>
	/// <returns>The current builder instance.</returns>
	public MassTransitBuilder AddPublisher()
	{
		services.AddSingleton<IEventPublisher, MassTransitEventPublisher>();
		services.AddSingleton<IMessagePublisher, MassTransitMessagePublisher>();
		return this;
	}

	/// <summary>
	/// Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	public void Build()
	{
		TransportBuilder?.Build(services);
	}
}