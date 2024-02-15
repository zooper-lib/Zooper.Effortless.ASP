using AspNetCore.ClaimsValueProvider;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ZEA.Api.MVC.ModelBinders;

public class UniversalModelBinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		// Check if the binding source is Header or Custom (which we'll use for claims)
		if (context.BindingInfo.BindingSource != null && (context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Header) ||
		                                                  context.BindingInfo.BindingSource.CanAcceptDataFrom(ClaimsBindingSource.BindingSource)))
			return new UniversalModelBinder();

		return null;
	}
}