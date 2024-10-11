using System.Text.Json;
using MassTransit;
using Newtonsoft.Json;
using ZEA.Communications.Messaging.MassTransit.Builders;
using ZEA.Communications.Messaging.MassTransit.Extensions;
using ZEA.Communications.Messaging.MassTransit.RabbitMq.Observers;

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

	// RabbitMQ-specific dead-lettering settings
	private string? _deadLetterExchange;
	private string? _deadLetterRoutingKey;

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

	/// <inheritdoc/>
	public ITransportBuilder UseMessageRetry(Action<IRetryConfigurator> configureRetry)
	{
		_retryConfigurator = configureRetry;
		return this;
	}

	/// <summary>
	/// Configures dead-lettering by setting the dead-letter exchange and routing key.
	/// </summary>
	/// <param name="deadLetterExchange">The dead-letter exchange name.</param>
	/// <param name="deadLetterRoutingKey">The dead-letter routing key.</param>
	/// <returns>The current builder instance.</returns>
	public RabbitMqBuilder ConfigureDeadLettering(
		string deadLetterExchange,
		string deadLetterRoutingKey)
	{
		_deadLetterExchange = deadLetterExchange;
		_deadLetterRoutingKey = deadLetterRoutingKey;
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

				// Apply message retry policy
				if (_retryConfigurator != null)
				{
					cfg.UseMessageRetry(_retryConfigurator);
				}

				// Apply dead-lettering configuration via endpoint observer
				if (!string.IsNullOrEmpty(_deadLetterExchange))
				{
					cfg.ConnectEndpointConfigurationObserver(
						new DeadLetterEndpointConfigurationObserver(_deadLetterExchange, _deadLetterRoutingKey)
					);
				}

				// Apply additional configurations
				_configureBus?.Invoke(cfg, context);

				// Configure endpoints
				cfg.ConfigureEndpoints(context);
			}
		);
	}
}