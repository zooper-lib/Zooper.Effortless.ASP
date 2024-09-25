using System;
using ZEA.Architecture.Pattern.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Pattern.StrongTypes.Interfaces;

namespace ZEA.Architecture.Pattern.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record GuidStrongTypeRecord(Guid Value) : StrongTypeRecord<Guid, GuidStrongTypeRecord>(Value)
{
	public partial class GuidStrongTypeRecordValueConverter;

	public partial class GuidStrongTypeRecordNewtonsoftJsonConverter;

	public partial class GuidStrongTypeRecordTypeConverter;
}