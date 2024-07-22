namespace Ithline.Extensions.Localization.SourceGeneration.Specs;

public sealed record SourceGenerationSpec
{
    public required EquatableArray<TypeSpec> Types { get; init; }
}
