using System.Reflection;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

[Obsolete("Use 'AzureServiceBusBuilder' in 'ZEA.Communications.Messaging.MassTransit.AzureServiceBus' instead.")]
public class AzureServiceBusBuilder(string connectionString) : ITransportBuilder
{
	private readonly List<Assembly> _consumerAssemblies = [];

	public ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies)
	{
		_consumerAssemblies.AddRange(consumerAssemblies);
		return this;
	}

	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		throw new NotImplementedException();
	}

	public ITransportBuilder UseNewtonsoftJson(Func<JsonSerializerSettings, JsonSerializerSettings> configure)
	{
		throw new NotImplementedException();
	}

	public ITransportBuilder UseSystemTextJson(Func<JsonSerializerOptions, JsonSerializerOptions> configure)
	{
		throw new NotImplementedException();
	}

	public ITransportBuilder UseNewtonsoftJsonSerialization(TypeNameHandling typeNameHandling = TypeNameHandling.Objects)
	{
		throw new NotImplementedException();
	}

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