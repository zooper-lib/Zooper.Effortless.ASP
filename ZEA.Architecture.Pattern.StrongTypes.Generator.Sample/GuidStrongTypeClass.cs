using System;
using ZEA.Architecture.Pattern.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Pattern.StrongTypes.Interfaces;

namespace ZEA.Architecture.Pattern.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial class GuidStrongTypeClass(Guid value) : StrongTypeClass<Guid, GuidStrongTypeClass>(value)
{
	public partial class GuidStrongTypeClassValueConverter;

	public partial class GuidStrongTypeClassNewtonsoftJsonConverter;

	public partial class GuidStrongTypeClassTypeConverter;
}