using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ZEA.Communications.Messaging.MassTransit.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Helpers;

namespace ZEA.Communications.Messaging.MassTransit.Generators.RabbitMq;

[Generator]
public sealed class ChannelGenerator : ISourceGenerator
{
	private const string FileName = "MassTransitChannelRegistration";
	private const string Namespace = "ZEA.MassTransit.RabbitMq.Generated";
	private const string ClassName = "MassTransitChannelRegistration";
	private const string MethodName = "ConfigureChannels";

	public void Initialize(GeneratorInitializationContext context)
	{
		// Register a syntax receiver that will collect candidate classes
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		// Retrieve the populated receiver 
		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
		{
			return;
		}

		// Get the ChannelAttribute symbol
		var attributeSymbol = NamedTypeSymbolHelper.FindTypeByName(context.Compilation, nameof(ChannelAttribute));

		if (attributeSymbol == null)
		{
			// Attribute not found; nothing to generate
			return;
		}

		// Collect all channel information
		var channels = new List<ChannelInfo>();

		foreach (var classDeclaration in receiver.CandidateTypes)
		{
			var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);

			if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
				continue;

			var attributeData = classSymbol.GetAttributes()
				.FirstOrDefault(ad => ad.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true);

			if (attributeData is null)
			{
				continue;
			}

			// Extract channel name from the ChannelAttribute
			var channelName = attributeData.ConstructorArguments.Length > 0 ? attributeData.ConstructorArguments[0].Value as string : null;

			if (channelName is null)
				continue;

			channels.Add(
				new()
				{
					EventName = classSymbol.ToDisplayString(),
					ChannelName = channelName
				}
			);
		}

		if (channels.Count == 0)
		{
			return;
		}

		// Generate the channel registration code
		var sourceBuilder = new StringBuilder();

		AppendUsings(sourceBuilder);
		AppendNamespace(sourceBuilder);
		AppendClass(sourceBuilder, channels);

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
		sourceBuilder.AppendLine($"public static void {MethodName}(this IRabbitMqBusFactoryConfigurator cfg)");
		sourceBuilder.AppendLine("{");

		AppendChannelList(sourceBuilder, channels);

		sourceBuilder.AppendLine("}");
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
			$"cfg.Message<{channel.EventName}>(x => x.SetEntityName(\"{channel.ChannelName}\"));"
		);
	}

	/// <summary>
	/// Receiver that collects classes with attributes
	/// </summary>
	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<TypeDeclarationSyntax> CandidateTypes { get; } = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			switch (syntaxNode)
			{
				case ClassDeclarationSyntax classDeclaration:
					CandidateTypes.Add(classDeclaration);
					break;
				case RecordDeclarationSyntax recordDeclaration:
					CandidateTypes.Add(recordDeclaration);
					break;
			}
		}
	}

	private class ChannelInfo
	{
		public string EventName { get; init; } = string.Empty;
		public string ChannelName { get; init; } = string.Empty;
	}
}