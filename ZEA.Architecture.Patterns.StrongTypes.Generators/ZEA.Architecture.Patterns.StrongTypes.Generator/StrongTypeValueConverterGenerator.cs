using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator;

[Generator]
public class StrongTypeValueConverterGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			return;

		foreach (var recordDeclaration in receiver.CandidateRecords)
		{
			var semanticModel = context.Compilation.GetSemanticModel(recordDeclaration.SyntaxTree);

			if (semanticModel.GetDeclaredSymbol(recordDeclaration) is not INamedTypeSymbol symbol ||
			    !HasGenerateValueConverterAttribute(symbol)) continue;

			var generatedSource = GenerateValueConverterClass(symbol);
			context.AddSource($"{symbol.Name}.g.cs", generatedSource);
		}
	}

	private static bool HasGenerateValueConverterAttribute(ISymbol symbol)
	{
		foreach (var attribute in symbol.GetAttributes())
		{
			Console.WriteLine(attribute.AttributeClass?.ToDisplayString());
			Console.WriteLine(attribute.AttributeClass?.Name);
		}

		return symbol.GetAttributes()
			.Any(attr => attr.AttributeClass?.Name == nameof(GenerateValueConverterAttribute));
	}

	private string GenerateValueConverterClass(INamedTypeSymbol recordSymbol)
	{
		var recordName = recordSymbol.Name;
		var valueType = GetEncapsulatedType(recordSymbol);
		var namespaceName = recordSymbol.ContainingNamespace.ToDisplayString();

		var valueProperty = FindValueProperty(recordSymbol);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (valueProperty == null)
		{
			// No 'Value' property found, skip generation
			return string.Empty;
		}

		// Generate the source code for the ValueConverter class inside the partial class
		return $$"""

		         using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

		         namespace {{namespaceName}};

		         public partial record {{recordName}}
		         {
		             public sealed class {{recordName}}ValueConverter() : ValueConverter<{{recordName}}, {{valueType}}>(
		         		e => e.Value,
		         		e => new(e)
		         	);
		         }
		         """;
	}

	// Helper method to find the 'Value' property in the type hierarchy
	private static IPropertySymbol? FindValueProperty(INamedTypeSymbol? symbol)
	{
		// Traverse up the type hierarchy to find the 'Value' property
		while (symbol != null)
		{
			var valueProperty = symbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == "Value");

			if (valueProperty != null)
			{
				return valueProperty; // Found the 'Value' property
			}

			symbol = symbol.BaseType; // Move to the base type
		}

		return null; // 'Value' property not found
	}

	private static string GetEncapsulatedType(INamedTypeSymbol recordSymbol)
	{
		var baseType = recordSymbol.BaseType;

		return baseType is { TypeArguments.Length: > 0 }
			? baseType.TypeArguments[0].ToDisplayString()
			: "int"; // Fallback if the encapsulated type can't be determined
	}

	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<RecordDeclarationSyntax> CandidateRecords { get; } = [];

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is RecordDeclarationSyntax recordDeclaration &&
			    recordDeclaration.AttributeLists
				    .Any(
					    attrList => attrList.Attributes
						    .Any(attr => attr.Name.ToString().Contains("GenerateValueConverter"))
				    ))
			{
				CandidateRecords.Add(recordDeclaration);
			}
		}
	}
}