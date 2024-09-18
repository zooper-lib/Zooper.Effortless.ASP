using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ZEA.Serialization.NewtonsoftJson.Builders;

/// <summary>
/// Provides a fluent API for configuring Newtonsoft.Json serialization settings in an ASP.NET Core application.
/// </summary>
public class NewtonsoftJsonBuilder(IServiceCollection services)
{
	private readonly List<JsonConverter> _converters = [];
	private ReferenceLoopHandling _referenceLoopHandling = ReferenceLoopHandling.Ignore;
	private TypeNameHandling _typeNameHandling = TypeNameHandling.Objects;
	private ISerializationBinder? _serializationBinder = new DefaultSerializationBinder();

	/// <summary>
	/// Scans the specified assemblies for custom JSON converters and adds them to the list of converters used by Newtonsoft.Json.
	/// </summary>
	/// <param name="assemblies">The assemblies to scan for custom JSON converters.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder AddConvertersFromAssemblies(params Assembly[] assemblies)
	{
		var customConverters = assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(JsonConverter)) && !type.IsAbstract)
			.Select(type => (JsonConverter)Activator.CreateInstance(type)!)
			.ToList();

		_converters.AddRange(customConverters);
		return this;
	}

	/// <summary>
	/// Adds a single JSON converter to the list of converters used by Newtonsoft.Json.
	/// </summary>
	/// <param name="converter">The JSON converter to add.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder AddConverter(JsonConverter converter)
	{
		_converters.Add(converter);
		return this;
	}

	/// <summary>
	/// Adds multiple JSON converters to the list of converters used by Newtonsoft.Json.
	/// </summary>
	/// <param name="converters">The JSON converters to add.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder AddConverters(params JsonConverter[] converters)
	{
		_converters.AddRange(converters);
		return this;
	}

	/// <summary>
	/// Sets the handling of reference loops in JSON serialization.
	/// </summary>
	/// <param name="handling">The reference loop handling setting to use.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder SetReferenceLoopHandling(ReferenceLoopHandling handling)
	{
		_referenceLoopHandling = handling;
		return this;
	}

	/// <summary>
	/// Sets the handling of type names in JSON serialization.
	/// </summary>
	/// <param name="handling">The type name handling setting to use.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder SetTypeNameHandling(TypeNameHandling handling)
	{
		_typeNameHandling = handling;
		return this;
	}

	/// <summary>
	/// Sets the serialization binder used by Newtonsoft.Json to resolve and bind types during deserialization.
	/// </summary>
	/// <param name="binder">The serialization binder to use.</param>
	/// <returns>The current <see cref="NewtonsoftJsonBuilder"/> instance for chaining further configuration.</returns>
	public NewtonsoftJsonBuilder SetSerializationBinder(ISerializationBinder binder)
	{
		_serializationBinder = binder;
		return this;
	}

	/// <summary>
	/// Finalizes the configuration and applies the Newtonsoft.Json settings to the provided <see cref="IServiceCollection"/>.
	/// Also configures the default JSON settings for the application.
	/// </summary>
	/// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
	public IServiceCollection Build()
	{
		_serializationBinder ??= new DefaultSerializationBinder();

		services.AddMvc()
			.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = _referenceLoopHandling;
					options.SerializerSettings.TypeNameHandling = _typeNameHandling;
					options.SerializerSettings.SerializationBinder = _serializationBinder;
					options.SerializerSettings.Converters = _converters;
				}
			);

		JsonConvert.DefaultSettings = () => new()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			ReferenceLoopHandling = _referenceLoopHandling,
			TypeNameHandling = _typeNameHandling,
			SerializationBinder = _serializationBinder,
			Converters = _converters
		};

		return services;
	}
}