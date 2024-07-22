using Microsoft.CodeAnalysis;

namespace Ithline.Extensions.Localization.SourceGeneration;

internal static class Descriptors
{
    public static DiagnosticDescriptor LanguageVersionIsNotSupported { get; } = Create(
        id: "ITH0001",
        title: "Language version is required to be at least C# 6",
        message: "The project's language version has to be at least 'C# 6'.");

    public static DiagnosticDescriptor NameStartsWithUnderscore { get; } = Create(
        id: "ITH0002",
        title: "Method names cannot start with _",
        message: "Method names cannot start with '_' character.");

    public static DiagnosticDescriptor MustBePartial { get; } = Create(
        id: "ITH0003",
        title: "Method must be partial",
        message: "Method must be partial.");

    public static DiagnosticDescriptor MethodHasBody { get; } = Create(
        id: "ITH0004",
        title: "Method cannot have body",
        message: "Method cannot have body declared.");

    public static DiagnosticDescriptor MissingLocalizerArgument { get; } = Create(
        id: "ITH0005",
        title: "No arguments of type Microsoft.Extensions.Localization.IStringLocalizer found",
        message: "Method has no arguments of type Microsoft.Extensions.Localization.IStringLocalizer declared.");

    public static DiagnosticDescriptor MultipleLocalizerArguments { get; } = Create(
        id: "ITH0006",
        title: "Multiple arguments of type Microsoft.Extensions.Localization.IStringLocalizer found",
        message: "Only one argument of type Microsoft.Extensions.Localization.IStringLocalizer is permitted as argument.");

    public static DiagnosticDescriptor InstanceMethodHasLocalizerArgument { get; } = Create(
        id: "ITH0007",
        title: "Instance method has argument of type Microsoft.Extensions.Localization.IStringLocalizer",
        message: "Instance method are not allowed to declare parameters of type Microsoft.Extensions.Localization.IStringLocalizer.");

    public static DiagnosticDescriptor PrimaryConstructorParameterHidden { get; } = Create(
        id: "ITH0008",
        title: "Primary constructor parameter of type Microsoft.Extensions.Localization.IStringLocalizer is hidden by a field",
        message: "Class '{0}' has a primary constructor parameter of type Microsoft.Extensions.Localization.IStringLocalizer that is hidden by a field in the class or a base class, preventing its use..");

    public static DiagnosticDescriptor MultipleLocalizerFields { get; } = Create(
        id: "ITH0009",
        title: "Found multiple fields of type Microsoft.Extensions.Localization.IStringLocalizer",
        message: "Class '{0}' has multiple fields of type Microsoft.Extensions.Localization.IStringLocalizer.");

    public static DiagnosticDescriptor MissingLocalizerField { get; } = Create(
        id: "ITH0010",
        title: "Class has no fields of type Microsoft.Extensions.Localization.IStringLocalizer",
        message: "Class '{0}' has no accessible fields of type Microsoft.Extensions.Localization.IStringLocalizer.");

    public static DiagnosticDescriptor MethodIsGeneric { get; } = Create(
        id: "ITH0011",
        title: "Method cannot be generic.",
        message: "Method cannot be generic.");

    public static DiagnosticDescriptor MustReturnLocalizedString { get; } = Create(
        id: "ITH0012",
        title: "Method must return Microsoft.Extensions.Localization.LocalizedString",
        message: "Method must return Microsoft.Extensions.Localization.LocalizedString.");

    public static DiagnosticDescriptor ParameterHasRefModifier { get; } = Create(
        id: "ITH0013",
        title: "Argument is using unsupported parameter modifier",
        message: "Argument '{0}' is using an unsupported parameter modifier.");

    private static DiagnosticDescriptor Create(string id, string title, string message, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        return new DiagnosticDescriptor(id, title, message, nameof(LocalizationGenerator), severity, true);
    }
}
