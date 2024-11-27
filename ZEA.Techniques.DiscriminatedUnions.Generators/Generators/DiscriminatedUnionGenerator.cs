using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZEA.Techniques.DiscriminatedUnions.Generators.Generators;

[Generator]
public class DiscriminatedUnionGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var classDeclarations = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: (
					node,
					_) => node is ClassDeclarationSyntax,
				transform: (
					ctx,
					_) => GetSemanticTargetForGeneration(ctx)
			)
			.Where(m => m is not null)!;

		var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

		context.RegisterSourceOutput(
			compilationAndClasses,
			(
				spc,
				source) => Execute(spc, source.Left, source.Right, spc.CancellationToken)
		);
	}

	private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
	{
		var classDecl = (ClassDeclarationSyntax)context.Node;

		if (classDecl.AttributeLists.SelectMany(al => al.Attributes)
		    .Any(attr => attr.Name.ToString() == "DiscriminatedUnion"))
		{
			return classDecl;
		}

		return null;
	}

	private void Execute(
		SourceProductionContext context,
		Compilation compilation,
		ImmutableArray<ClassDeclarationSyntax> classDeclarations,
		CancellationToken cancellationToken)
	{
		if (classDeclarations.IsDefaultOrEmpty)
		{
			return;
		}

		var model = compilation.GetSemanticModel(classDeclarations.First().SyntaxTree);

		foreach (var classDecl in classDeclarations)
		{
			var classSymbol = model.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
			var variants = classSymbol.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(
					method => method.GetAttributes()
						.Any(attr => attr.AttributeClass.Name == "Variant")
				)
				.ToList();

			var source = GenerateSource(classSymbol, variants);
			context.AddSource($"{classSymbol.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
		}
	}

	private string GenerateSource(
		INamedTypeSymbol classSymbol,
		List<IMethodSymbol> variants)
	{
		var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
		var className = classSymbol.Name;

		var sb = new StringBuilder();
		sb.AppendLine("using OneOf;");
		sb.AppendLine("using System;");
		sb.AppendLine();

		sb.AppendLine($"namespace {namespaceName}");
		sb.AppendLine("{");

		GenerateClassHeader(sb, className, variants);
		sb.AppendLine("    {");

		GenerateConstructor(sb, className, variants);
		sb.AppendLine();

		GenerateVariantMethods(sb, className, variants);
		sb.AppendLine();

		GenerateVariantClasses(sb, className, variants);
		sb.AppendLine();

		sb.AppendLine("    }");
		sb.AppendLine("}");

		return sb.ToString();
	}

	private void GenerateClassHeader(
		StringBuilder sb,
		string className,
		List<IMethodSymbol> variants)
	{
		var variantTypeNames = variants.Select(v => $"{className}.{v.Name}Variant").ToList();
		sb.AppendLine($"    public partial class {className} : OneOfBase<{string.Join(", ", variantTypeNames)}>");
	}

	private void GenerateConstructor(
		StringBuilder sb,
		string className,
		List<IMethodSymbol> variants)
	{
		var variantTypeNames = variants.Select(v => $"{className}.{v.Name}Variant").ToList();
		sb.AppendLine($"        private {className}(OneOf<{string.Join(", ", variantTypeNames)}> value) : base(value) {{ }}");
	}

	private void GenerateVariantMethods(
		StringBuilder sb,
		string className,
		List<IMethodSymbol> variants)
	{
		foreach (var variant in variants)
		{
			var variantName = variant.Name;
			var parameters = string.Join(", ", variant.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
			var args = string.Join(", ", variant.Parameters.Select(p => p.Name));
			var variantTypeName = $"{variantName}Variant";
			var methodSignature = $"public static partial {className} {variantName}({parameters})";
			var newVariantInstance = string.IsNullOrEmpty(args)
				? $"new {variantTypeName}()"
				: $"new {variantTypeName}({args})";

			sb.AppendLine($"        {methodSignature} => new {className}({newVariantInstance});");
		}
	}

	private void GenerateVariantClasses(
		StringBuilder sb,
		string className,
		List<IMethodSymbol> variants)
	{
		foreach (var variant in variants)
		{
			var variantName = variant.Name;
			var variantTypeName = $"{variantName}Variant";
			var parameters = variant.Parameters;
			var fields = parameters.Select(p => $"            public {p.Type.ToDisplayString()} {FirstCharToUpper(p.Name)} {{ get; }}")
				.ToList();
			var ctorParameters = string.Join(", ", parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
			var assignments = parameters.Select(p => $"                this.{FirstCharToUpper(p.Name)} = {p.Name};").ToList();

			// Make the variant classes public
			sb.AppendLine($"        public class {variantTypeName}");
			sb.AppendLine("        {");

			if (parameters.Any())
			{
				// Make the constructor internal or private
				sb.AppendLine($"            internal {variantTypeName}({ctorParameters})");
				sb.AppendLine("            {");

				foreach (var assignment in assignments)
				{
					sb.AppendLine(assignment);
				}

				sb.AppendLine("            }");
				sb.AppendLine();

				foreach (var field in fields)
				{
					sb.AppendLine(field);
				}
			}
			else
			{
				sb.AppendLine($"            internal {variantTypeName}() {{ }}");
			}

			sb.AppendLine("        }");
			sb.AppendLine();
		}
	}

	// Helper methods
	private string FirstCharToLower(string input) =>
		string.IsNullOrEmpty(input) ? input : char.ToLower(input[0]) + input.Substring(1);

	private string FirstCharToUpper(string input) =>
		string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input.Substring(1);
}