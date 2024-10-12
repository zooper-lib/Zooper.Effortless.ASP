using IdentityModel.Client;

namespace ZEA.Communications.Refit.Models;

/// <summary>
///     This class is a wrapper around the TokenResponse
/// </summary>
public record AccessData
{
	public required TokenResponse AuthResponse { get; set; }
	public required DateTime ExpiresAt { get; set; }
}