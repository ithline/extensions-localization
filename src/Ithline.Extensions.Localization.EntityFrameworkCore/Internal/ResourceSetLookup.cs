using System.Collections.Immutable;
using System.Globalization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore.Internal;

internal sealed class ResourceSetLookup
{
    private readonly Dictionary<CultureInfo, ResourceSet> _sets;

    public ResourceSetLookup(ResourceSet[] sets)
    {
        _sets = sets.ToDictionary(t => t.Culture);
    }

    public ResourceSet? Find(CultureInfo culture)
    {
        return _sets.GetValueOrDefault(culture);
    }
}
