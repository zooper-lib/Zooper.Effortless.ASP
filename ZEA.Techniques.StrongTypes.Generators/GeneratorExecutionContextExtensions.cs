using Microsoft.CodeAnalysis;

namespace ZEA.Techniques.StrongTypes.Generators;

public static class GeneratorExecutionContextExtensions
{
	public static void ReportDiagnostic(
		this GeneratorExecutionContext context,
		DiagnosticData data)
	{
		Diagnostic.Create(
			new(
				data.Id,
				data.Title,
				data.Message,
				data.Category,
				data.Severity,
				true
			),
			Location.None
		);
	}
}