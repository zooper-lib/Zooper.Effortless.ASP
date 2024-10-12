using System.Reflection;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

[Obsolete("Use 'AzureServiceBusBuilder' in 'ZEA.Communications.Messaging.MassTransit.AzureServiceBus' instead.")]
public class AzureServiceBusBuilder(string connectionString) : ITransportBuilder
{
	[Obsolete]
	private readonly List<Assembly> _consumerAssemblies = [];

	[Obsolete]
	public ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		_consumerAssemblies.AddRange(consumerAssemblies);
		return this;
	}

	[Obsolete]
	public void ConfigureTransport(IBusRegistrationConfigurator configurator)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
	public ITransportBuilder UseNewtonsoftJson(Func<JsonSerializerSettings, JsonSerializerSettings> configure)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
	public ITransportBuilder UseSystemTextJson(Func<JsonSerializerOptions, JsonSerializerOptions> configure)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
	public ITransportBuilder UseMessageRetry(Action<IRetryConfigurator> configureRetry)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
	public ITransportBuilder UseNewtonsoftJsonSerialization(TypeNameHandling typeNameHandling = TypeNameHandling.Objects)
	{
		throw new NotImplementedException();
	}

	[Obsolete]
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
						cfg.Host(connectionString);
					}
				);
			}
		);
	}
}