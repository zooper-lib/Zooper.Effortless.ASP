namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateConvertersAttribute(
	bool generateValueConverter = true,
	bool generateNewtonsoftJsonConverter = true,
	bool generateTypeConverter = true)
	: Attribute
{
	public bool GenerateValueConverter { get; } = generateValueConverter;
	public bool GenerateNewtonsoftJsonConverter { get; } = generateNewtonsoftJsonConverter;
	public bool GenerateTypeConverter { get; } = generateTypeConverter;
}