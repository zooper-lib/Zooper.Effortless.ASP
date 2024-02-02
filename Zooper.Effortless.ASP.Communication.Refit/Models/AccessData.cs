using IdentityModel.Client;

namespace Zooper.Effortless.ASP.Communication.Refit.Models;

/// <summary>
///     This class is a wrapper around the TokenResponse
/// </summary>
public record AccessData
{
	public required TokenResponse AuthResponse { get; set; }
	public required DateTime ExpiresAt { get; set; }
}