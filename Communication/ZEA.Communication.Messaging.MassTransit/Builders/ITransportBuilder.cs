using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ZEA.Communication.Messaging.MassTransit.Builders;

public interface ITransportBuilder
{
	ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies);

	void Build(IServiceCollection services);
}