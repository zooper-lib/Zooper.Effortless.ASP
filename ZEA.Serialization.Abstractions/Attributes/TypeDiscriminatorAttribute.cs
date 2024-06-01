namespace ZEA.Serialization.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TypeDiscriminatorAttribute(string value) : Attribute
{
	public string Value { get; } = value;
}