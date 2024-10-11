using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.MassTransit.Attributes;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

/// <summary>
///     A builder class for configuring MassTransit with RabbitMQ.
/// </summary>
/// <param name="host">The RabbitMQ host address.</param>
/// <param name="username">The RabbitMQ username.</param>
/// <param name="password">The RabbitMQ password.</param>
[Obsolete("Use 'RabbitMqBuilder' in 'ZEA.Communications.Messaging.MassTransit.RabbitMq' instead.")]
public class RabbitMqBuilder(
	string host,
	string username,
	string password) : ITransportBuilder
{
	private readonly List<Assembly> _consumerAssemblies = [];
	private readonly List<Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>> _endpointConfigurations = [];
	private Action<IRetryConfigurator>? _retryConfigurator;
	private Action<IRedeliveryConfigurator>? _redeliveryConfigurator;

	/// <summary>
	///     Adds the specified consumer assemblies to the builder.
	/// </summary>
	/// <param name="consumerAssemblies">The consumer assemblies to add.</param>
	/// <returns>The current builder instance.</returns>
	/// /// <exception cref="ArgumentNullException">Thrown when no consumer assemblies are provided.</exception>
	public ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		// There must at least be one consumer assembly.
		if (consumerAssemblies.Length == 0)
			throw new ArgumentNullException(nameof(consumerAssemblies));

		_consumerAssemblies.AddRange(consumerAssemblies);
		return this;
	}

	/// <inheritdoc/>
	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///     Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	/// /// <param name="services">The service collection to register the configuration with.</param>
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
							host,
							h =>
							{
								h.Username(username);
								h.Password(password);
							}
						);
						foreach (var endpointConfig in _endpointConfigurations)
							endpointConfig(
								cfg,
								context
							);

						// Configure the retry policy
						if (_retryConfigurator != null)
						{
							cfg.UseMessageRetry(_retryConfigurator);
						}

						// Configure the redelivery policy
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
	///     Adds a configuration action for an endpoint.
	/// </summary>
	/// <param name="configure">The configuration action for the endpoint.</param>
	/// <returns>The current builder instance.</returns>
	/// <example>
	///     <code>
	/// 	rabbitMqBuilder.ConfigureEndpoints(
	///  	(cfg, ctx) =&gt;
	///  	{
	///  		cfg.ReceiveEndpoint(
	///  			"account-queue",
	///  			e =&gt;
	///  			{
	///  				// Configure the endpoint, e.g., specify the consumer
	///  				e.Consumer&lt;AccountUpdatedConsumer&gt;();
	///  			}
	///  		);
	///  	}
	///  );
	///  </code>
	/// </example>
	public ITransportBuilder ConfigureEndpoints(Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		_endpointConfigurations.Add(configure);
		return this;
	}

	/// <summary>
	///     Adds the endpoints for consumers that are decorated with a <see cref="QueueNameAttribute" />.
	/// </summary>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder ConfigureEndpointsByAttribute()
	{
		var consumersByQueueName = new Dictionary<string, List<Type>>();

		// Group consumers by queue name
		foreach (var assembly in _consumerAssemblies)
		foreach (var type in assembly.GetTypes())
		{
			var queueNameAttribute = type.GetCustomAttribute<QueueNameAttribute>();
			if (queueNameAttribute == null) continue;

			if (!consumersByQueueName.ContainsKey(queueNameAttribute.QueueName)) consumersByQueueName[queueNameAttribute.QueueName] = new();

			consumersByQueueName[queueNameAttribute.QueueName]
				.Add(type);
		}

		// Configure each queue with its consumers
		foreach (var kvp in consumersByQueueName)
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
								e.ConfigureConsumer(
									ctx,
									consumerType
								);
						}
					);
				}
			);

		return this;
	}

	/// <summary>
	///     Adds the endpoints for consumers that are decorated with a <see cref="ExchangeNameAttribute" />.
	/// </summary>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder ConfigureEndpointsByAttribute(
		string queueName,
		string exchangeType)
	{
		var consumersByExchange = new Dictionary<string, List<Type>>();

		// Group consumers by exchange name
		foreach (var assembly in _consumerAssemblies)
		foreach (var type in assembly.GetTypes())
		{
			var exchangeNameAttribute = type.GetCustomAttribute<ExchangeNameAttribute>();
			if (exchangeNameAttribute == null) continue;

			if (consumersByExchange.ContainsKey(exchangeNameAttribute.ExchangeName) == false)
				consumersByExchange[exchangeNameAttribute.ExchangeName] = new();

			consumersByExchange[exchangeNameAttribute.ExchangeName]
				.Add(type);
		}

		// Configure each exchange with its consumers
		foreach (var kvp in consumersByExchange)
			_endpointConfigurations.Add(
				(
					cfg,
					ctx) =>
				{
					cfg.ReceiveEndpoint(
						queueName,
						e =>
						{
							e.Bind(
								kvp.Key,
								exchangeConfigurator => { exchangeConfigurator.ExchangeType = exchangeType; }
							);

							foreach (var consumerType in kvp.Value)
								e.ConfigureConsumer(
									ctx,
									consumerType
								);
						}
					);
				}
			);

		return this;
	}

	/// <summary>
	///     Sets a retry configuration.
	/// </summary>
	/// <param name="configureRetry">The retry configuration action.</param>
	/// <returns>The current builder instance.</returns>
	public ITransportBuilder UseRetry(Action<IRetryConfigurator> configureRetry)
	{
		_retryConfigurator = configureRetry;
		return this;
	}

	/// <summary>
	///		Sets a redelivery configuration.
	/// </summary>
	/// <param name="configureRedelivery"></param>
	/// <returns></returns>
	public ITransportBuilder UseRedelivery(Action<IRedeliveryConfigurator> configureRedelivery)
	{
		_redeliveryConfigurator = configureRedelivery;
		return this;
	}
}