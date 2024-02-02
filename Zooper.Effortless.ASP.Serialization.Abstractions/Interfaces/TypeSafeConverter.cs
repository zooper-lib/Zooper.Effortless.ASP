using System.ComponentModel;
using System.Globalization;

namespace Zooper.Effortless.ASP.Serialization.Abstractions.Interfaces;

public abstract class TypeSafeConverter<TSource, TDestination> : TypeConverter, ITypeConverterFor<TSource>
{
	public override bool CanConvertFrom(
		ITypeDescriptorContext? context,
		Type sourceType)
	{
		return sourceType == typeof(TDestination) || base.CanConvertFrom(
			context,
			sourceType
		);
	}

	public override object? ConvertFrom(
		ITypeDescriptorContext? context,
		CultureInfo? culture,
		object value)
	{
		if (value is TDestination destValue) return ConvertFromType(destValue);

		return base.ConvertFrom(
			context,
			culture,
			value
		);
	}

	public override bool CanConvertTo(
		ITypeDescriptorContext? context,
		Type? destinationType)
	{
		return destinationType == typeof(TDestination) || base.CanConvertTo(
			context,
			destinationType
		);
	}

	public override object? ConvertTo(
		ITypeDescriptorContext? context,
		CultureInfo? culture,
		object? value,
		Type destinationType)
	{
		if (destinationType == typeof(TDestination) && value is TSource srcValue) return ConvertToType(srcValue);

		return base.ConvertTo(
			context,
			culture,
			value,
			destinationType
		);
	}

	protected abstract TSource ConvertFromType(TDestination value);
	protected abstract TDestination ConvertToType(TSource value);
}