using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator;

[Generator]
public class StrongTypeConverterGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		context.ReportDiagnostic(
			Diagnostic.Create(
				new DiagnosticDescriptor(
					"GEN001",
					"Generator Debug",
					"Source generator running",
					"Generator",
					DiagnosticSeverity.Info,
					true
				),
				Location.None
			)
		);

		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			return;

		foreach (var recordDeclaration in receiver.CandidateRecords)
		{
			var semanticModel = context.Compilation.GetSemanticModel(recordDeclaration.SyntaxTree);

			if (semanticModel.GetDeclaredSymbol(recordDeclaration) is not INamedTypeSymbol symbol)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						new DiagnosticDescriptor(
							"GEN002",
							"Generator Debug",
							"Record symbol could not be determined",
							"Generator",
							DiagnosticSeverity.Warning,
							true
						),
						Location.None
					)
				);
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					new DiagnosticDescriptor(
						"GEN003",
						"Generator Debug",
						$"Found record: {symbol.Name}",
						"Generator",
						DiagnosticSeverity.Info,
						true
					),
					Location.None
				)
			);

			// Ensure the class inherits from StrongTypeRecord or StrongTypeClass
			if (!InheritsFromStrongType(symbol))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						new DiagnosticDescriptor(
							"GEN002",
							"Generator Debug",
							$"{symbol.Name} does not inherit from StrongTypeRecord or StrongTypeClass",
							"Generator",
							DiagnosticSeverity.Warning,
							true
						),
						Location.None
					)
				);
				continue;
			}

			if (!TryGetConvertersFromAttribute(
				    symbol,
				    out var generateValueConverter,
				    out var generateJsonConverter,
				    out var generateTypeConverter
			    ))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						new DiagnosticDescriptor(
							"GEN006",
							"Generator Debug",
							$"{symbol.Name} does not have GenerateConverters attribute",
							"Generator",
							DiagnosticSeverity.Warning,
							true
						),
						Location.None
					)
				);
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					new DiagnosticDescriptor(
						"GEN003",
						"Generator Debug",
						$"Generating converters for {symbol.Name}",
						"Generator",
						DiagnosticSeverity.Info,
						true
					),
					Location.None
				)
			);

			var generatedSource = GenerateConvertersClass(symbol, generateValueConverter, generateJsonConverter, generateTypeConverter);
			context.AddSource($"{symbol.Name}.g.cs", generatedSource);
		}
	}

	// Ensure the class inherits from StrongTypeRecord or StrongTypeClass
	private static bool InheritsFromStrongType(INamedTypeSymbol symbol)
	{
		var baseType = symbol.BaseType;

		while (baseType != null)
		{
			if (baseType.Name is "StrongTypeRecord" or "StrongTypeClass")
			{
				return true;
			}

			baseType = baseType.BaseType;
		}

		return false;
	}

	// Extract converter options from the GenerateConvertersAttribute
	private static bool TryGetConvertersFromAttribute(
		INamedTypeSymbol symbol,
		out bool generateValueConverter,
		out bool generateJsonConverter,
		out bool generateTypeConverter)
	{
		generateValueConverter = false;
		generateJsonConverter = false;
		generateTypeConverter = false;

		foreach (var attribute in symbol.GetAttributes()
			         .Where(attribute => attribute.AttributeClass?.Name == nameof(GenerateConvertersAttribute))
			         .Where(attribute => attribute.ConstructorArguments.Length == 3))
		{
			generateValueConverter = attribute.ConstructorArguments[0].Value as bool? ?? false;
			generateJsonConverter = attribute.ConstructorArguments[1].Value as bool? ?? false;
			generateTypeConverter = attribute.ConstructorArguments[2].Value as bool? ?? false;
			return true;
		}

		return false;
	}

	// Generate the selected converters
	private static string GenerateConvertersClass(
		INamedTypeSymbol recordSymbol,
		bool generateValueConverter,
		bool generateJsonConverter,
		bool generateTypeConverter)
	{
		var recordName = recordSymbol.Name;
		var encapsulatedType = GetEncapsulatedType(recordSymbol);
		var baseConverter = GetBaseJsonConverterType(encapsulatedType);
		var namespaceName = recordSymbol.ContainingNamespace.ToDisplayString();

		var sourceBuilder = new StringBuilder(
			$$"""
			  using System;
			  using System.ComponentModel;
			  using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
			  using Newtonsoft.Json;
			  using ZEA.Serialization.Abstractions.Interfaces;
			  using ZEA.Serialization.NewtonsoftJson.Converters;

			  namespace {{namespaceName}};

			  public partial record {{recordName}}
			  {
			      // Automatically generated static Create method
			      public static {{recordName}} Create({{encapsulatedType}} value) => new(value);
			      
			  """
		);

		if (generateValueConverter)
		{
			AppendValueConverter(recordName, encapsulatedType, sourceBuilder);
		}

		if (generateJsonConverter)
		{
			AppendJsonConverter(recordName, encapsulatedType, baseConverter, sourceBuilder);
		}

		if (generateTypeConverter)
		{
			AppendTypeConverter(recordName, encapsulatedType, sourceBuilder);
		}

		sourceBuilder.Append("}\n");
		return sourceBuilder.ToString();
	}

	// Append the ValueConverter code
	private static void AppendValueConverter(
		string recordName,
		string encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      public sealed class {{recordName}}ValueConverter : ValueConverter<{{recordName}}, {{encapsulatedType}}>
			      {
			          public {{recordName}}ValueConverter() 
			              : base(e => e.Value, e => new {{recordName}}(e)) { }
			      }

			  """
		);
	}

	// Append the JsonConverter code based on the encapsulated type
	private static void AppendJsonConverter(
		string recordName,
		string encapsulatedType,
		string baseConverter,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      public sealed class {{recordName}}JsonConverter : {{baseConverter}}<{{recordName}}>
			      {
			          protected override {{recordName}} CreateInstance({{encapsulatedType}} value) => new(value);
			          protected override {{encapsulatedType}} GetValue({{recordName}} instance) => instance.Value;
			      }

			  """
		);
	}

	// Append the TypeConverter code with correct inheritance from TypeSafeConverter
	private static void AppendTypeConverter(
		string recordName,
		string encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      public sealed class {{recordName}}TypeConverter : TypeSafeConverter<{{recordName}}, {{encapsulatedType}}>
			      {
			          protected override {{recordName}} ConvertFromType({{encapsulatedType}} value) => {{recordName}}.Create(value);
			          protected override {{encapsulatedType}} ConvertToType({{recordName}} value) => value.Value;
			      }

			  """
		);
	}

	// Get the encapsulated type from the StrongTypeRecord or StrongTypeClass
	private static string GetEncapsulatedType(INamedTypeSymbol recordSymbol)
	{
		var baseType = recordSymbol.BaseType;
		return baseType is { TypeArguments.Length: > 0 }
			? baseType.TypeArguments[0].ToDisplayString()
			: "int"; // Fallback if the encapsulated type can't be determined
	}

	// Determine the appropriate base JsonConverter based on the encapsulated type
	private static string GetBaseJsonConverterType(string encapsulatedType)
	{
		return encapsulatedType switch
		{
			"int" => "IntJsonConverter",
			"double" => "DoubleJsonConverter",
			"DateTime" => "DateTimeJsonConverter",
			_ => throw new InvalidOperationException($"No JsonConverter defined for type {encapsulatedType}")
		};
	}

	// Syntax receiver to collect record declarations that have the GenerateConverters attribute
	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<RecordDeclarationSyntax> CandidateRecords { get; } = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is RecordDeclarationSyntax recordDeclaration &&
			    recordDeclaration.AttributeLists
				    .Any(
					    attrList => attrList.Attributes
						    .Any(attr => attr.Name.ToString().Contains("GenerateConverters"))
				    ))
			{
				CandidateRecords.Add(recordDeclaration);
			}
		}
	}
}