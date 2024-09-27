using System.Reflection;
using ProtoBuf.Meta;
using ZEA.Communications.gRPC.Interfaces;

namespace ZEA.Communications.gRPC.Extensions;

/// <summary>
/// Provides extension methods for RuntimeTypeModel to configure surrogates for protobuf-net serialization.
/// </summary>
public static class RuntimeTypeModelExtensions
{
	/// <summary>
	/// Configures a surrogate type for a model type in the given RuntimeTypeModel.
	/// </summary>
	/// <typeparam name="TModel">The model type to be surrogated.</typeparam>
	/// <typeparam name="TSurrogate">The surrogate type for the model.</typeparam>
	/// <param name="model">The RuntimeTypeModel to configure.</param>
	public static void ConfigureSurrogate<TModel, TSurrogate>(this RuntimeTypeModel model)
		where TSurrogate : BaseSurrogate<TModel, TSurrogate>, new()
		where TModel : class
	{
		var metaType = model.Add(typeof(TModel), false);
		metaType.SetSurrogate(typeof(TSurrogate));
	}

	/// <summary>
	/// Automatically configures all surrogate types found in the specified assemblies or the current AppDomain for the given RuntimeTypeModel.
	/// </summary>
	/// <param name="model">The RuntimeTypeModel to configure.</param>
	/// <param name="assemblies">Optional list of assemblies to scan for surrogate types. If not provided, scans the current AppDomain.</param>
	public static void ConfigureAllSurrogates(
		this RuntimeTypeModel model,
		params Assembly[] assemblies)
	{
		var surrogateType = typeof(BaseSurrogate<,>);
		var assembliesToScan = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

		var surrogates = assembliesToScan
			.SelectMany(s => s.GetTypes())
			.Where(p => p is { IsClass: true, IsAbstract: false } && p.IsSubclassOfRawGeneric(surrogateType));

		foreach (var surrogate in surrogates)
		{
			var modelType = surrogate.BaseType?.GetGenericArguments()[0];

			if (modelType == null) continue;

			var method = typeof(RuntimeTypeModelExtensions).GetMethod(nameof(ConfigureSurrogate));
			var genericMethod = method?.MakeGenericMethod(modelType, surrogate);
			genericMethod?.Invoke(
				null,
				[
					model
				]
			);
		}
	}

	/// <summary>
	/// Determines whether the given type is a subclass of a raw generic type.
	/// </summary>
	/// <param name="toCheck">The type to check.</param>
	/// <param name="generic">The raw generic type to compare against.</param>
	/// <returns>True if the type is a subclass of the raw generic type, false otherwise.</returns>
	private static bool IsSubclassOfRawGeneric(
		this Type? toCheck,
		Type generic)
	{
		while (toCheck != null && toCheck != typeof(object))
		{
			var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

			if (generic == cur)
			{
				return true;
			}

			toCheck = toCheck.BaseType;
		}

		return false;
	}
}