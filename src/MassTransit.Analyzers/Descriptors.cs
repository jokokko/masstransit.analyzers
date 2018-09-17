using MassTransit.Analyzers.Infrastructure;
using Microsoft.CodeAnalysis;

namespace MassTransit.Analyzers
{
    internal static class Descriptors
    {
        private static DiagnosticDescriptor Rule(string id, string title, RuleCategory category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {            
            return new DiagnosticDescriptor(id, title, messageFormat, category.Name, defaultSeverity, true, description, $"https://jokokko.github.io/masstransit.analyzers/rules/{id}");
        }

	    internal static readonly DiagnosticDescriptor MassTransit1000LossyMessagingCandidate = Rule("MassTransit1000", "Await or capture a messaging task from MassTransit", RuleCategory.Usage, DiagnosticSeverity.Error, "MassTransit messaging operation is not awaited on, or captured, resulting in possible message loss.");
    }
}