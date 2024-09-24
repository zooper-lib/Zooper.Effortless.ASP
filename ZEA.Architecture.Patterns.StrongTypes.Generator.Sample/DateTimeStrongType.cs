using System;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateConverters]
public partial record DateTimeStrongType(DateTime Value) : StrongTypeRecord<DateTime, DateTimeStrongType>(Value)
{
	public partial class DateTimeStrongTypeValueConverter;

	public partial class DateTimeStrongTypeNewtonsoftJsonConverter;

	public partial class DateTimeStrongTypeTypeConverter;
}