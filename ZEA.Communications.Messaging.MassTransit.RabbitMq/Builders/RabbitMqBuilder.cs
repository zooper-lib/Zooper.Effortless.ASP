using MassTransit;
using ZEA.Communications.Messaging.MassTransit.Interfaces;
using ZEA.Communications.Messaging.MassTransit.RabbitMq.Observers;

namespace ZEA.Communications.Messaging.MassTransit.RabbitMq.Builders;

/// <summary>
/// A builder class for configuring MassTransit with RabbitMQ.
/// </summary>
public class RabbitMqBuilder : TransportBuilderBase<IRabbitMqBusFactoryConfigurator>
{
	// RabbitMQ-specific dead-lettering settings
	private string? _deadLetterExchange;
	private string? _deadLetterRoutingKey;
	private readonly string _host;
	private readonly string _username;
	private readonly string _password;

	/// <summary>
	/// Creates a new instance of the <see cref="RabbitMqBuilder"/> class.
	/// <param name="host">The RabbitMQ host address.</param>
	/// <param name="username">The RabbitMQ username.</param>
	/// <param name="password">The RabbitMQ password.</param>
	/// </summary>
	public RabbitMqBuilder(
		string host,
		string username,
		string password)
	{
		if (string.IsNullOrEmpty(host)) throw new ArgumentException("Host cannot be null or empty.", nameof(host));
		if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username cannot be null or empty.", nameof(username));
		if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty.", nameof(password));

		_host = host;
		_username = username;
		_password = password;
	}

	/// <summary>
	/// Configures dead-lettering by setting the dead-letter exchange and routing key.
	/// </summary>
	/// <param name="deadLetterExchange">The dead-letter exchange name.</param>
	/// <param name="deadLetterRoutingKey">The dead-letter routing key.</param>
	/// <returns>The current builder instance.</returns>
	public RabbitMqBuilder ConfigureDeadLettering(
		string deadLetterExchange,
		string? deadLetterRoutingKey = null)
	{
		_deadLetterExchange = deadLetterExchange;
		_deadLetterRoutingKey = deadLetterRoutingKey;
		return this;
	}

	public RabbitMqBuilder ConfigureBus(Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		ConfigureBusFunction = configure;
		return this;
	}

	/// <inheritdoc/>
	public override void ConfigureTransport(IBusRegistrationConfigurator configurator)
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

				ApplyCommonConfigurations(cfg);

				// Apply RabbitMQ-specific configurations
				if (!string.IsNullOrEmpty(_deadLetterExchange))
				{
					cfg.ConnectEndpointConfigurationObserver(
						new DeadLetterEndpointConfigurationObserver(_deadLetterExchange, _deadLetterRoutingKey)
					);
				}

				ConfigureBusFunction?.Invoke(cfg, context);

				// Configure endpoints
				cfg.ConfigureEndpoints(context);
			}
		);
	}
}