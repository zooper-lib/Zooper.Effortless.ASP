using Microsoft.Extensions.DependencyInjection;
using ZEA.Communication.Messaging.MassTransit.Builders;

namespace ZEA.Communication.Messaging.MassTransit.Extensions;

public static class MassTransitExtensions
{
	public static IServiceCollection AddMassTransitServices(
		this IServiceCollection services,
		Action<MassTransitBuilder> configure)
	{
		var builder = new MassTransitBuilder(services);
		configure(builder);
		return services;
	}
}