using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Zooper.Effortless.ASP.Serialization.NewtonsoftJson.Extensions;

public static class NewtonsoftJsonExtensions
{
	/// <summary>
	///     Configures the Newtonsoft Json Serialization for the application.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="assemblies">The assemblies to scan for types and Json converters.</param>
	/// <returns>The IServiceCollection so that additional calls can be chained.</returns>
	public static IServiceCollection ConfigureNewtonsoftJsonSerialization(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		if (!assemblies.Any()) assemblies = AppDomain.CurrentDomain.GetAssemblies();

		var knownTypes = new List<Type>();
		knownTypes.AddRange(assemblies.SelectMany(assembly => assembly.GetTypes()));

		// Get all known converters from assemblies
		var converters = assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(JsonConverter)) && type.IsAbstract == false)
			.Select(type => (JsonConverter)Activator.CreateInstance(type)!)
			.ToList();

		// Add the default converters
		converters.Add(new StringEnumConverter());
		converters.Add(new IsoDateTimeConverter());

		services.AddMvc()
			.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
					options.SerializerSettings.SerializationBinder = new KnownTypesBinder(knownTypes);
					options.SerializerSettings.Converters = converters;
				}
			);

		JsonConvert.DefaultSettings = () => new()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Objects,
			SerializationBinder = new KnownTypesBinder(knownTypes),
			Converters = converters
		};

		return services;
	}
}