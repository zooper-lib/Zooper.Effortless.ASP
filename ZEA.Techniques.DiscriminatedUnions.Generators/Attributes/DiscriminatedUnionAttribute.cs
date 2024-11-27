using System;

namespace ZEA.Techniques.DiscriminatedUnions.Generators.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DiscriminatedUnionAttribute : Attribute;