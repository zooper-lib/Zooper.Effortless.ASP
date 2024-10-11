using System.Reflection;
using System.Text.Json;
using MassTransit;
using Newtonsoft.Json;
using ZEA.Communications.Messaging.MassTransit.Builders;
using ZEA.Communications.Messaging.MassTransit.Extensions;

namespace ZEA.Communications.Messaging.MassTransit.RabbitMq.Builders;

/// <summary>
/// A builder class for configuring MassTransit with RabbitMQ.
/// </summary>
public class RabbitMqBuilder : ITransportBuilder
{
	private readonly string _host;
	private readonly string _username;
	private readonly string _password;

	private bool _excludeBaseInterfaces;

	private Func<JsonSerializerSettings, JsonSerializerSettings>? _newtonsoftJsonConfig;
	private Func<JsonSerializerOptions, JsonSerializerOptions>? _systemTextJsonConfig;

	private Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? _configureBus;

	private Action<IRetryConfigurator>? _retryConfigurator;
	private Action<IRedeliveryConfigurator>? _redeliveryConfigurator;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqBuilder"/> class.
	/// </summary>
	/// <param name="host">The RabbitMQ host address.</param>
	/// <param name="username">The RabbitMQ username.</param>
	/// <param name="password">The RabbitMQ password.</param>
	public RabbitMqBuilder(
		string host,
		string username,
		string password)
	{
		_host = host;
		_username = username;
		_password = password;
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
	/// Allows additional configuration of the RabbitMQ bus.
	/// </summary>
	/// <param name="configure">An action to configure the bus factory configurator.</param>
	/// <returns>The current builder instance.</returns>
	public RabbitMqBuilder ConfigureBus(Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		_configureBus = configure;
		return this;
	}

	/// <summary>
	/// Sets a retry configuration.
	/// </summary>
	/// <param name="configureRetry">The retry configuration action.</param>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder UseRetry(Action<IRetryConfigurator> configureRetry)
	{
		_retryConfigurator = configureRetry;
		return this;
	}

	/// <summary>
	/// Sets a redelivery configuration.
	/// </summary>
	/// <param name="configureRedelivery">The redelivery configuration action.</param>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder UseRedelivery(Action<IRedeliveryConfigurator> configureRedelivery)
	{
		_redeliveryConfigurator = configureRedelivery;
		return this;
	}

	/// <inheritdoc/>
	public void ConfigureTransport(IBusRegistrationConfigurator configurator)
	{
		configurator.UsingRabbitMq(
			(
				context,
				cfg) =>
			{
				cfg.Host(
					_host,
					h =>
					{
						h.Username(_username);
						h.Password(_password);
					}
				);

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