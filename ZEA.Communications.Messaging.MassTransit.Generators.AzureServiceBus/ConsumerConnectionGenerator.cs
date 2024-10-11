using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ZEA.Communications.Messaging.MassTransit.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Helpers;

namespace ZEA.Communications.Messaging.MassTransit.Generators.AzureServiceBus;

[Generator]
public class ConsumerConnectionGenerator : ISourceGenerator
{
	private const string FileName = "MassTransitConsumerConnection";
	private const string Namespace = "ZEA.MassTransit.AzureServiceBus.Generated";
	private const string ClassName = "MassTransitConsumerConnection";
	private const string MethodName = "ConfigureSubscriptions";

	public void Initialize(GeneratorInitializationContext context)
	{
		// Register a syntax receiver that will be created for each generation pass
		context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		// Retrieve the populated receiver 
		if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			return;

		// Get the attribute symbol
		var attributeSymbol = NamedTypeSymbolHelper.FindTypeByName(context.Compilation, nameof(ConsumerAttribute));

		if (attributeSymbol == null)
		{
			// Attribute not found
			return;
		}

		// Get the IConsumer<T> symbol from MassTransit
		var consumerInterfaceSymbol = context.Compilation.GetTypeByMetadataName("MassTransit.IConsumer`1");

		if (consumerInterfaceSymbol == null)
		{
			// IConsumer<T> interface not found; ensure MassTransit is referenced
			return;
		}

		// Collect all consumer information
		var consumers = new List<ConsumerInfo>();

		foreach (var classDeclaration in receiver.CandidateClasses)
		{
			var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);

			if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
			{
				continue;
			}

			var attributeData = classSymbol.GetAttributes()
				.FirstOrDefault(ad => ad.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true);

			if (attributeData is null)
			{
				continue;
			}

			// Extract attribute arguments
			var entityName = attributeData.ConstructorArguments.Length > 0 ? attributeData.ConstructorArguments[0].Value as string : null;
			var subscriptionName = attributeData.ConstructorArguments.Length > 1
				? attributeData.ConstructorArguments[1].Value as string
				: null;

			if (entityName is null || subscriptionName is null)
			{
				continue;
			}

			consumers.Add(
				new()
				{
					EventName = classSymbol.ToDisplayString(),
					ChannelName = entityName,
					SubscriptionName = subscriptionName
				}
			);
		}

		if (consumers.Count == 0)
		{
			return;
		}

		// Generate the registration code
		var sourceBuilder = new StringBuilder();

		AppendUsings(sourceBuilder);
		AppendNamespace(sourceBuilder);
		AppendClass(sourceBuilder, consumers);

		// Add the generated source to the compilation
		context.AddSource($"{FileName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
	}

	private static void AppendUsings(StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
		sourceBuilder.AppendLine("using Microsoft.Extensions.Configuration;");
		sourceBuilder.AppendLine("using System;");
		sourceBuilder.AppendLine("using MassTransit;");
		sourceBuilder.AppendLine();
	}

	private static void AppendNamespace(StringBuilder sourceBuilder)
	{
		sourceBuilder.AppendLine($"namespace {Namespace};");
		sourceBuilder.AppendLine();
	}

	private static void AppendClass(
		StringBuilder sourceBuilder,
		List<ConsumerInfo> consumers)
	{
		sourceBuilder.AppendLine($"public static class {ClassName}");
		sourceBuilder.AppendLine("{");

		AppendMethod(sourceBuilder, consumers);

		sourceBuilder.AppendLine("}");
	}

	private static void AppendMethod(
		StringBuilder sourceBuilder,
		List<ConsumerInfo> consumers)
	{
		sourceBuilder.AppendLine(
			$"public static void {MethodName}(this IServiceBusBusFactoryConfigurator cfg, IBusRegistrationContext context)"
		);
		sourceBuilder.AppendLine("{");

		AppendAllConsumers(sourceBuilder, consumers);

		sourceBuilder.AppendLine("}");
	}

	private static void AppendAllConsumers(
		StringBuilder sourceBuilder,
		List<ConsumerInfo> consumers)
	{
		foreach (var consumer in consumers)
		{
			AppendConsumer(sourceBuilder, consumer);
		}
	}

	private static void AppendConsumer(
		StringBuilder sourceBuilder,
		ConsumerInfo consumer)
	{
		sourceBuilder.AppendLine(
			$$"""
			  cfg.SubscriptionEndpoint("{{consumer.SubscriptionName}}", "{{consumer.ChannelName}}", e =>
			  {
			      e.ConfigureConsumer<{{consumer.EventName}}>(context);
			  });
			  """
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

	private class ConsumerInfo
	{
		public string EventName { get; init; } = string.Empty;
		public string ChannelName { get; init; } = string.Empty;
		public string SubscriptionName { get; init; } = string.Empty;
	}
}