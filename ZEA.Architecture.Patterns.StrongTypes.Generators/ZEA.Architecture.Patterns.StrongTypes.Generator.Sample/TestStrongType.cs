using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Sample;

[GenerateValueConverter]
public partial record TestStrongType(int Value) : StrongTypeRecord<int, TestStrongType>(Value)
{
	//public partial class TestStrongTypeValueConverter;
}