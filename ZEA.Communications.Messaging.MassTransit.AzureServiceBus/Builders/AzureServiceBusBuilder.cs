using System.Reflection;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ZEA.Communications.Messaging.MassTransit.Builders;
using ZEA.Communications.Messaging.MassTransit.Extensions;

namespace ZEA.Communications.Messaging.MassTransit.AzureServiceBus.Builders;

/// <summary>
/// Builder class for configuring MassTransit with Azure Service Bus.
/// </summary>
public class AzureServiceBusBuilder : ITransportBuilder
{
	private readonly string _connectionString;

	private bool _excludeBaseInterfaces;

	private Func<JsonSerializerSettings, JsonSerializerSettings>? _newtonsoftJsonConfig;
	private Func<JsonSerializerOptions, JsonSerializerOptions>? _systemTextJsonConfig;

	private Action<IServiceBusBusFactoryConfigurator, IBusRegistrationContext>? _configureBus;

	/// <summary>
	/// Initializes a new instance of the <see cref="AzureServiceBusBuilder"/> class.
	/// </summary>
	/// <param name="connectionString">The Azure Service Bus connection string.</param>
	public AzureServiceBusBuilder(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <inheritdoc/>
	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		_excludeBaseInterfaces = exclude;
		return this;
	}

	/// <inheritdoc/>
	public ITransportBuilder UseNewtonsoftJson(Func<JsonSerializerSettings, JsonSerializerSettings> configure)
	{
		_newtonsoftJsonConfig = configure;
		return this;
	}

	/// <inheritdoc/>
	public ITransportBuilder UseSystemTextJson(Func<JsonSerializerOptions, JsonSerializerOptions> configure)
	{
		_systemTextJsonConfig = configure;
		return this;
	}

	/// <summary>
	/// Allows additional configuration of the Azure Service Bus.
	/// </summary>
	/// <param name="configure">An action to configure the bus factory configurator.</param>
	/// <returns>The current builder instance.</returns>
	public AzureServiceBusBuilder ConfigureBus(Action<IServiceBusBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		_configureBus = configure;
		return this;
	}

	/// <inheritdoc/>
	public void ConfigureTransport(IBusRegistrationConfigurator configurator)
	{
		configurator.UsingAzureServiceBus(
			(
				context,
				cfg) =>
			{
				cfg.Host(_connectionString);

				// Apply Newtonsoft.Json configuration
				if (_newtonsoftJsonConfig != null)
				{
					cfg.UseNewtonsoftJsonSerializer();
					cfg.ConfigureNewtonsoftJsonSerializer(_newtonsoftJsonConfig);
					cfg.UseNewtonsoftJsonDeserializer();
					cfg.ConfigureNewtonsoftJsonDeserializer(_newtonsoftJsonConfig);
				}

				// Apply System.Text.Json configuration
				if (_systemTextJsonConfig != null)
				{
					cfg.UseJsonSerializer();
					cfg.ConfigureJsonSerializerOptions(_systemTextJsonConfig);
					cfg.UseJsonDeserializer();
				}

				// Conditionally exclude base interfaces
				if (_excludeBaseInterfaces)
				{
					cfg.ExcludeBaseInterfaces();
				}

				// Apply additional configurations
				_configureBus?.Invoke(cfg, context);
			}
		);
	}
}