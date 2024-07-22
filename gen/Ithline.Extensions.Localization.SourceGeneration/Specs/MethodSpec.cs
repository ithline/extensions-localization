namespace Ithline.Extensions.Localization.SourceGeneration.Specs;

public sealed record MethodSpec
{
    public required string Name { get; init; }
    public required string? Modifiers { get; init; }
    public required TypeRef ReturnType { get; init; }
    public required bool IsExtensionMethod { get; init; }
    public required bool IsStatic { get; init; }
    public required bool IsPartial { get; init; }
    public required string ResourceId { get; init; }
    public required EquatableArray<MethodParameterSpec> Parameters { get; init; }
}
