using Zooper.Effortless.ASP.Communication.Refit.Models;

namespace Zooper.Effortless.ASP.Communication.Refit.Stores;

public interface IAccessDataStore
{
	Task<AccessData?> GetAsync(CancellationToken cancellationToken);

	Task SetAsync(
		AccessData? token,
		CancellationToken cancellationToken);
}