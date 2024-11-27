using System;

namespace ZEA.Techniques.DiscriminatedUnions.Generators.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class VariantAttribute : Attribute;