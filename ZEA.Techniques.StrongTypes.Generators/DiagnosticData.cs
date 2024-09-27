using Microsoft.CodeAnalysis;

namespace ZEA.Techniques.StrongTypes.Generators;

public sealed record DiagnosticData(
	string Id,
	string Title,
	string Message,
	string Category,
	DiagnosticSeverity Severity,
	bool IsEnabledByDefault = true);

public static class DiagnosticDescriptors
{
	public readonly static DiagnosticData StrongTypeRecordMustBePartial = new(
		"STRONGTYPE001",
		"Is not partial",
		"StrongTypeRecord must be partial",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeMustExtendBaseClass = new(
		"STRONGTYPE002",
		"Does not extend base class",
		"Symbol does not inherit from StrongTypeRecord or StrongTypeClass",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeRecordMustHaveOneConstructor = new(
		"STRONGTYPE003",
		"No constructor",
		"StrongTypeRecord must have one constructor",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeRecordMustHaveOneConstructorWithOneParameter = new(
		"STRONGTYPE004",
		"Invalid constructor",
		"StrongTypeRecord must have one constructor with one parameter",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValue = new(
		"STRONGTYPE005",
		"Invalid constructor",
		"StrongTypeRecord must have one constructor with one parameter of type Value",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBase = new(
		"STRONGTYPE006",
		"Invalid constructor",
		"StrongTypeRecord must have one constructor with one parameter of type Value and call base",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBaseWithSameParameter =
		new(
			"STRONGTYPE007",
			"Invalid constructor",
			"StrongTypeRecord must have one constructor with one parameter of type Value and call base with same parameter",
			"StrongTypes",
			DiagnosticSeverity.Error
		);

	public readonly static DiagnosticData
		StrongTypeRecordMustHaveOneConstructorWithOneParameterOfTypeValueAndCallBaseWithSameParameterName =
			new(
				"STRONGTYPE008",
				"Invalid constructor",
				"StrongTypeRecord must have one constructor with one parameter of type Value and call base with same parameter name",
				"StrongTypes",
				DiagnosticSeverity.Error
			);

	public readonly static DiagnosticData GenerateConvertersAttributeNotFound = new(
		"STRONGTYPE009",
		"Attribute not found",
		"GenerateConverters attribute not found",
		"StrongTypes",
		DiagnosticSeverity.Warning
	);

	public readonly static DiagnosticData BaseClassNotFound = new(
		"STRONGTYPE010",
		"Base class not found",
		"Neither StrongTypeRecord nor StrongTypeClass found in compilation",
		"StrongTypes",
		DiagnosticSeverity.Error
	);

	public readonly static DiagnosticData SymbolCannotBeDetermined = new(
		"STRONGTYPE011",
		"Symbol cannot be determined",
		"Record symbol could not be determined",
		"StrongTypes",
		DiagnosticSeverity.Warning
	);
}