using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ZEA.Communications.Messaging.MassTransit.Generators.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Helpers;

namespace ZEA.Communications.Messaging.MassTransit.Generators;

[Generator]
public sealed class TopicGenerator : ISourceGenerator
{
	private const string FileName = "MassTransitTopicRegistration";
	private const string Namespace = "ZEA.MassTransit.Generated";
	private const string ClassName = "MassTransitTopicRegistration";
	private const string MethodName = "ConfigureTopics";

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

		// Get the TopicAttribute symbol
		var attributeSymbol = NamedTypeSymbolHelper.FindTypeByName(context.Compilation, nameof(TopicAttribute));

		if (attributeSymbol == null)
		{
			// Attribute not found; nothing to generate
			return;
		}

		// Collect all topic information
		var topics = new List<TopicInfo>();

		foreach (var classDeclaration in receiver.CandidateClasses)
		{
			var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
			var classSymbol = model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

			if (classSymbol is null)
				continue;

			var attributeData = classSymbol.GetAttributes()
				.FirstOrDefault(ad => ad.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true);

			if (attributeData is null)
				continue;

			// Extract topic name from the TopicAttribute
			var topicName = attributeData.ConstructorArguments.Length > 0 ? attributeData.ConstructorArguments[0].Value as string : null;

			if (topicName is null)
				continue;

			topics.Add(
				new()
				{
					EventName = classSymbol.ToDisplayString(),
					TopicName = topicName
				}
			);
		}

		if (topics.Count == 0)
		{
			return;
		}

		// Generate the topic registration code
		var sourceBuilder = new StringBuilder();

		AppendUsings(sourceBuilder);
		AppendNamespace(sourceBuilder);
		AppendClass(sourceBuilder, topics);

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
		List<TopicInfo> topics)
	{
		sourceBuilder.AppendLine($"public static class {ClassName}");
		sourceBuilder.AppendLine("{");

		AppendMethod(sourceBuilder, topics);

		sourceBuilder.AppendLine("}");
	}

	private static void AppendMethod(
		StringBuilder sourceBuilder,
		List<TopicInfo> topics)
	{
		sourceBuilder.AppendLine($"public static void {MethodName}(this IServiceBusBusFactoryConfigurator cfg)");
		sourceBuilder.AppendLine("{");

		AppendTopicList(sourceBuilder, topics);

		sourceBuilder.AppendLine("}");
	}

	private static void AppendTopicList(
		StringBuilder sourceBuilder,
		List<TopicInfo> topics)
	{
		foreach (var topic in topics)
		{
			AppendTopicRegistration(sourceBuilder, topic);
		}
	}

	private static void AppendTopicRegistration(
		StringBuilder sourceBuilder,
		TopicInfo topic)
	{
		sourceBuilder.AppendLine(
			$"cfg.Message<{topic.EventName}>(x => x.SetEntityName(\"{topic.TopicName}\"));"
		);
	}

	/// <summary>
	/// Receiver that collects classes with attributes
	/// </summary>
	private class SyntaxReceiver : ISyntaxReceiver
	{
		public List<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax> CandidateClasses { get; } = [];

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			// Look for classes with attributes
			if (syntaxNode is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration)
			{
				CandidateClasses.Add(classDeclaration);
			}
		}
	}

	private class TopicInfo
	{
		public string EventName { get; init; } = string.Empty;
		public string TopicName { get; init; } = string.Empty;
	}
}