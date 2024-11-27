using System;
using OneOf;

namespace ZEA.Techniques.DiscriminatedUnions.Sample;

public class TestSample
{
	public void Test()
	{
		// Use the static method to create an instance of SignUpError
		var error = SignUpError.InternalError();

		var a = error.Match(
			serviceUnavailable => "",
			invalidCredentials => "",
			internalError => ""
		);
	}
}