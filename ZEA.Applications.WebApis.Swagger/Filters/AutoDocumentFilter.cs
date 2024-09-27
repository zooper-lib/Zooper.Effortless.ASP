using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZEA.Applications.WebApis.Swagger.Filters;

// ReSharper disable once ClassNeverInstantiated.Global
public class AutoDocumentFilter(IDocumentFilter filter) : IDocumentFilter
{
	public void Apply(
		OpenApiDocument swaggerDoc,
		DocumentFilterContext context)
	{
		filter.Apply(
			swaggerDoc,
			context
		);
	}
}