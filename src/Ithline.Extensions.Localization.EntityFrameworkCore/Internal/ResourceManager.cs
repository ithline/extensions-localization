using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore.Internal;

internal sealed class ResourceManager
{
    private readonly ResourceSet[] _sets;

    public ResourceManager(ResourceSet[] cultures)
    {
        _sets = cultures;
    }

    public string? GetString(string name, CultureInfo culture)
    {
        while (true)
        {
            if (this.TryGetResourceSet(culture, out var set))
            {
                var value = set.Find(name);
                if (value is not null)
                {
                    return value;
                }
            }

            if (culture == culture.Parent)
            {
                break;
            }

            culture = culture.Parent;
        }

        return null;
    }

    public ImmutableArray<string> GetAllResourceStrings(CultureInfo culture)
    {
        if (this.TryGetResourceSet(culture, out var resourceSet))
        {
            return resourceSet.Keys;
        }

        return [];
    }

    private bool TryGetResourceSet(CultureInfo culture, [NotNullWhen(true)] out ResourceSet? set)
    {
        foreach (var candidate in _sets)
        {
            if (candidate.Culture == culture)
            {
                set = candidate;
                return true;
            }
        }

        set = null;
        return false;
    }
}
