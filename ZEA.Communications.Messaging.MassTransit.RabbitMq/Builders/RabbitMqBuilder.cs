using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.MassTransit.Attributes;
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
	private readonly List<Assembly> _consumerAssemblies = [];
	private bool excludeBaseInterfaces = false;
	private readonly List<Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>> _endpointConfigurations = [];
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
	public ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		if (consumerAssemblies == null || consumerAssemblies.Length == 0)
			throw new ArgumentNullException(nameof(consumerAssemblies));

		_consumerAssemblies.AddRange(consumerAssemblies);
		return this;
	}

	/// <inheritdoc/>
	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		excludeBaseInterfaces = exclude;
		return this;
	}

	/// <inheritdoc/>
	public void Build(IServiceCollection services)
	{
		services.AddMassTransit(
			configurator =>
			{
				configurator.AddConsumers(_consumerAssemblies.ToArray());
				configurator.UsingRabbitMq(
					(
						context,
						cfg) =>
					{
						cfg.UseNewtonsoftJsonSerializer();
						cfg.Host(
							_host,
							h =>
							{
								h.Username(_username);
								h.Password(_password);
							}
						);

						// Conditionally exclude base interfaces
						if (excludeBaseInterfaces)
						{
							cfg.ExcludeBaseInterfaces();
						}

						foreach (var endpointConfig in _endpointConfigurations)
							endpointConfig(cfg, context);

						if (_retryConfigurator != null)
						{
							cfg.UseMessageRetry(_retryConfigurator);
						}

						if (_redeliveryConfigurator != null)
						{
							cfg.UseDelayedRedelivery(_redeliveryConfigurator);
						}
					}
				);
			}
		);
	}

	/// <summary>
	/// Adds a configuration action for endpoints.
	/// </summary>
	/// <param name="configure">The configuration action for the endpoints.</param>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder ConfigureEndpoints(Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		_endpointConfigurations.Add(configure);
		return this;
	}

	/// <summary>
	/// Adds endpoints for consumers that are decorated with a <see cref="QueueNameAttribute"/>.
	/// </summary>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder ConfigureEndpointsByAttribute()
	{
		var consumersByQueueName = new Dictionary<string, List<Type>>();

		// Group consumers by queue name
		foreach (var assembly in _consumerAssemblies)
		{
			foreach (var type in assembly.GetTypes())
			{
				var queueNameAttribute = type.GetCustomAttribute<QueueNameAttribute>();
				if (queueNameAttribute == null) continue;

				if (!consumersByQueueName.ContainsKey(queueNameAttribute.QueueName))
					consumersByQueueName[queueNameAttribute.QueueName] = [];

				consumersByQueueName[queueNameAttribute.QueueName].Add(type);
			}
		}

		// Configure each queue with its consumers
		foreach (var kvp in consumersByQueueName)
		{
			_endpointConfigurations.Add(
				(
					cfg,
					ctx) =>
				{
					cfg.ReceiveEndpoint(
						kvp.Key,
						e =>
						{
							foreach (var consumerType in kvp.Value)
								e.ConfigureConsumer(ctx, consumerType);
						}
					);
				}
			);
		}

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
}