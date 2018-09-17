using Microsoft.CodeAnalysis.Diagnostics;

namespace MassTransit.Analyzers.Infrastructure
{
    public abstract class MassTransitAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(ctx =>
            {
                var massTransitCtx = new MassTransitContext(ctx.Compilation);

                if (ContextDefined(massTransitCtx))
                {
                    AnalyzeCompilation(ctx, massTransitCtx);
                }
            });
        }
        protected virtual bool ContextDefined(MassTransitContext massTransitCtx) => massTransitCtx.Version != null;
        protected abstract void AnalyzeCompilation(CompilationStartAnalysisContext ctx, MassTransitContext massTransitCtx);
    }
}