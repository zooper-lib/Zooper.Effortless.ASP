using Microsoft.CodeAnalysis;

namespace ZEA.Techniques.StrongTypes.Generators;

public static class DiagnosticDescriptors
{
	public readonly static DiagnosticDescriptor StrongTypeRecordMustBePartial = new DiagnosticDescriptor(
		id: "STRONGTYPE001",
		title: "Is not partial",
		messageFormat: "StrongTypeRecord must be partial",
		category: "StrongTypes",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeMustExtendBaseClass = new DiagnosticDescriptor(
		id: "STRONGTYPE002",
		title: "Does not extend base class",
		messageFormat: "Symbol does not inherit from StrongTypeRecord or StrongTypeClass",
		category: "StrongTypes",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeRecordMustHaveOneConstructor = new DiagnosticDescriptor(
		id: "STRONGTYPE003",
		title: "No constructor",
		messageFormat: "StrongTypeRecord must have one constructor",
		category: "StrongTypes",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeRecordMustHaveOneConstructorWithOneParameter = new(
		id: "STRONGTYPE004",
		title: "Invalid constructor",
		messageFormat: "StrongTypeRecord must have one constructor with one parameter",
		category: "StrongTypes",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValue = new(
		id: "STRONGTYPE005",
		title: "Invalid constructor",
		messageFormat: "StrongTypeRecord must have one constructor with one parameter of type Value",
		category: "StrongTypes",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBase = new(
		"STRONGTYPE006",
		"Invalid constructor",
		"StrongTypeRecord must have one constructor with one parameter of type Value and call base",
		"StrongTypes",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBaseWithSameParameter =
		new(
			"STRONGTYPE007",
			"Invalid constructor",
			"StrongTypeRecord must have one constructor with one parameter of type Value and call base with same parameter",
			"StrongTypes",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true
		);

	public readonly static DiagnosticDescriptor
		StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBaseWithSameParameterName =
			new(
				"STRONGTYPE008",
				"Invalid constructor",
				"StrongTypeRecord must have one constructor with one parameter of type Value and call base with same parameter name",
				"StrongTypes",
				DiagnosticSeverity.Error,
				isEnabledByDefault: true
			);

	public readonly static DiagnosticDescriptor GenerateConvertersAttributeNotFound = new(
		"STRONGTYPE009",
		"Attribute not found",
		"GenerateConverters attribute not found",
		"StrongTypes",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor BaseClassNotFound = new(
		"STRONGTYPE010",
		"Base class not found",
		"Neither StrongTypeRecord nor StrongTypeClass found in compilation",
		"StrongTypes",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public readonly static DiagnosticDescriptor SymbolCannotBeDetermined = new(
		"STRONGTYPE011",
		"Symbol cannot be determined",
		"Record symbol could not be determined",
		"StrongTypes",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true
	);
}