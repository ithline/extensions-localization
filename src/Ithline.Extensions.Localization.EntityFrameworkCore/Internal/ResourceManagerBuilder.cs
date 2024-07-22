using System.Globalization;
using System.Runtime.InteropServices;

namespace Ithline.Extensions.Localization.EntityFrameworkCore.Internal;

internal sealed class ResourceManagerBuilder
{
    private readonly Dictionary<CultureInfo, Dictionary<string, string>> _builder;

    public ResourceManagerBuilder()
    {
        _builder = [];
    }

    public void Add(CultureInfo culture, string key, string value)
    {
        ref var dictionary = ref CollectionsMarshal.GetValueRefOrAddDefault(_builder, culture, out _);

        dictionary ??= [];
        dictionary[key] = value;
    }

    public ResourceManager ToImmutable()
    {
        var sets = _builder
            .Select(t => new ResourceSet(t.Key, t.Value))
            .ToArray();

        return new ResourceManager(sets);
    }
}
