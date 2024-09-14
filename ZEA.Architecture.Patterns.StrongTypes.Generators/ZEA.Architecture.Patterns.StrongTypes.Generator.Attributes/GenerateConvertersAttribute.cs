namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateConvertersAttribute : Attribute
{
	public GenerateConvertersAttribute(
		bool generateValueConverter = true,
		bool generateNewtonsoftJsonConverter = true,
		bool generateTypeConverter = true)
	{
		GenerateValueConverter = generateValueConverter;
		GenerateNewtonsoftJsonConverter = generateNewtonsoftJsonConverter;
		GenerateTypeConverter = generateTypeConverter;
	}

	public bool GenerateValueConverter { get; }
	public bool GenerateNewtonsoftJsonConverter { get; }
	public bool GenerateTypeConverter { get; }
}