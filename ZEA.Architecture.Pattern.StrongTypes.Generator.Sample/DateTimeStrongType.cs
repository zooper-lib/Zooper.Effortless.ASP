using System;
using ZEA.Architecture.Pattern.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Pattern.StrongTypes.Interfaces;

namespace ZEA.Architecture.Pattern.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record DateTimeStrongType(DateTime Value) : StrongTypeRecord<DateTime, DateTimeStrongType>(Value)
{
	public partial class DateTimeStrongTypeValueConverter;

	public partial class DateTimeStrongTypeNewtonsoftJsonConverter;

	public partial class DateTimeStrongTypeTypeConverter;
}