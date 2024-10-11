using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

/// <summary>
/// Interface for transport builders to configure MassTransit with different transports.
/// </summary>
public interface ITransportBuilder
{
	/// <summary>
	/// Adds consumer assemblies to the transport builder.
	/// </summary>
	/// <param name="consumerAssemblies">Assemblies containing MassTransit consumers.</param>
	/// <returns>The current transport builder instance.</returns>
	ITransportBuilder AddConsumerAssemblies(params Assembly[] consumerAssemblies);

	/// <summary>
	/// Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	/// <param name="services">The service collection to register services with.</param>
	void Build(IServiceCollection services);
}