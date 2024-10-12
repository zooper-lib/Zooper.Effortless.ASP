using System;
using ZEA.Techniques.StrongTypes.Generators.Attributes;
using ZEA.Techniques.StrongTypes.Interfaces;

namespace ZEA.Techniques.StrongTypes.Generators.Samples;

[GenerateConverters]
public partial record GuidStrongTypeRecord(Guid Value) : StrongTypeRecord<Guid, GuidStrongTypeRecord>(Value)
{
	public partial class GuidStrongTypeRecordValueConverter;

	public partial class GuidStrongTypeRecordNewtonsoftJsonConverter;

	public partial class GuidStrongTypeRecordTypeConverter;
}