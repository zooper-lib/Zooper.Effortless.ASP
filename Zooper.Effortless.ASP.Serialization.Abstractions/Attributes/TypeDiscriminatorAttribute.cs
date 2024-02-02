namespace Zooper.Effortless.ASP.Serialization.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TypeDiscriminatorAttribute : Attribute
{
	public string Value { get; }

	public TypeDiscriminatorAttribute(string value)
	{
		Value = value;
	}
}