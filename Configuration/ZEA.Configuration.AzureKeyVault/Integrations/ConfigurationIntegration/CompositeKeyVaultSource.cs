using Microsoft.Extensions.Configuration;

namespace ZEA.Configuration.AzureKeyVault.Integrations.ConfigurationIntegration;

public class CompositeKeyVaultSource(IEnumerable<string> keyVaultUris) : IConfigurationSource
{
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new CompositeKeyVaultProvider(keyVaultUris);
	}
}