using Microsoft.Extensions.Configuration;

namespace ZEA.Configurations.AzureKeyVault.Integrations.ConfigurationIntegration;

public static class CompositeKeyVaultConfigurationExtensions
{
	public static IConfigurationBuilder AddCompositeKeyVault(this IConfigurationBuilder builder, IEnumerable<string> keyVaultUris)
	{
		return builder.Add(new CompositeKeyVaultSource(keyVaultUris));
	}
}