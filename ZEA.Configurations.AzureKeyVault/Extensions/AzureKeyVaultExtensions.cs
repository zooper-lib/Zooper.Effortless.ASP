using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ZEA.Configurations.AzureKeyVault.Extensions;

[UsedImplicitly]
public static class AzureKeyVaultExtensions
{
	/// <summary>
	///     Configures the Azure Key Vault for the application.
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static IHostBuilder AddAzureKeyVault(this IHostBuilder builder)
	{
		return builder.ConfigureAppConfiguration(
			configurationBuilder =>
			{
				var settings = configurationBuilder.Build();

				var tenantId = settings["Azure:TenantId"] ?? throw new("TenantId is missing from settings");
				var url = settings["Azure:KeyVaultUrl"] ?? throw new("KeyVaultUrl is missing from settings");
				var clientId = settings["Azure:ClientId"] ?? throw new("ClientId is missing from settings");
				var clientSecret = settings["Azure:ClientSecret"] ?? throw new("ClientSecret is missing from settings");

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