using System.Text.Json;
using MassTransit;
using Newtonsoft.Json;
using ZEA.Communications.Messaging.MassTransit.Interfaces;

namespace ZEA.Communications.Messaging.MassTransit.Builders;

/// <summary>
/// Interface for transport builders to configure MassTransit with different transports.
/// </summary>
public interface ITransportBuilder
{
	/// <summary>
	/// Configures the transport-specific settings.
	/// </summary>
	/// <param name="configurator">The bus registration configurator.</param>
	void ConfigureTransport(IBusRegistrationConfigurator configurator);

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
	/// Configures the message retry policy.
	/// </summary>
	/// <param name="configureRetry">An action to configure the retry policy.</param>
	/// <returns>The current transport builder instance.</returns>
	ITransportBuilder UseMessageRetry(Action<IRetryConfigurator> configureRetry);
}