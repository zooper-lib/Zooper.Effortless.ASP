using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record IntStrongType(int Value) : StrongTypeRecord<int, IntStrongType>(Value)
{
	public partial class IntStrongTypeValueConverter;

	public partial class IntStrongTypeNewtonsoftJsonConverter;

	public partial class IntStrongTypeTypeConverter;
}