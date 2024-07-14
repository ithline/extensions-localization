using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

internal sealed class StringLocalizationDictionary
{
    private readonly Dictionary<CultureKey, string> _cultures;
    private readonly Dictionary<string, string> _invariant;

    public StringLocalizationDictionary()
    {
        _invariant = [];
        _cultures = [];
    }

    public IReadOnlyCollection<string> Keys => _invariant.Keys;

    public string? FindString(string key, CultureInfo culture)
    {
        if (!_invariant.TryGetValue(key, out var invariant))
        {
            return null;
        }

        // we try to find a value in the culture hierarchy
        string? value = null;
        var currentCulture = culture;
        while (!currentCulture.IsNeutralCulture)
        {
            var cultureKey = new CultureKey(key, currentCulture);
            if (_cultures.TryGetValue(cultureKey, out value))
            {
                break;
            }

            currentCulture = currentCulture.Parent;
        }

        // value wasn't found in any culture, set it from invariant
        value ??= invariant;

        // if value was found as fallback, add all previous cultures
        while (!culture.Equals(currentCulture))
        {
            var cultureKey = new CultureKey(key, culture);
            _cultures.Add(cultureKey, value);
            culture = culture.Parent;
        }

        return value;
    }

    public void Add(string key, string invariant, IEnumerable<StringLocalizationEntryCulture>? cultures = null)
    {
        _invariant[key] = invariant;

        // if we don't have cultures, we can return
        if (cultures is null)
        {
            return;
        }

        // for each culture we try to add row to lookup
        foreach (var culture in cultures)
        {
            // we ignore rows with empty id and/or value
            if (string.IsNullOrWhiteSpace(culture.Value))
            {
                continue;
            }

            // we find current culture info and if it's invariant culture, we skip
            var cultureInfo = CultureInfo.GetCultureInfo(culture.CultureId);
            if (cultureInfo.IsNeutralCulture)
            {
                continue;
            }

            var cultureKey = new CultureKey(key, cultureInfo);
            _cultures[cultureKey] = culture.Value;
        }
    }

    private readonly struct CultureKey : IEquatable<CultureKey>
    {
        private readonly string _cultureName;
        private readonly string _key;

        public CultureKey(string key, CultureInfo culture)
        {
            _key = key;
            _cultureName = culture.Name;
        }

        public bool Equals(CultureKey other)
        {
            return _key == other._key
                && _cultureName == other._cultureName;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is CultureKey ck && this.Equals(ck);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_key, _cultureName);
        }
    }
}
