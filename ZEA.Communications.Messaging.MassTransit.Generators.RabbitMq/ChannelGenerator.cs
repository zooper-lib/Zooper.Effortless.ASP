using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZEA.Communications.Messaging.MassTransit.Generators.RabbitMq;

[Generator]
public sealed class ChannelGenerator : IIncrementalGenerator
{
	private const string FileName = "MassTransitChannelRegistration";
	private const string Namespace = "ZEA.MassTransit.RabbitMq.Generated";
	private const string ClassName = "MassTransitChannelRegistration";
	private const string MethodName = "ConfigureChannels";
	public const string ChannelAttributeName = "ZEA.Communications.Messaging.MassTransit.Attributes.ChannelAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Register a syntax provider that filters for class and record declarations with ChannelAttribute
		var channelTypes = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: IsSyntaxTargetForGeneration, // Filter syntax nodes
				transform: GetSemanticTargetForGeneration // Transform to semantic symbols
			)
			.Where(static classSymbol => classSymbol != null)!; // Filter out nulls

		// Combine the compilation with the collected channel symbols
		var compilationAndChannels = context.CompilationProvider.Combine(channelTypes.Collect());

		// Register the source output
		context.RegisterSourceOutput(
			compilationAndChannels,
			(
				spc,
				source) => Execute(source.Left, source.Right, spc)
		);
	}

	/// <summary>
	/// Predicate to identify candidate classes or records with ChannelAttribute.
	/// </summary>
	private static bool IsSyntaxTargetForGeneration(
		SyntaxNode node,
		CancellationToken cancellationToken)
	{
		return node is ClassDeclarationSyntax classDeclaration &&
		       classDeclaration.AttributeLists.Count > 0 ||
		       node is RecordDeclarationSyntax recordDeclaration &&
		       recordDeclaration.AttributeLists.Count > 0;
	}

	/// <summary>
	/// Transforms a syntax node into a semantic symbol if it has ChannelAttribute.
	/// </summary>
	private static INamedTypeSymbol? GetSemanticTargetForGeneration(
		GeneratorSyntaxContext context,
		CancellationToken cancellationToken)
	{
		// Determine if the node is a class or record declaration
		if (context.Node is ClassDeclarationSyntax classDeclaration)
		{
			var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) as INamedTypeSymbol;
			return HasChannelAttribute(classSymbol);
		}
		else if (context.Node is RecordDeclarationSyntax recordDeclaration)
		{
			var recordSymbol = context.SemanticModel.GetDeclaredSymbol(recordDeclaration, cancellationToken) as INamedTypeSymbol;
			return HasChannelAttribute(recordSymbol);
		}

		return null;
	}

	/// <summary>
	/// Checks if the symbol has the ChannelAttribute.
	/// </summary>
	private static INamedTypeSymbol? HasChannelAttribute(INamedTypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		foreach (var attribute in symbol.GetAttributes())
		{
			if (attribute.AttributeClass == null)
				continue;

			// Use fully qualified string name to identify the attribute
			if (attribute.AttributeClass.ToDisplayString() == ChannelAttributeName)
			{
				return symbol;
			}
		}

		return null;
	}

	/// <summary>
	/// Executes the generation logic.
	/// </summary>
	private static void Execute(
		Compilation compilation,
		ImmutableArray<INamedTypeSymbol?> channels,
		SourceProductionContext context)
	{
		if (channels.IsDefaultOrEmpty)
			return;

		// Retrieve the ChannelAttribute symbol using fully qualified string name
		var channelAttributeSymbol = compilation.GetTypeByMetadataName(ChannelAttributeName);

		if (channelAttributeSymbol == null)
		{
			// Attribute not found; nothing to generate
			return;
		}

		// Collect all channel information
		var channelInfos = new List<ChannelInfo>();

		foreach (var classSymbol in channels.Distinct())
		{
			if (classSymbol == null)
				continue;

			// Retrieve the ChannelAttribute data using fully qualified string name
			var attributeData = classSymbol.GetAttributes()
				.FirstOrDefault(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, channelAttributeSymbol));

			if (attributeData == null)
				continue;

			// Extract channel name from the ChannelAttribute
			var channelName = attributeData.ConstructorArguments.Length > 0 ? attributeData.ConstructorArguments[0].Value as string : null;

			if (channelName is null)
				continue;

			channelInfos.Add(
				new ChannelInfo
				{
					EventName = classSymbol.ToDisplayString(),
					ChannelName = channelName
				}
			);
		}

		if (channelInfos.Count == 0)
			return;

		// Generate the channel registration code
		var sourceBuilder = new StringBuilder();

		AppendUsings(sourceBuilder);
		AppendNamespace(sourceBuilder);
		AppendClass(sourceBuilder, channelInfos);

		// Add the generated source to the compilation
		context.AddSource($"{FileName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
	}

	private static void AppendUsings(StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine("using MassTransit;");
		sourceBuilder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
		sourceBuilder.AppendLine();
	}

	private static void AppendNamespace(StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine($"namespace {Namespace};");
		sourceBuilder.AppendLine();
	}

	private static void AppendClass(
		StringBuilder sourceBuilder,
		List<ChannelInfo> channels)
	{
		sourceBuilder.AppendLine($"public static class {ClassName}");
		sourceBuilder.AppendLine("{");

		AppendMethod(sourceBuilder, channels);

		sourceBuilder.AppendLine("}");
	}

	private static void AppendMethod(
		StringBuilder sourceBuilder,
		List<ChannelInfo> channels)
	{
		sourceBuilder.AppendLine($"    public static void {MethodName}(this IRabbitMqBusFactoryConfigurator cfg)");
		sourceBuilder.AppendLine("    {");

		AppendChannelList(sourceBuilder, channels);

		sourceBuilder.AppendLine("    }");
	}

	private static void AppendChannelList(
		StringBuilder sourceBuilder,
		List<ChannelInfo> channels)
	{
		foreach (var channel in channels)
		{
			AppendChannelRegistration(sourceBuilder, channel);
		}
	}

	private static void AppendChannelRegistration(
		StringBuilder sourceBuilder,
		ChannelInfo channel)
	{
		sourceBuilder.AppendLine(
			$"        cfg.Message<{channel.EventName}>(x => x.SetEntityName(\"{channel.ChannelName}\"));"
		);
	}

	/// <summary>
	/// Represents information about a channel.
	/// </summary>
	private class ChannelInfo
	{
		public string EventName { get; init; } = string.Empty;
		public string ChannelName { get; init; } = string.Empty;
	}
}