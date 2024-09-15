using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZEA.Serialization.NewtonsoftJson.Converters;

namespace ZEA.Architecture.Patterns.StrongTypes.Generator;

[Generator]
public sealed class StrongTypeConverterGenerator : ISourceGenerator
{
	private readonly static Dictionary<SpecialType, Type> ConverterTypeMap = new()
	{
		{
			SpecialType.System_Int32, typeof(IntJsonConverter<>)
		},
		{
			SpecialType.System_Int64, typeof(LongJsonConverter<>)
		},
		{
			SpecialType.System_Double, typeof(DoubleJsonConverter<>)
		},
		{
			SpecialType.System_String, typeof(StringJsonConverter<>)
		},
		{
			SpecialType.System_Boolean, typeof(BoolJsonConverter<>)
		}
	};

	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		context.ReportDiagnostic(
			Diagnostic.Create(
				new(
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
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					new(
						"GEN002",
						"Generator Debug",
						"Syntax receiver is null",
						"Generator",
						DiagnosticSeverity.Warning,
						true
					),
					Location.None
				)
			);

			return;
		}

		if (receiver.CandidateRecords.Count == 0)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					new(
						"GEN002",
						"Generator Debug",
						"Syntax receiver has no candidate records",
						"Generator",
						DiagnosticSeverity.Warning,
						true
					),
					Location.None
				)
			);

			return;
		}

		foreach (var recordDeclaration in receiver.CandidateRecords)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					new(
						"GEN003",
						"Generator Debug",
						"Record found",
						"Generator",
						DiagnosticSeverity.Info,
						true
					),
					Location.None
				)
			);

			var semanticModel = context.Compilation.GetSemanticModel(recordDeclaration.SyntaxTree);

			if (semanticModel.GetDeclaredSymbol(recordDeclaration) is not INamedTypeSymbol symbol)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						new(
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
					new(
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
						new(
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
				    context,
				    out var generateValueConverter,
				    out var generateJsonConverter,
				    out var generateTypeConverter
			    ))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						new(
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
					new(
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

			var generatedSource = GenerateConvertersClass(
				symbol,
				generateValueConverter,
				generateJsonConverter,
				generateTypeConverter,
				context
			);
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
		GeneratorExecutionContext context,
		out bool generateValueConverter,
		out bool generateJsonConverter,
		out bool generateTypeConverter)
	{
		generateValueConverter = false;
		generateJsonConverter = false;
		generateTypeConverter = false;

		// Get the symbol of the GenerateConvertersAttribute from the entire compilation
		var generateConvertersAttributeSymbol = context.Compilation.GetTypeByMetadataName(
			"ZEA.Architecture.Patterns.StrongTypes.Generator.Attributes.GenerateConvertersAttribute"
		);

		if (generateConvertersAttributeSymbol == null)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					new(
						"GEN002",
						"Generator Debug",
						"GenerateConvertersAttribute not found",
						"Generator",
						DiagnosticSeverity.Warning,
						true
					),
					Location.None
				)
			);

			return false; // Attribute not found
		}

		// Perform the actual comparison using SymbolEqualityComparer
		foreach (var attribute in symbol.GetAttributes())
		{
			var attributeName = attribute.AttributeClass?.ToDisplayString();
			var expectedName = generateConvertersAttributeSymbol.ToDisplayString();

			context.ReportDiagnostic(
				Diagnostic.Create(
					new DiagnosticDescriptor(
						"GEN008",
						"Generator Debug",
						$"Comparing attribute: {attributeName} with expected: {expectedName}",
						"Generator",
						DiagnosticSeverity.Info,
						true
					),
					Location.None
				)
			);

			if (attributeName == expectedName && attribute.ConstructorArguments.Length == 3)
			{
				generateValueConverter = attribute.ConstructorArguments[0].Value as bool? ?? false;
				generateJsonConverter = attribute.ConstructorArguments[1].Value as bool? ?? false;
				generateTypeConverter = attribute.ConstructorArguments[2].Value as bool? ?? false;

				return true; // Return if the attribute was successfully matched and processed
			}
		}

		return false;
	}

	// Generate the selected converters
	private static string GenerateConvertersClass(
		INamedTypeSymbol recordSymbol,
		bool generateValueConverter,
		bool generateJsonConverter,
		bool generateTypeConverter,
		GeneratorExecutionContext context)
	{
		var recordName = recordSymbol.Name;
		var encapsulatedType = GetEncapsulatedType(recordSymbol);
		var baseConverter = GetBaseJsonConverterType(encapsulatedType, context.Compilation);
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
			      public static {{recordName}} Create({{encapsulatedType.ToDisplayString()}} value) => new(value);
			      
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
		ITypeSymbol encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      partial class {{recordName}}ValueConverter : ValueConverter<{{recordName}}, {{encapsulatedType}}>
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
		ITypeSymbol encapsulatedType,
		INamedTypeSymbol baseConverter,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      partial class {{recordName}}NewtonsoftJsonConverter : {{baseConverter}}<{{recordName}}>
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
		ITypeSymbol encapsulatedType,
		StringBuilder sourceBuilder)
	{
		sourceBuilder.Append(
			$$"""
			  
			      partial class {{recordName}}TypeConverter : TypeSafeConverter<{{recordName}}, {{encapsulatedType}}>
			      {
			          protected override {{recordName}} ConvertFromType({{encapsulatedType}} value) => {{recordName}}.Create(value);
			          protected override {{encapsulatedType}} ConvertToType({{recordName}} value) => value.Value;
			      }

			  """
		);
	}

	// Get the encapsulated type from the StrongTypeRecord or StrongTypeClass
	private static ITypeSymbol GetEncapsulatedType(INamedTypeSymbol recordSymbol)
	{
		var baseType = recordSymbol.BaseType ??
		               throw new InvalidOperationException($"Record {recordSymbol.Name} does not have a base type.");

		if (baseType.TypeArguments.Length > 0)
		{
			return baseType.TypeArguments[0];
		}

		throw new InvalidOperationException($"Could not determine encapsulated type for {recordSymbol.Name}");
	}

	private static INamedTypeSymbol GetConverterTypeSymbol(Compilation compilation, Type converterGenericType)
	{
		var converterTypeName = converterGenericType.FullName;

		if (converterGenericType.IsGenericTypeDefinition)
		{
			// Handle generic type definition (e.g., IntJsonConverter<>)
			var backtickIndex = converterTypeName.IndexOf('`');
			if (backtickIndex > 0)
			{
				converterTypeName = converterTypeName.Substring(0, backtickIndex);
			}
		}

		converterTypeName = converterTypeName.Replace('+', '.'); // Handle nested types if needed

		// Get the type symbol from the compilation
		var converterTypeSymbol = compilation.GetTypeByMetadataName(converterTypeName);
		if (converterTypeSymbol == null)
		{
			throw new InvalidOperationException($"Converter type {converterTypeName} not found in the compilation.");
		}

		return converterTypeSymbol;
	}

	private static INamedTypeSymbol? GetConverterForSpecialType(ITypeSymbol encapsulatedType, Compilation compilation)
	{
		if (ConverterTypeMap.TryGetValue(encapsulatedType.SpecialType, out var converterType))
		{
			return GetConverterTypeSymbol(compilation, converterType);
		}

		return null; // Type not found in the map
	}

	private static INamedTypeSymbol GetBaseJsonConverterType(
		ITypeSymbol encapsulatedType,
		Compilation compilation)
	{
		// Handle nullable types
		if (encapsulatedType is INamedTypeSymbol
		    {
			    IsGenericType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T
		    } namedTypeSymbol)
		{
			encapsulatedType = namedTypeSymbol.TypeArguments[0];
		}

		INamedTypeSymbol? converterTypeSymbol = null;

		// Obtain type symbols for types not covered by SpecialType
		var guidType = compilation.GetTypeByMetadataName("System.Guid");
		var dateTimeOffsetType = compilation.GetTypeByMetadataName("System.DateTimeOffset");
		var uint16Type = compilation.GetTypeByMetadataName("System.UInt16");
		var uint32Type = compilation.GetTypeByMetadataName("System.UInt32");
		var uint64Type = compilation.GetTypeByMetadataName("System.UInt64");

		// Map types to converter type symbols
		if (SymbolEqualityComparer.Default.Equals(encapsulatedType, guidType))
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(GuidJsonConverter<>));
		}
		else if (SymbolEqualityComparer.Default.Equals(encapsulatedType, dateTimeOffsetType))
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(DateTimeOffsetJsonConverter<>));
		}
		else if (SymbolEqualityComparer.Default.Equals(encapsulatedType, uint16Type))
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(UInt16JsonConverter<>));
		}
		else if (SymbolEqualityComparer.Default.Equals(encapsulatedType, uint32Type))
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(UInt32JsonConverter<>));
		}
		else if (SymbolEqualityComparer.Default.Equals(encapsulatedType, uint64Type))
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(UInt64JsonConverter<>));
		}
		else if (encapsulatedType.TypeKind == TypeKind.Enum)
		{
			converterTypeSymbol = GetConverterTypeSymbol(compilation, typeof(EnumJsonConverter<>));
		}
		else
		{
			// Handle types covered by SpecialType
			converterTypeSymbol = GetConverterForSpecialType(encapsulatedType, compilation);
		}

		if (converterTypeSymbol == null)
		{
			throw new InvalidOperationException($"No JsonConverter defined for type {encapsulatedType.ToDisplayString()}");
		}

		return converterTypeSymbol;
	}

	// Syntax receiver to collect record declarations that have the GenerateConverters attribute
	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<RecordDeclarationSyntax> CandidateRecords { get; } = [];

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