using ZEA.Communication.Refit.Models;

namespace ZEA.Communication.Refit.Stores;

public interface IAccessDataStore
{
	Task<AccessData?> GetAsync(CancellationToken cancellationToken);

	Task SetAsync(
		AccessData? token,
		CancellationToken cancellationToken);
}