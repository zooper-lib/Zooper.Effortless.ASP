using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record Height(int Value) : StrongTypeRecord<int, Height>(Value)
{
	public partial class HeightValueConverter;

	public partial class HeightNewtonsoftJsonConverter;

	public partial class HeightTypeConverter;
}