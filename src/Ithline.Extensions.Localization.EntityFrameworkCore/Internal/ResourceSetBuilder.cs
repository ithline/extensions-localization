using System.Globalization;
using System.Runtime.InteropServices;

namespace Ithline.Extensions.Localization.EntityFrameworkCore.Internal;

internal sealed class ResourceSetBuilder
{
    private readonly Dictionary<CultureInfo, Dictionary<string, string>> _builder;

    public ResourceSetBuilder()
    {
        _builder = [];
    }

    public void Add(string id, CultureInfo culture, string value)
    {
        ref var dictionary = ref CollectionsMarshal.GetValueRefOrAddDefault(_builder, culture, out _);
        dictionary ??= [];

        dictionary[id] = value;
    }

    public ResourceSetLookup ToImmutable()
    {
        var sets = _builder
            .Select(t => new ResourceSet(t.Key, t.Value))
            .ToArray();

        return new ResourceSetLookup(sets);
    }
}
