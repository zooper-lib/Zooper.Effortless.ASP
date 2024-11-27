using ZEA.Techniques.DiscriminatedUnions.Generators.Attributes;
using OneOf;

namespace ZEA.Techniques.DiscriminatedUnions.Sample;

[DiscriminatedUnion]
public partial class SignUpError
{
	[Variant]
	public static partial SignUpError ServiceUnavailable();
	
	[Variant]
	public static partial SignUpError InvalidCredentials();
	
	[Variant]
	public static partial SignUpError InternalError();
}