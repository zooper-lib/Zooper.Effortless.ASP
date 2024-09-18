using System;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial class GuidStrongTypeClass(Guid value) : StrongTypeClass<Guid, GuidStrongTypeClass>(value)
{
	public partial class GuidStrongTypeClassValueConverter;

	public partial class GuidStrongTypeClassNewtonsoftJsonConverter;

	public partial class GuidStrongTypeClassTypeConverter;
}