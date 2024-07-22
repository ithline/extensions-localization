using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ithline.Extensions.Localization.SourceGeneration;

internal sealed class KnownTypeSymbols
{
    public KnownTypeSymbols(CSharpCompilation compilation)
    {
        Compilation = compilation;

        IStringLocalizer = compilation.GetBestTypeByMetadataName("Microsoft.Extensions.Localization.IStringLocalizer")!;
        LocalizedString = compilation.GetBestTypeByMetadataName("Microsoft.Extensions.Localization.LocalizedString")!;
    }

    public CSharpCompilation Compilation { get; }

    public INamedTypeSymbol IStringLocalizer { get; }
    public INamedTypeSymbol LocalizedString { get; }
}
