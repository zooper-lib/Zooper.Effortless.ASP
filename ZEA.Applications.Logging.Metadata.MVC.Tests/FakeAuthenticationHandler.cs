using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ZEA.Applications.Logging.Metadata.MVC.Tests;

public class FakeAuthenticationHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder,
	ISystemClock clock)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		// Check if the request should be authenticated
		if (Context.Request.Headers.ContainsKey("X-Authenticated"))
		{
			// Proceed with authentication
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "TestUser"),
				new Claim(ClaimTypes.Name, "Test User"),
				new Claim(ClaimTypes.Role, "User"),
			};

			var identity = new ClaimsIdentity(claims, "FakeAuthentication");
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, "FakeAuthentication");

			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
		else
		{
			// Do not authenticate
			return Task.FromResult(AuthenticateResult.NoResult());
		}
	}
}