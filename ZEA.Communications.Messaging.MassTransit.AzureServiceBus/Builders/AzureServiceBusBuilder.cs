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
	private readonly List<Assembly> _consumerAssemblies = [];

	private bool _excludeBaseInterfaces;

	private Func<JsonSerializerSettings, JsonSerializerSettings>? _newtonsoftJsonConfig;
	private Func<JsonSerializerOptions, JsonSerializerOptions>? _systemTextJsonConfig;

	/// <summary>
	/// Initializes a new instance of the <see cref="AzureServiceBusBuilder"/> class.
	/// </summary>
	/// <param name="connectionString">The Azure Service Bus connection string.</param>
	public AzureServiceBusBuilder(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <inheritdoc/>
	public ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		_consumerAssemblies.AddRange(consumerAssemblies);
		return this;
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

	/// <inheritdoc/>
	public void Build(IServiceCollection services)
	{
		services.AddMassTransit(
			configurator =>
			{
				configurator.AddConsumers(_consumerAssemblies.ToArray());
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
					}
				);
			}
		);
	}
}