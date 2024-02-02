using System.Reflection;
using Newtonsoft.Json;
using Refit;
using Zooper.Effortless.ASP.Serialization.NewtonsoftJson;

namespace Zooper.Effortless.ASP.Communication.Refit.Configurations;

public class RefitConfigurator
{
	public static RefitSettings GetRefitSettings(
		IEnumerable<JsonConverter> converters,
		TypeNameHandling typeNameHandling,
		params Assembly[] assemblies)
	{
		if (!assemblies.Any()) assemblies = AppDomain.CurrentDomain.GetAssemblies();

		var knownTypes = new List<Type>();
		knownTypes.AddRange(assemblies.SelectMany(assembly => assembly.GetTypes()));

		// Get all known converters from assemblies
		var foundConverters = assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(JsonConverter)) && type.IsAbstract == false)
			.Select(type => (JsonConverter)Activator.CreateInstance(type)!)
			.ToList();

		return new()
		{
			ContentSerializer = new NewtonsoftJsonContentSerializer(
				new()
				{
					//ContractResolver = new CamelCasePropertyNamesWithDiscriminatorResolver(),
					Converters = converters.Concat(foundConverters)
						.ToList(),
					TypeNameHandling = typeNameHandling,
					SerializationBinder = new KnownTypesBinder(knownTypes)
				}
			)
		};
	}
}