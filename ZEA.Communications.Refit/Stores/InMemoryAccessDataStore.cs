using ZEA.Communications.Refit.Models;

namespace ZEA.Communications.Refit.Stores;

public class InMemoryAccessDataStore : IAccessDataStore
{
	private AccessData? _accessData;

	public async Task<AccessData?> GetAsync(CancellationToken cancellationToken)
	{
		return await Task.FromResult(_accessData);
	}

	public Task SetAsync(
		AccessData? accessData,
		CancellationToken cancellationToken)
	{
		_accessData = accessData;
		return Task.CompletedTask;
	}
}