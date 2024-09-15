using System;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateConvertersAttribute(
#pragma warning disable CS9113 // Parameter is unread.
	bool generateValueConverter = true,
	bool generateNewtonsoftJsonConverter = true,
	bool generateTypeConverter = true
#pragma warning restore CS9113 // Parameter is unread.
) : Attribute;