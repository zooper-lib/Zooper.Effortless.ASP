using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ZEA.Secrets.AzureKeyVault.Extensions;

public static class AzureKeyVaultExtensions
{
	/// <summary>
	///     Configures the Azure Key Vault for the application.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="tenantId"></param>
	/// <param name="url"></param>
	/// <param name="clientId"></param>
	/// <param name="clientSecret"></param>
	/// <returns></returns>
	public static IHostBuilder AddAzureKeyVault(
		this IHostBuilder builder,
		string tenantId,
		string url,
		string clientId,
		string clientSecret)
	{
		return builder.ConfigureAppConfiguration(
			configurationBuilder =>
			{
				configurationBuilder.AddAzureKeyVault(
					tenantId,
					url,
					clientId,
					clientSecret
				);
			}
		);
	}

	// ReSharper disable once MemberCanBePrivate.Global
	public static IConfigurationBuilder AddAzureKeyVault(
		this IConfigurationBuilder builder,
		string tenantId,
		string url,
		string clientId,
		string clientSecret)
	{
		var credentials = new ClientSecretCredential(
			tenantId,
			clientId,
			clientSecret
		);
		var client = new SecretClient(
			new(url),
			credentials
		);

		// * The KeyVaultSecretManager is used to replace "--" with ":" from the secret name.
		builder.AddAzureKeyVault(
			client,
			new KeyVaultSecretManager()
		);

		return builder;
	}
}