using Ithline.Extensions.Localization.SourceGeneration.Specs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ithline.Extensions.Localization.SourceGeneration;

[Generator]
public sealed partial class LocalizationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if LAUNCH_DEBUGGER
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif

        var compilationData = context.CompilationProvider
            .Select((compilation, _) => compilation.Options is CSharpCompilationOptions options
                ? new CompilationData((CSharpCompilation)compilation)
                : null);

        var methodSpec = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "Ithline.Extensions.Localization.LocalizedStringAttribute",
                predicate: (node, _) => MethodCandidate.IsCandidateSyntaxNode(node),
                transform: MethodCandidate.Create)
            .Where(static candidate => candidate is not null)
            .Collect()
            .Combine(compilationData)
            .Select((tuple, cancellationToken) =>
            {
                if (tuple.Right is not CompilationData compilationData)
                {
                    return (null, null);
                }

                var parser = new Parser(compilationData.TypeSymbols, compilationData.LanguageVersionIsSupported);
                var specs = parser.GetGeneratorSpec(tuple.Left, cancellationToken);
                var diagnostics = parser.Diagnostics?.ToEquatableArray();
                return (specs, diagnostics);
            })
            .WithTrackingName(nameof(SourceGenerationSpec));

        context.RegisterSourceOutput(methodSpec, this.ReportDiagnosticsAndEmitSource);
    }

    /// <summary>
    /// Instrumentation helper for unit tests.
    /// </summary>
    public Action<SourceGenerationSpec>? OnSourceEmitting { get; init; }

    private void ReportDiagnosticsAndEmitSource(SourceProductionContext sourceProductionContext, (SourceGenerationSpec? SourceGenerationSpec, EquatableArray<DiagnosticInfo>? Diagnostics) input)
    {
        if (input.Diagnostics is EquatableArray<DiagnosticInfo> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                sourceProductionContext.ReportDiagnostic(diagnostic.CreateDiagnostic());
            }
        }

        if (input.SourceGenerationSpec is SourceGenerationSpec spec)
        {
            OnSourceEmitting?.Invoke(spec);
            var emitter = new Emitter(spec);
            emitter.Emit(sourceProductionContext);
        }
    }

    internal sealed class CompilationData
    {
        public CompilationData(CSharpCompilation compilation)
        {
            LanguageVersionIsSupported = compilation.LanguageVersion >= LanguageVersion.CSharp6;

            TypeSymbols = new KnownTypeSymbols(compilation);
        }

        public bool LanguageVersionIsSupported { get; }
        public KnownTypeSymbols TypeSymbols { get; }
    }
}
