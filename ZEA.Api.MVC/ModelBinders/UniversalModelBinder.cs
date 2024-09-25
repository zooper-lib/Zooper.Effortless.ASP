using System.ComponentModel;
using System.Reflection;
using AspNetCore.ClaimsValueProvider;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZEA.Api.MVC.ModelBinders;

/// <summary>
/// A universal model binder that can bind data from headers and claims.
/// </summary>
public class UniversalModelBinder : IModelBinder
{
	/// <summary>
	/// Binds the model asynchronously.
	/// </summary>
	/// <param name="bindingContext">The model binding context.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		// Handle Headers
		if (bindingContext.BindingSource == BindingSource.Header) return BindHeader(bindingContext);

		// Handle Claims
		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (bindingContext.BindingSource == ClaimsBindingSource.BindingSource) return BindClaim(bindingContext);

		return Task.CompletedTask;
	}

	/// <summary>
	/// Binds data from the header to the model.
	/// </summary>
	/// <param name="bindingContext">The model binding context.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private static Task BindHeader(ModelBindingContext bindingContext)
	{
		var logger = GetLogger(bindingContext) ?? throw new InvalidOperationException("Logger not available");

		var headerName = bindingContext.FieldName;
		var headerValues = bindingContext.HttpContext.Request.Headers[headerName];
		if (headerValues.Count == 0)
		{
			MarkBindingFailed(bindingContext);
			return Task.CompletedTask;
		}

		var value = headerValues[0];
		if (value is null)
		{
			MarkBindingFailed(bindingContext);
			return Task.CompletedTask;
		}

		try
		{
			if (TryConvertValue(
				    bindingContext.ModelType,
				    value,
				    out var convertedValue
			    ))
			{
				bindingContext.Result = ModelBindingResult.Success(convertedValue);
			}
			else
			{
				var model = ConstructModel(
					bindingContext.ModelType,
					value
				);
				bindingContext.Result = model != null ? ModelBindingResult.Success(model) : ModelBindingResult.Failed();
			}
		}
		catch (Exception ex)
		{
			logger.LogError(
				ex,
				"Failed to bind header {HeaderName} to model {ModelType}",
				headerName,
				bindingContext.ModelType
			);
			MarkBindingFailed(bindingContext);
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Binds data from the claim to the model.
	/// </summary>
	/// <param name="bindingContext">The model binding context.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private static Task BindClaim(ModelBindingContext bindingContext)
	{
		var logger = GetLogger(bindingContext) ?? throw new InvalidOperationException("Logger not available");

		var claimType = bindingContext.FieldName;
		var claimValue = bindingContext.HttpContext.User.FindFirst(claimType)
			?.Value;

		if (string.IsNullOrEmpty(claimValue))
		{
			MarkBindingFailed(bindingContext);
			return Task.CompletedTask;
		}

		try
		{
			if (TryConvertValue(
				    bindingContext.ModelType,
				    claimValue,
				    out var convertedValue
			    ))
			{
				bindingContext.Result = ModelBindingResult.Success(convertedValue);
			}
			else
			{
				var model = ConstructModel(
					bindingContext.ModelType,
					claimValue
				);
				bindingContext.Result = model != null ? ModelBindingResult.Success(model) : ModelBindingResult.Failed();
			}
		}
		catch (Exception ex)
		{
			logger.LogError(
				ex,
				"Failed to bind claim {ClaimType} to model {ModelType}",
				claimType,
				bindingContext.ModelType
			);
			MarkBindingFailed(bindingContext);
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Retrieves the logger service.
	/// </summary>
	/// <param name="bindingContext">The model binding context.</param>
	/// <returns>The logger service.</returns>
	private static ILogger<UniversalModelBinder>? GetLogger(ModelBindingContext bindingContext)
	{
		return bindingContext.HttpContext.RequestServices.GetService<ILogger<UniversalModelBinder>>();
	}

	/// <summary>
	/// Marks the binding as failed.
	/// </summary>
	/// <param name="bindingContext">The model binding context.</param>
	private static void MarkBindingFailed(ModelBindingContext bindingContext)
	{
		bindingContext.Result = ModelBindingResult.Failed();
	}

	/// <summary>
	/// Tries to convert the value to the target type.
	/// </summary>
	/// <param name="targetType">The target type.</param>
	/// <param name="value">The value to convert.</param>
	/// <param name="convertedValue">The converted value.</param>
	/// <returns>True if the conversion was successful, false otherwise.</returns>
	private static bool TryConvertValue(
		Type targetType,
		string value,
		out object? convertedValue)
	{
		var converter = TypeDescriptor.GetConverter(targetType);
		if (converter.CanConvertFrom(typeof(string)))
		{
			convertedValue = converter.ConvertFromInvariantString(value);
			return true;
		}

		convertedValue = null;
		return false;
	}

	/// <summary>
	/// Constructs a model using the provided value.
	/// </summary>
	/// <param name="modelType">The type of the model.</param>
	/// <param name="value">The value to use for constructing the model.</param>
	/// <returns>The constructed model.</returns>
	private static object? ConstructModel(
		Type modelType,
		string value)
	{
		// Try to find a public constructor that takes a single string argument
		var constructor = modelType.GetConstructor(new[] { typeof(string) });
		if (constructor != null)
		{
			return constructor.Invoke(new object[] { value });
		}

		// If no public constructor, try to find a static 'Create' method or similar
		var createMethod = modelType.GetMethod(
			"Create",
			BindingFlags.Public | BindingFlags.Static,
			null,
			new[] { typeof(string) },
			null
		);
		if (createMethod != null)
		{
			return createMethod.Invoke(
				null,
				new object[] { value }
			);
		}

		// Fallback to non-public constructor
		constructor = modelType.GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new[] { typeof(string) },
			null
		);
		if (constructor != null)
		{
			return constructor.Invoke(new object[] { value });
		}

		// No suitable constructor or method found
		return null;
	}
}