using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZEA.Architecture.Pattern.StrongTypes.Generator;

[Generator]
public class StrongTypeConverterGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		// Register a syntax receiver that will be created for each generation pass
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
		{
			return;
		}

		if (receiver.CandidateTypes.Count == 0)
		{
			return;
		}

		// Retrieve the GenerateConvertersAttribute symbol using the corrected helper method
		var generateConvertersAttributeSymbol = FindTypeByName(context.Compilation, "GenerateConvertersAttribute");

		if (generateConvertersAttributeSymbol == null)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.GenerateConvertersAttributeNotFound);
			return;
		}

		// Retrieve symbols for StrongTypeRecord and StrongTypeClass using the corrected helper method
		var strongTypeRecordSymbol = FindTypeByName(context.Compilation, "StrongTypeRecord");
		var strongTypeClassSymbol = FindTypeByName(context.Compilation, "StrongTypeClass");

		if (strongTypeRecordSymbol == null && strongTypeClassSymbol == null)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BaseClassNotFound);
			return;
		}

		foreach (var recordDeclaration in receiver.CandidateTypes)
		{
			var semanticModel = context.Compilation.GetSemanticModel(recordDeclaration.SyntaxTree);

			if (semanticModel.GetDeclaredSymbol(recordDeclaration) is not INamedTypeSymbol symbol)
			{
				context.ReportDiagnostic(DiagnosticDescriptors.SymbolCannotBeDetermined);
				continue;
			}

			// Ensure the class inherits from StrongTypeRecord or StrongTypeClass
			if (!InheritsFromStrongType(symbol, strongTypeRecordSymbol, strongTypeClassSymbol))
			{
				context.ReportDiagnostic(DiagnosticDescriptors.StrongTypeMustExtendBaseClass);
				continue;
			}

			if (!TryGetConvertersFromAttribute(
				    symbol,
				    generateConvertersAttributeSymbol,
				    out var generateValueConverter,
				    out var generateJsonConverter,
				    out var generateTypeConverter
			    ))
			{
				context.ReportDiagnostic(DiagnosticDescriptors.GenerateConvertersAttributeNotFound);
				continue;
			}

			var generatedSource = GenerateConvertersClass(symbol, generateValueConverter, generateJsonConverter, generateTypeConverter);
			context.AddSource($"{symbol.Name}.g.cs", generatedSource);
		}
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

	// Ensure the class inherits from StrongTypeRecord or StrongTypeClass
	private static bool InheritsFromStrongType(
		INamedTypeSymbol symbol,
		INamedTypeSymbol? strongTypeRecordSymbol,
		INamedTypeSymbol? strongTypeClassSymbol)
	{
		var baseType = symbol.BaseType;

		while (baseType != null)
		{
			// Get the original definition if the base type is generic
			var baseTypeDefinition = baseType.IsGenericType ? baseType.OriginalDefinition : baseType;

			if (strongTypeRecordSymbol != null && SymbolEqualityComparer.Default.Equals(baseTypeDefinition, strongTypeRecordSymbol))
				return true;

			if (strongTypeClassSymbol != null && SymbolEqualityComparer.Default.Equals(baseTypeDefinition, strongTypeClassSymbol))
				return true;

			baseType = baseType.BaseType;
		}

		return false;
	}

	// Extract converter options from the GenerateConvertersAttribute
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

	// Generate the selected converters
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
			using ZEA.Serialization.Abstractions.Interfaces;
			using ZEA.Serialization.NewtonsoftJson.Converters;
			"""
		);

		// Write namespace if applicable
		if (!string.IsNullOrEmpty(namespaceName))
		{
			sourceBuilder.AppendLine();
			sourceBuilder.AppendLine($"namespace {namespaceName};");
		}

		// Write the record declaration
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

	// Append the ValueConverter code
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
	/// Appends the TypeConverter code to the source builder.
	/// <param name="recordName">The records name</param>
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
	/// Appends the TypeConverter code to the source builder.
	/// </summary>
	/// <param name="recordName">The name of the record.</param>
	/// <param name="encapsulatedType">The type encapsulated by the record.</param>
	/// <param name="sourceBuilder">The StringBuilder to append the code to.</param>
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
	/// Retrieves the encapsulated type from the StrongTypeRecord or StrongTypeClass.
	/// </summary>
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
			"System.DateTime" => "DateTimeJsonConverter",
			"string" => "StringJsonConverter",
			"bool" => "BoolJsonConverter",
			"System.Guid" => "GuidJsonConverter",
			_ => throw new InvalidOperationException($"No JsonConverter defined for type {encapsulatedType}")
		};
	}

	// Syntax receiver to collect record declarations that have the GenerateConverters attribute
	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<TypeDeclarationSyntax> CandidateTypes { get; } = [];

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			// Check if the node is a class or a record
			if (syntaxNode is TypeDeclarationSyntax typeDeclaration &&
			    (typeDeclaration is ClassDeclarationSyntax || typeDeclaration is RecordDeclarationSyntax) &&
			    typeDeclaration.AttributeLists
				    .Any(
					    attrList => attrList.Attributes
						    .Any(attr => attr.Name.ToString().Contains("GenerateConverters"))
				    ))
			{
				CandidateTypes.Add(typeDeclaration);
			}
		}
	}
}