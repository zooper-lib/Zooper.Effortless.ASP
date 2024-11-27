using ZEA.Techniques.DiscriminatedUnions.Generators.Attributes;

namespace ZEA.Techniques.DiscriminatedUnions.Sample;

[DiscriminatedUnion]
public partial class SignInError
{
	[Variant]
	public static partial SignInError ServiceUnavailable();
	
	[Variant]
	public static partial SignInError InvalidCredentials();
	
	[Variant]
	public static partial SignInError InternalError();
}