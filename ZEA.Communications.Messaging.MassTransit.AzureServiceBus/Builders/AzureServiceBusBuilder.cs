using MassTransit;
using ZEA.Communications.Messaging.MassTransit.Interfaces;

namespace ZEA.Communications.Messaging.MassTransit.AzureServiceBus.Builders;

/// <summary>
/// Builder class for configuring MassTransit with Azure Service Bus.
/// </summary>
public class AzureServiceBusBuilder(string connectionString) : TransportBuilderBase<IServiceBusBusFactoryConfigurator>
{
	private int? _maxDeliveryCount;
	private bool? _enableDeadLetteringOnMessageExpiration;

	public AzureServiceBusBuilder SetMaxDeliveryCount(int maxDeliveryCount)
	{
		_maxDeliveryCount = maxDeliveryCount;
		return this;
	}

	public AzureServiceBusBuilder EnableDeadLetteringOnMessageExpiration(bool enable)
	{
		_enableDeadLetteringOnMessageExpiration = enable;
		return this;
	}

	public AzureServiceBusBuilder ConfigureBus(Action<IServiceBusBusFactoryConfigurator, IBusRegistrationContext> configure)
	{
		ConfigureBusFunction = configure;
		return this;
	}

	/// <inheritdoc/>
	public override void ConfigureTransport(IBusRegistrationConfigurator configurator)
	{
		configurator.UsingAzureServiceBus(
			(
				context,
				cfg) =>
			{
				cfg.Host(connectionString);

				ApplyCommonConfigurations(cfg);

				// Apply Azure-specific configurations
				if (_maxDeliveryCount.HasValue)
				{
					cfg.MaxDeliveryCount = _maxDeliveryCount.Value;
				}

				if (_enableDeadLetteringOnMessageExpiration.HasValue)
				{
					cfg.EnableDeadLetteringOnMessageExpiration = _enableDeadLetteringOnMessageExpiration.Value;
				}

				ConfigureBusFunction?.Invoke(cfg, context);
			}
		);
	}
}