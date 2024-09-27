using System;
using ZEA.Techniques.StrongTypes.Generators.Attributes;
using ZEA.Techniques.StrongTypes.Interfaces;

namespace ZEA.Techniques.StrongTypes.Generators.Samples;

[GenerateConverters]
public partial class GuidStrongTypeClass(Guid value) : StrongTypeClass<Guid, GuidStrongTypeClass>(value)
{
	public partial class GuidStrongTypeClassValueConverter;

	public partial class GuidStrongTypeClassNewtonsoftJsonConverter;

	public partial class GuidStrongTypeClassTypeConverter;
}