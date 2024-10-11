using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.MassTransit.Builders;

namespace ZEA.Communications.Messaging.MassTransit.AzureServiceBus.Builders;

/// <summary>
/// Builder class for configuring MassTransit with Azure Service Bus.
/// </summary>
public class AzureServiceBusBuilder : ITransportBuilder
{
	private readonly string _connectionString;
	private readonly List<Assembly> _consumerAssemblies = [];

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
						// Add additional Azure Service Bus configurations here
					}
				);
			}
		);
	}
}