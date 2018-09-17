using System.Collections.Immutable;
using MassTransit.Analyzers.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MassTransit.Analyzers.Usage
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AwaitOnRequestAnalyzer : MassTransitInvocationAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptors.MassTransit1000LossyMessagingCandidate);

		public AwaitOnRequestAnalyzer() : base(
			"MassTransit.ConsumeContext.Request",
			"MassTransit.ConsumeContext.RespondAsync",
			"MassTransit.ConsumeContextExtensions.Forward",
			"MassTransit.ConsumeContextSelfSchedulerExtensions.ScheduleSend",
			"MassTransit.IPublishEndpoint.Publish",
			"MassTransit.IRequestClient.Request",
			"MassTransit.MessageRequestClient.Request",
			"MassTransit.PublishEndpointRecurringSchedulerExtensions.ScheduleRecurringSend",
			"MassTransit.RedeliverExtensions.Redeliver",
			"MassTransit.RequestExtensions.Request",
			"MassTransit.RespondAsyncExecuteExtensions.RespondAsync",
			"MassTransit.SendEndpointExtensions.Send"
			)
		{
		}
		protected override void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpressionSyntax,
			IMethodSymbol methodSymbol)
		{
			if (!(invocationExpressionSyntax.Parent is ExpressionStatementSyntax))
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(
				SupportedDiagnostics[0],
				invocationExpressionSyntax.GetLocation(), SymbolDisplay.ToDisplayString(methodSymbol,
					SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
						SymbolDisplayParameterOptions.None))));
		}
	}
}