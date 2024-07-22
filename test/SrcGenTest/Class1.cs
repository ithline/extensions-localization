using Ithline.Extensions.Localization;
using Microsoft.Extensions.Localization;

namespace SrcGenTest;

internal static partial class StaticLocalizerMethods
{
    [LocalizedString("ac")]
    public static partial LocalizedString Static(IStringLocalizer localizer, int a, string x);
}

internal partial class InstanceLocalizerMethods
{
    private readonly IStringLocalizer _localizer;

    [LocalizedString]
    public partial LocalizedString Instance();
}
