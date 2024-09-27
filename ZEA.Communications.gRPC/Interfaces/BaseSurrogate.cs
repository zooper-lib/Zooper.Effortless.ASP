namespace ZEA.Communications.gRPC.Interfaces;

/// <summary>
/// Provides a base class for implementing surrogate types for protobuf-net serialization.
/// </summary>
/// <typeparam name="TModel">The model type to be surrogated.</typeparam>
/// <typeparam name="TSurrogate">The surrogate type for the model.</typeparam>
public abstract class BaseSurrogate<TModel, TSurrogate>
	where TSurrogate : BaseSurrogate<TModel, TSurrogate>, new()
	where TModel : class
{
	/// <summary>
	/// Converts the surrogate instance to its corresponding model instance.
	/// </summary>
	/// <returns>The model instance.</returns>
	protected abstract TModel ToModel();

	/// <summary>
	/// Creates a surrogate instance from a model instance.
	/// </summary>
	/// <param name="model">The model instance to convert.</param>
	/// <returns>The surrogate instance.</returns>
	protected abstract TSurrogate FromModel(TModel model);

	/// <summary>
	/// Converts a model instance to its corresponding surrogate instance, handling null values.
	/// </summary>
	/// <param name="model">The model instance to convert.</param>
	/// <returns>The surrogate instance, or null if the input is null.</returns>
	public static TSurrogate? ConvertToSurrogate(TModel? model)
		=> model == null ? null : new TSurrogate().FromModel(model);

	/// <summary>
	/// Converts a surrogate instance to its corresponding model instance, handling null values.
	/// </summary>
	/// <param name="surrogate">The surrogate instance to convert.</param>
	/// <returns>The model instance, or null if the input is null.</returns>
	public static TModel? ConvertToModel(TSurrogate? surrogate)
		=> surrogate?.ToModel();
}