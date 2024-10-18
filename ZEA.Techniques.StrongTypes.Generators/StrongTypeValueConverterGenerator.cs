using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZEA.Techniques.StrongTypes.Generators;

[Generator]
public class StrongTypeConverterGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Register a syntax provider that filters for classes or records with the GenerateConverters attribute
		var candidateTypes = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: IsCandidateType,
				transform: GetSemanticTarget
			)
			.Where(symbol => symbol != null)!; // Filter out nulls

		// Combine all candidate symbols with the compilation
		var compilationAndTypes = context.CompilationProvider.Combine(candidateTypes.Collect());

		// Register the source output
		context.RegisterSourceOutput(
			compilationAndTypes,
			(
				spc,
				source) =>
			{
				var compilation = source.Left;
				var types = source.Right;

				// Retrieve the GenerateConvertersAttribute symbol
				var generateConvertersAttributeSymbol = FindTypeByName(compilation, "GenerateConvertersAttribute");

				if (generateConvertersAttributeSymbol == null)
				{
					// Report diagnostic if the attribute is not found
					context.ReportDiagnostic(DiagnosticDescriptors.GenerateConvertersAttributeNotFound);
					return;
				}

				// Retrieve symbols for StrongTypeRecord and StrongTypeClass
				var strongTypeRecordSymbol = FindTypeByName(compilation, "StrongTypeRecord");
				var strongTypeClassSymbol = FindTypeByName(compilation, "StrongTypeClass");

				if (strongTypeRecordSymbol == null && strongTypeClassSymbol == null)
				{
					// Report diagnostic if base classes are not found
					context.ReportDiagnostic(DiagnosticDescriptors.BaseClassNotFound);
					return;
				}

				foreach (var typeSymbol in types.Distinct())
				{
					if (typeSymbol is null)
					{
						context.ReportDiagnostic(DiagnosticDescriptors.SymbolCannotBeDetermined);
						continue;
					}

					// Ensure the class inherits from StrongTypeRecord or StrongTypeClass
					if (!InheritsFromStrongType(typeSymbol, strongTypeRecordSymbol, strongTypeClassSymbol))
					{
						var location = typeSymbol.Locations.FirstOrDefault() ?? Location.None;
						var diagnostic = Diagnostic.Create(DiagnosticDescriptors.StrongTypeMustExtendBaseClass, location);
						spc.ReportDiagnostic(diagnostic);
						continue;
					}

					// Extract converter options from the GenerateConvertersAttribute
					if (!TryGetConvertersFromAttribute(
						    typeSymbol,
						    generateConvertersAttributeSymbol,
						    out var generateValueConverter,
						    out var generateJsonConverter,
						    out var generateTypeConverter
					    ))
					{
						var location = typeSymbol.Locations.FirstOrDefault() ?? Location.None;
						var diagnostic = Diagnostic.Create(DiagnosticDescriptors.GenerateConvertersAttributeNotFound, location);
						spc.ReportDiagnostic(diagnostic);
						continue;
					}

					// Generate the converters class
					var generatedSource = GenerateConvertersClass(
						typeSymbol,
						generateValueConverter,
						generateJsonConverter,
						generateTypeConverter
					);
					spc.AddSource($"{typeSymbol.Name}.g.cs", SourceText.From(generatedSource, Encoding.UTF8));
				}
			}
		);
	}

	/// <summary>
	/// Determines if a syntax node is a candidate type (class or record with GenerateConverters attribute).
	/// </summary>
	private static bool IsCandidateType(
		SyntaxNode node,
		CancellationToken cancellationToken)
	{
		return node is TypeDeclarationSyntax typeDeclaration &&
		       (typeDeclaration is ClassDeclarationSyntax || typeDeclaration is RecordDeclarationSyntax) &&
		       typeDeclaration.AttributeLists.Any(
			       attrList =>
				       attrList.Attributes.Any(attr => attr.Name.ToString().Contains("GenerateConverters"))
		       );
	}

	/// <summary>
	/// Transforms a syntax node into a semantic symbol.
	/// </summary>
	private static INamedTypeSymbol? GetSemanticTarget(
		GeneratorSyntaxContext context,
		CancellationToken cancellationToken)
	{
		if (context.Node is TypeDeclarationSyntax typeDeclaration)
		{
			var symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, cancellationToken) as INamedTypeSymbol;
			return symbol;
		}

		return null;
	}

	/// <summary>
	/// Recursively searches the global namespace for a type with the specified name.
	/// </summary>
	private static INamedTypeSymbol? FindTypeByName(
		Compilation compilation,
		string typeName)
	{
		var globalNamespace = compilation.GlobalNamespace;
		var queue = new Queue<INamespaceSymbol>();
		queue.Enqueue(globalNamespace);

		while (queue.Count > 0)
		{
			var currentNamespace = queue.Dequeue();

			foreach (var member in currentNamespace.GetMembers())
			{
				switch (member)
				{
					case INamespaceSymbol namespaceMember:
						queue.Enqueue(namespaceMember);
						break;
					case INamedTypeSymbol typeMember when typeMember.Name == typeName:
						return typeMember;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Ensures the class inherits from StrongTypeRecord or StrongTypeClass
	/// </summary>
	private static bool InheritsFromStrongType(
		INamedTypeSymbol symbol,
		INamedTypeSymbol? strongTypeRecordSymbol,
		INamedTypeSymbol? strongTypeClassSymbol)
	{
		var baseType = symbol.BaseType;

		while (baseType != null)
		{
			var baseTypeDefinition = baseType.IsGenericType ? baseType.OriginalDefinition : baseType;

			if (strongTypeRecordSymbol != null && SymbolEqualityComparer.Default.Equals(baseTypeDefinition, strongTypeRecordSymbol))
				return true;

			if (strongTypeClassSymbol != null && SymbolEqualityComparer.Default.Equals(baseTypeDefinition, strongTypeClassSymbol))
				return true;

			baseType = baseType.BaseType;
		}

		return false;
	}

	/// <summary>
	/// Extract converter options from the GenerateConvertersAttribute
	/// </summary>
	private static bool TryGetConvertersFromAttribute(
		INamedTypeSymbol symbol,
		INamedTypeSymbol generateConvertersAttributeSymbol,
		out bool generateValueConverter,
		out bool generateJsonConverter,
		out bool generateTypeConverter)
	{
		generateValueConverter = false;
		generateJsonConverter = false;
		generateTypeConverter = false;

		foreach (var attribute in symbol.GetAttributes()
			         .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, generateConvertersAttributeSymbol))
			         .Where(attribute => attribute.ConstructorArguments.Length == 3))
		{
			generateValueConverter = attribute.ConstructorArguments[0].Value is true;
			generateJsonConverter = attribute.ConstructorArguments[1].Value is true;
			generateTypeConverter = attribute.ConstructorArguments[2].Value is true;

			return true;
		}

		return false;
	}

	/// <summary>
	/// Generates the converters class based on the provided options
	/// </summary>
	private static string GenerateConvertersClass(
		INamedTypeSymbol typeSymbol,
		bool generateValueConverter,
		bool generateJsonConverter,
		bool generateTypeConverter)
	{
		var typeName = typeSymbol.Name;
		var encapsulatedType = GetEncapsulatedType(typeSymbol);
		var baseConverter = GetBaseJsonConverterType(encapsulatedType);
		var namespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace ? null : typeSymbol.ContainingNamespace.ToDisplayString();

		var typeKind = typeSymbol.IsRecord ? "record" : "class";

		var sourceBuilder = new StringBuilder();

		// Write the usings
		sourceBuilder.AppendLine(
			"""
			using System;
			using System.ComponentModel;
			using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
			using Newtonsoft.Json;
			using ZEA.Serializations.Abstractions.Interfaces;
			using ZEA.Serializations.NewtonsoftJson.Converters;
			"""
		);

		// Write namespace if applicable
		if (!string.IsNullOrEmpty(namespaceName))
		{
			sourceBuilder.AppendLine();
			sourceBuilder.AppendLine($"namespace {namespaceName};");
		}

		// Write the record or class declaration
		sourceBuilder.AppendLine();
		sourceBuilder.AppendLine(
			$$"""
			  public partial {{typeKind}} {{typeName}}
			  {
			      // Automatically generated static Create method
			      public static {{typeName}} Create({{encapsulatedType}} value) => new(value);
			  """
		);

		if (generateValueConverter)
		{
			AppendValueConverter(typeName, encapsulatedType, sourceBuilder);
		}

		if (generateJsonConverter)
		{
			AppendJsonConverter(typeName, encapsulatedType, baseConverter, sourceBuilder);
		}

		if (generateTypeConverter)
		{
			AppendTypeConverter(typeName, encapsulatedType, sourceBuilder);
		}

		sourceBuilder.Append('}');
		return sourceBuilder.ToString();
	}

	/// <summary>
	/// Appends the ValueConverter code
	/// </summary>
	private static void AppendValueConverter(
		string recordName,
		string encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine();
		sourceBuilder.AppendLine(
			$$"""
			      partial class {{recordName}}ValueConverter : ValueConverter<{{recordName}}, {{encapsulatedType}}>
			      {
			          public {{recordName}}ValueConverter() 
			              : base(e => e.Value, e => new {{recordName}}(e)) { }
			      }
			  """
		);
	}

	/// <summary>
	/// Appends the JsonConverter code
	/// </summary>
	private static void AppendJsonConverter(
		string recordName,
		string encapsulatedType,
		string baseConverter,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine();
		sourceBuilder.AppendLine(
			$$"""
			      partial class {{recordName}}NewtonsoftJsonConverter : {{baseConverter}}<{{recordName}}>
			      {
			          protected override {{recordName}} CreateInstance({{encapsulatedType}} value) => new(value);
			          protected override {{encapsulatedType}} GetValue({{recordName}} instance) => instance.Value;
			      }
			  """
		);
	}

	/// <summary>
	/// Appends the TypeConverter code
	/// </summary>
	private static void AppendTypeConverter(
		string recordName,
		string encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine();
		sourceBuilder.AppendLine(
			$$"""
			      partial class {{recordName}}TypeConverter : TypeSafeConverter<{{recordName}}, {{encapsulatedType}}>
			      {
			          protected override {{recordName}} ConvertFromType({{encapsulatedType}} value) => {{recordName}}.Create(value);
			          protected override {{encapsulatedType}} ConvertToType({{recordName}} value) => value.Value;
			      }
			  """
		);
	}

	/// <summary>
	/// Retrieves the encapsulated type from the StrongTypeRecord or StrongTypeClass
	/// </summary>
	private static string GetEncapsulatedType(INamedTypeSymbol recordSymbol)
	{
		var baseType = recordSymbol.BaseType;
		return baseType is { TypeArguments.Length: > 0 }
			? baseType.TypeArguments[0].ToDisplayString()
			: "int"; // Fallback if the encapsulated type can't be determined
	}

	/// <summary>
	/// Determines the appropriate base JsonConverter based on the encapsulated type
	/// </summary>
	private static string GetBaseJsonConverterType(string encapsulatedType)
	{
		return encapsulatedType switch
		{
			"int" => "IntJsonConverter",
			"double" => "DoubleJsonConverter",
			"System.DateTime" => "DateTimeJsonConverter",
			"string" => "StringJsonConverter",
			"bool" => "BoolJsonConverter",
			"System.Guid" => "GuidJsonConverter",
			_ => throw new InvalidOperationException($"No JsonConverter defined for type {encapsulatedType}")
		};
	}
}