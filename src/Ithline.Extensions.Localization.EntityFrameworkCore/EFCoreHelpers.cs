using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

internal static class EFCoreHelpers
{
    internal const DynamicallyAccessedMemberTypes MemberTypes =
        DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.NonPublicConstructors
        | DynamicallyAccessedMemberTypes.PublicProperties;
}
