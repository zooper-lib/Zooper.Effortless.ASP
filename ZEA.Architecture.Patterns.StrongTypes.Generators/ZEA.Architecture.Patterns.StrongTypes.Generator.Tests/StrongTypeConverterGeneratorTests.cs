using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Tests;

public class StrongTypeConverterGeneratorTests(ITestOutputHelper testOutputHelper)
{
	private const string StrongTypeSource = """
	                                        
	                                                using System;
	                                                using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
	                                        
	                                                [GenerateConverters]
	                                                public partial record Height(int Value) : StrongTypeRecord<int, Height>;
	                                            
	                                        """;

	[Fact]
	public void GeneratesConvertersForStrongType()
	{
		// Create an instance of the source generator.
		var generator = new StrongTypeConverterGenerator();

		// Source generators should be tested using 'GeneratorDriver'.
		var driver = CSharpGeneratorDriver.Create(generator);

		// Create a dummy compilation with the strong type source.
		var compilation = CSharpCompilation.Create(
			"TestAssembly",
			new[]
			{
				CSharpSyntaxTree.ParseText(StrongTypeSource)
			},
			new[]
			{
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(StrongTypeRecord<,>).Assembly.Location)
			},
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);

		// Run the generator and update the compilation.
		driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

		// Print any diagnostics emitted by the generator.
		foreach (var diagnostic in diagnostics)
		{
			testOutputHelper.WriteLine(diagnostic.ToString());
		}

		// Filter to only include errors or warnings.
		var errorDiagnostics = diagnostics.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning);

		// Ensure there are no errors or warnings.
		Assert.Empty(errorDiagnostics);

		// Retrieve all generated files.
		var generatedFiles = newCompilation.SyntaxTrees
			.Select(t => Path.GetFileName(t.FilePath))
			.ToArray();

		// Verify that the correct files were generated.
		Assert.Contains("Height.g.cs", generatedFiles);

		// Optionally, verify the generated content.
		var generatedCode = newCompilation.SyntaxTrees
			.FirstOrDefault(t => t.FilePath.EndsWith("Height.g.cs"))
			?.ToString();

		Assert.NotNull(generatedCode);
		Assert.Contains("public sealed class HeightValueConverter", generatedCode);
		Assert.Contains("public sealed class HeightJsonConverter", generatedCode);
		Assert.Contains("public sealed class HeightTypeConverter", generatedCode);
	}
}