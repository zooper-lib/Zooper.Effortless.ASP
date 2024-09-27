using System;
using ZEA.Techniques.StrongTypes.Generators.Attributes;
using ZEA.Techniques.StrongTypes.Interfaces;

namespace ZEA.Techniques.StrongTypes.Generators.Samples;

[GenerateConverters]
public partial record DateTimeStrongType(DateTime Value) : StrongTypeRecord<DateTime, DateTimeStrongType>(Value)
{
	public partial class DateTimeStrongTypeValueConverter;

	public partial class DateTimeStrongTypeNewtonsoftJsonConverter;

	public partial class DateTimeStrongTypeTypeConverter;
}