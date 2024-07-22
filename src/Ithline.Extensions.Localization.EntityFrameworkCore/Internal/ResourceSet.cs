using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore.Internal;

internal sealed class ResourceSet
{
    private readonly FrozenDictionary<string, string> _resources;

    public ResourceSet(
        CultureInfo culture,
        IEnumerable<KeyValuePair<string, string>> resources)
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(resources);

        Culture = culture;
        _resources = FrozenDictionary.ToFrozenDictionary(resources);
    }

    public CultureInfo Culture { get; }
    public ImmutableArray<string> Keys => _resources.Keys;

    public string? Find(string resourceId)
    {
        ArgumentNullException.ThrowIfNull(resourceId);

        return _resources.TryGetValue(resourceId, out var value) ? value : null;
    }
}
