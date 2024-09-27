using ZEA.Communications.Refit.Models;

namespace ZEA.Communications.Refit.Stores;

public interface IAccessDataStore
{
	Task<AccessData?> GetAsync(CancellationToken cancellationToken);

	Task SetAsync(
		AccessData? token,
		CancellationToken cancellationToken);
}