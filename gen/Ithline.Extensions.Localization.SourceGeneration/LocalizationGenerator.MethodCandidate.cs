using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Ithline.Extensions.Localization.SourceGeneration.Specs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ithline.Extensions.Localization.SourceGeneration;

public sealed partial class LocalizationGenerator
{
    private sealed class MethodCandidate
    {
        private MethodCandidate(
            ClassDeclarationSyntax classSyntax,
            INamedTypeSymbol classSymbol,
            MethodDeclarationSyntax methodSyntax,
            IMethodSymbol methodSymbol,
            string? resourceId)
        {
            ClassSyntax = classSyntax;
            ClassSymbol = classSymbol;
            MethodSyntax = methodSyntax;
            MethodSymbol = methodSymbol;
            ResourceId = resourceId;

            TypeRef = new TypeRef(classSymbol);
        }

        public TypeRef TypeRef { get; }
        public ClassDeclarationSyntax ClassSyntax { get; }
        public INamedTypeSymbol ClassSymbol { get; }
        public MethodDeclarationSyntax MethodSyntax { get; }
        public IMethodSymbol MethodSymbol { get; }
        public string? ResourceId { get; }

        public static bool IsCandidateSyntaxNode(SyntaxNode node)
        {
            return TryExtractSyntaxNodes(node, out _, out _);
        }

        public static MethodCandidate? Create(GeneratorAttributeSyntaxContext ctx, CancellationToken cancellationToken)
        {
            if (!TryExtractSyntaxNodes(ctx.TargetNode, out var classSyntax, out var methodSyntax))
            {
                return null;
            }

            if (ctx.TargetSymbol is not IMethodSymbol methodSymbol)
            {
                return null;
            }

            var classSymbol = ctx.SemanticModel.GetDeclaredSymbol(classSyntax, cancellationToken);
            if (classSymbol is null)
            {
                return null;
            }

            string? resourceId = null;
            foreach (var attributeData in ctx.Attributes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var args = attributeData.ConstructorArguments;

                // more than 1 argument is compilation error
                if (args.Length > 1)
                {
                    Debug.Assert(false, "Unexpected number of arguments in attribute ctor.");
                    return null;
                }

                if (args is [var arg])
                {
                    // compilation error
                    if (arg.Kind is TypedConstantKind.Error)
                    {
                        return null;
                    }

                    resourceId = arg.IsNull ? null : (string?)arg.Value;
                }
            }

            return new MethodCandidate(classSyntax, classSymbol, methodSyntax, methodSymbol, resourceId);
        }

        private static bool TryExtractSyntaxNodes(
            SyntaxNode node,
            [NotNullWhen(true)] out ClassDeclarationSyntax? classSyntax,
            [NotNullWhen(true)] out MethodDeclarationSyntax? methodSyntax)
        {
            if (node is MethodDeclarationSyntax mds && mds.Parent is ClassDeclarationSyntax cds)
            {
                classSyntax = cds;
                methodSyntax = mds;
                return true;
            }

            classSyntax = null;
            methodSyntax = null;
            return false;
        }
    }
}
