using System.Text.Json;
using MassTransit;
using Newtonsoft.Json;
using ZEA.Communications.Messaging.MassTransit.Builders;
using ZEA.Communications.Messaging.MassTransit.Extensions;

namespace ZEA.Communications.Messaging.MassTransit.Interfaces;

public abstract class TransportBuilderBase<TConfigurator> : ITransportBuilder
	where TConfigurator : class, IBusFactoryConfigurator
{
	protected bool ExcludeBaseInterfaces;
	protected Func<JsonSerializerSettings, JsonSerializerSettings>? NewtonsoftJsonConfig;
	protected Func<JsonSerializerOptions, JsonSerializerOptions>? SystemTextJsonConfig;
	protected Action<IRetryConfigurator>? RetryConfigurator;
	protected Action<TConfigurator, IBusRegistrationContext>? ConfigureBusFunction;
	protected Action<TConfigurator>? AdditionalConfigurator;

	public ITransportBuilder ExcludeBaseInterfacesFromPublishing(bool exclude)
	{
		ExcludeBaseInterfaces = exclude;
		return this;
	}

	public ITransportBuilder UseNewtonsoftJson(Func<JsonSerializerSettings, JsonSerializerSettings> configure)
	{
		NewtonsoftJsonConfig = configure;
		return this;
	}

	public ITransportBuilder UseSystemTextJson(Func<JsonSerializerOptions, JsonSerializerOptions> configure)
	{
		SystemTextJsonConfig = configure;
		return this;
	}

	public ITransportBuilder UseMessageRetry(Action<IRetryConfigurator> configureRetry)
	{
		RetryConfigurator = configureRetry;
		return this;
	}

	public abstract void ConfigureTransport(IBusRegistrationConfigurator configurator);

	public ITransportBuilder ConfigureTransport(Action<TConfigurator> configure)
	{
		AdditionalConfigurator = configure;
		return this;
	}

	protected void ApplyCommonConfigurations(TConfigurator cfg)
	{
		// Apply Newtonsoft.Json configuration
		if (NewtonsoftJsonConfig != null)
		{
			cfg.UseNewtonsoftJsonSerializer();
			cfg.ConfigureNewtonsoftJsonSerializer(NewtonsoftJsonConfig);
			cfg.UseNewtonsoftJsonDeserializer();
			cfg.ConfigureNewtonsoftJsonDeserializer(NewtonsoftJsonConfig);
		}

		// Apply System.Text.Json configuration
		if (SystemTextJsonConfig != null)
		{
			cfg.UseJsonSerializer();
			cfg.ConfigureJsonSerializerOptions(SystemTextJsonConfig);
			cfg.UseJsonDeserializer();
		}

		// Conditionally exclude base interfaces
		if (ExcludeBaseInterfaces)
		{
			cfg.ExcludeBaseInterfaces();
		}

		// Apply message retry policy
		if (RetryConfigurator != null)
		{
			cfg.UseMessageRetry(RetryConfigurator);
		}

		AdditionalConfigurator?.Invoke(cfg);
	}
}