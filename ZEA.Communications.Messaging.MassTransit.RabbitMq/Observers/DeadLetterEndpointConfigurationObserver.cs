using MassTransit;

namespace ZEA.Communications.Messaging.MassTransit.RabbitMq.Observers;

public class DeadLetterEndpointConfigurationObserver(
	string deadLetterExchange,
	string? deadLetterRoutingKey) : IEndpointConfigurationObserver
{
	public void EndpointConfigured<T>(T configurator) where T : IReceiveEndpointConfigurator
	{
		if (configurator is not IRabbitMqReceiveEndpointConfigurator rmqEndpointConfigurator)
		{
			return;
		}

		// Set the dead-letter exchange
		rmqEndpointConfigurator.SetQueueArgument("x-dead-letter-exchange", deadLetterExchange);

		// Set the dead-letter routing key if provided
		if (!string.IsNullOrEmpty(deadLetterRoutingKey))
		{
			rmqEndpointConfigurator.SetQueueArgument("x-dead-letter-routing-key", deadLetterRoutingKey);
		}
	}
}