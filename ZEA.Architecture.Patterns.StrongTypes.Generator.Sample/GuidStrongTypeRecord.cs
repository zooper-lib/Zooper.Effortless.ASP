using System;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record GuidStrongTypeRecord(Guid Value) : StrongTypeRecord<Guid, GuidStrongTypeRecord>(Value)
{
	public partial class GuidStrongTypeRecordValueConverter;

	public partial class GuidStrongTypeRecordNewtonsoftJsonConverter;

	public partial class GuidStrongTypeRecordTypeConverter;
}