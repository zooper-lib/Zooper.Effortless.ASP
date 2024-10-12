using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.Abstractions;
using ZEA.Communications.Messaging.MassTransit.Implementations;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

public class MassTransitBuilder(IServiceCollection services)
{
	private readonly List<Action<IBusRegistrationConfigurator>> _consumerConfigurations = [];

	/// <summary>
	/// Gets or sets the transport builder to use for configuring MassTransit.
	/// </summary>
	public ITransportBuilder? TransportBuilder { get; set; }

	[Obsolete("This method is obsolete. Use the AzureServiceBusExtensions.ConfigureAzureServiceBus extension method instead.")]
	public MassTransitBuilder UseAzureServiceBus(
		string connectionString,
		Action<AzureServiceBusBuilder> configure)
	{
		var azureServiceBusBuilder = new AzureServiceBusBuilder(connectionString);
		configure(azureServiceBusBuilder);
		TransportBuilder = azureServiceBusBuilder;
		return this;
	}

	[Obsolete("This method is obsolete. Use the RabbitMqExtensions.ConfigureRabbitMq extension method instead.")]
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
	/// Adds consumers using a custom configuration action.
	/// </summary>
	/// <param name="configureConsumers">An action to configure consumers.</param>
	/// <returns>The current builder instance.</returns>
	public MassTransitBuilder AddConsumers(Action<IBusRegistrationConfigurator> configureConsumers)
	{
		_consumerConfigurations.Add(configureConsumers);
		return this;
	}

	/// <summary>
	/// Adds consumer assemblies to scan for consumers.
	/// </summary>
	/// <param name="consumerAssemblies">Assemblies containing consumers.</param>
	/// <returns>The current builder instance.</returns>
	public MassTransitBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		_consumerConfigurations.Add(cfg => cfg.AddConsumers(consumerAssemblies));
		return this;
	}

	/// <summary>
	/// Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	public void Build()
	{
		services.AddMassTransit(configurator =>
		{
			// Register consumers
			foreach (var configureConsumers in _consumerConfigurations)
			{
				configureConsumers(configurator);
			}

			// Configure transport-specific settings
			TransportBuilder?.ConfigureTransport(configurator);
		});
	}
}