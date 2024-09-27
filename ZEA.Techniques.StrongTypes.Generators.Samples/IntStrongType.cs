using ZEA.Techniques.StrongTypes.Generators.Attributes;
using ZEA.Techniques.StrongTypes.Interfaces;

namespace ZEA.Techniques.StrongTypes.Generators.Samples;

[GenerateConverters]
public partial record IntStrongType(int Value) : StrongTypeRecord<int, IntStrongType>(Value)
{
	public partial class IntStrongTypeValueConverter;

	public partial class IntStrongTypeNewtonsoftJsonConverter;

	public partial class IntStrongTypeTypeConverter;
}