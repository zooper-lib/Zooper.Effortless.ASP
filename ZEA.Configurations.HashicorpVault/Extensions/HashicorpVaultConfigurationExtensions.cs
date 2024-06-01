using Microsoft.Extensions.Hosting;

// ReSharper disable IdentifierTypo

namespace ZEA.Configurations.HashicorpVault.Extensions;

public static class HashicorpVaultConfigurationExtensions
{
	public static IHostBuilder AddHashicorpVault(
		this IHostBuilder builder,
		string uri,
		string vaultToken,
		string mountPoint)
	{
		return builder.ConfigureAppConfiguration(
			(
				_,
				config) =>
			{
				config.Add(
					new HashicorpVaultConfigurationSource(
						uri,
						vaultToken,
						mountPoint
					)
				);
			}
		);
	}
}