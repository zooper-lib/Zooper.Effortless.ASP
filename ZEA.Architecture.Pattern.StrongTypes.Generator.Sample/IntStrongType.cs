using ZEA.Architecture.Pattern.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Pattern.StrongTypes.Interfaces;

namespace ZEA.Architecture.Pattern.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record IntStrongType(int Value) : StrongTypeRecord<int, IntStrongType>(Value)
{
	public partial class IntStrongTypeValueConverter;

	public partial class IntStrongTypeNewtonsoftJsonConverter;

	public partial class IntStrongTypeTypeConverter;
}