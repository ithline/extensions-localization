namespace Ithline.Extensions.Localization.SourceGeneration.Specs;

public sealed record MethodParameterSpec
{
    public required string Name { get; init; }
    public required TypeRef Type { get; init; }
    public required bool IsLocalizer { get; init; }
}
