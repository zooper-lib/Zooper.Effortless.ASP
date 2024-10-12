using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.MassTransit.Builders;

namespace ZEA.Communications.Messaging.MassTransit.Extensions;

/// <summary>
/// Extension methods for configuring MassTransit.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds MassTransit to the service collection using the specified configuration.
	/// </summary>
	/// <param name="services">The service collection to add MassTransit to.</param>
	/// <param name="configure">An action to configure the MassTransit builder.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddMessagingWithMassTransit(
		this IServiceCollection services,
		Action<MassTransitBuilder> configure)
	{
		var builder = new MassTransitBuilder(services);
		configure(builder);
		builder.Build();
		return services;
	}
}