using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

internal sealed class EntityFrameworkStringLocalizer<TContext> : IStringLocalizerFactory, IStringLocalizer
    where TContext : DbContext, IStringLocalizationDbContext
{
    private const string CacheKey = "__EntityFrameworkStringLocalizer";
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly IMemoryCache _cache;

    public EntityFrameworkStringLocalizer(IDbContextFactory<TContext> contextFactory, IMemoryCache cache)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public LocalizedString this[string name]
    {
        get
        {
            var dictionary = this.ResolveDictionary();
            var format = dictionary.FindString(name, CultureInfo.CurrentUICulture);

            return new LocalizedString(name, format ?? name, resourceNotFound: format is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var dictionary = this.ResolveDictionary();
            var format = dictionary.FindString(name, CultureInfo.CurrentUICulture);

            // TODO: add named hole formatting
            var value = string.Format(CultureInfo.CurrentUICulture, format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format is null);
        }
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return this;
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return this;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture;
        var dictionary = this.ResolveDictionary();
        foreach (var key in dictionary.Keys)
        {
            var value = dictionary.FindString(key, culture);
            yield return new LocalizedString(key, value ?? key, resourceNotFound: value == null);
        }
    }

    private StringLocalizationDictionary ResolveDictionary()
    {
        if (_cache.TryGetValue<StringLocalizationDictionary>(CacheKey, out var dictionary) && dictionary is not null)
        {
            return dictionary;
        }

        dictionary = new StringLocalizationDictionary();
        using var entry = _cache.CreateEntry(CacheKey);
        using var db = _contextFactory.CreateDbContext();
        var entities = db.StringLocalizations
            .Include(t => t.Cultures)
            .AsNoTracking()
            .ToArray();

        foreach (var entity in entities)
        {
            dictionary.Add(entity.Id, entity.Invariant, entity.Cultures);
        }

        entry.Value = dictionary;

        return dictionary;
    }
}
