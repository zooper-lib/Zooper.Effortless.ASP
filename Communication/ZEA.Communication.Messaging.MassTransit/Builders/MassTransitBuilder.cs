using Microsoft.Extensions.DependencyInjection;
using ZEA.Communication.Messaging.Abstractions;

namespace ZEA.Communication.Messaging.MassTransit.Builders;

public class MassTransitBuilder(IServiceCollection services)
{
	private ITransportBuilder? _transportBuilder;

	public MassTransitBuilder UseAzureServiceBus(
		string connectionString,
		Action<AzureServiceBusBuilder> configure)
	{
		var azureServiceBusBuilder = new AzureServiceBusBuilder(connectionString);
		configure(azureServiceBusBuilder);
		_transportBuilder = azureServiceBusBuilder;
		return this;
	}

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
		_transportBuilder = rabbitMqBuilder;
		return this;
	}

	/// <summary>
	///     Adds a message publisher to the service collection.
	/// </summary>
	/// <returns>The current builder instance.</returns>
	public MassTransitBuilder AddPublisher()
	{
		services.AddSingleton<IMessagePublisher, MassTransitMessagePublisher>();
		return this;
	}

	/// <summary>
	///     Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	public void Build()
	{
		_transportBuilder?.Build(services);
	}
}