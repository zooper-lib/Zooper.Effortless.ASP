using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator.Tests;

public class StrongTypeConverterGeneratorTests(ITestOutputHelper testOutputHelper)
{
	private const string HeightClassSource = """
	                                         using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;
	                                         using ZEA.Architecture.Patterns.StrongTypes.Interfaces;

	                                         [GenerateConverters(generateValueConverter: true, generateJsonConverter: true, generateTypeConverter: true)]
	                                         public partial record IntStrongType(int Value) : StrongTypeRecord<int, IntStrongType>(Value);
	                                         """;

	//[Fact]
	public void GeneratesConvertersForStrongType()
	{
		// Create an instance of the source generator.
		var generator = new StrongTypeConverterGenerator();

		// Source generators should be tested using 'GeneratorDriver'.
		var driver = CSharpGeneratorDriver.Create(generator);

		var syntaxTree = CSharpSyntaxTree.ParseText(HeightClassSource);

		// Create a dummy compilation with a reference to the actual project containing the IntStrongType class and attributes
		var compilation = CSharpCompilation.Create(
			"TestAssembly",
			syntaxTrees: [syntaxTree],
			[
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // Reference to core .NET assemblies
				MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // Reference to core .NET assemblies

				// Reference the assemblies where StrongTypeRecord and GenerateConvertersAttribute are defined
				MetadataReference.CreateFromFile(typeof(StrongTypeRecord<,>).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(GenerateConvertersAttribute).Assembly.Location),

				// Add reference to the assembly where your IntStrongType class is defined
				//MetadataReference.CreateFromFile(typeof(IntStrongType).Assembly.Location)
			],
			new(OutputKind.DynamicallyLinkedLibrary)
		);

		// Log attributes found on the IntStrongType class
		var semanticModel = compilation.GetSemanticModel(syntaxTree);
		var heightSymbol =
			semanticModel.GetDeclaredSymbol(syntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First());

		foreach (var attribute in heightSymbol!.GetAttributes())
		{
			testOutputHelper.WriteLine($"Found attribute: {attribute.AttributeClass?.ToDisplayString()}");
		}

		// Run the generator and update the compilation.
		driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

		// Output diagnostics to the test output.
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

		// Assert that the expected generated file exists
		Assert.Contains(generatedFiles, file => file.EndsWith(".g.cs"));

		// Optionally, verify the generated content.
		var generatedCode = newCompilation.SyntaxTrees
			.FirstOrDefault(t => t.FilePath.EndsWith("IntStrongType.g.cs"))
			?.ToString();

		Assert.NotNull(generatedCode);
		Assert.Contains("public sealed class HeightValueConverter", generatedCode);
		Assert.Contains("public sealed class HeightJsonConverter", generatedCode);
		Assert.Contains("public sealed class HeightTypeConverter", generatedCode);
	}
}