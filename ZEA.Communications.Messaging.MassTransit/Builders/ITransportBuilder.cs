using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
	/// Sets whether to exclude base interfaces from publishing.
	/// This can prevent unintended publishing of base interfaces.
	/// </summary>
	/// <param name="exclude">True to exclude base interfaces; false to include them.</param>
	/// <returns>The current transport builder instance.</returns>
	ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude);

	/// <summary>
	/// Configures MassTransit to use Newtonsoft.Json with custom settings.
	/// </summary>
	/// <param name="configure">Action to configure JsonSerializerSettings.</param>
	/// <returns>The current transport builder instance.</returns>
	ITransportBuilder UseNewtonsoftJson(Func<JsonSerializerSettings, JsonSerializerSettings> configure);

	/// <summary>
	/// Configures MassTransit to use System.Text.Json with custom options.
	/// </summary>
	/// <param name="configure">Action to configure JsonSerializerOptions.</param>
	/// <returns>The current transport builder instance.</returns>
	ITransportBuilder UseSystemTextJson(Func<JsonSerializerOptions, JsonSerializerOptions> configure);

	/// <summary>
	/// Builds the MassTransit configuration and registers it with the service collection.
	/// </summary>
	/// <param name="services">The service collection to register services with.</param>
	void Build(IServiceCollection services);
}