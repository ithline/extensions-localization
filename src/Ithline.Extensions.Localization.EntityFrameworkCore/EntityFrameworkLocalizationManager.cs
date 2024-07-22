using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Ithline.Extensions.Localization.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using static Ithline.Extensions.Localization.EntityFrameworkCore.LinkerFlags;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

public sealed class EntityFrameworkLocalizationManager<[DynamicallyAccessedMembers(ContextFlags)] TContext>
    : ILocalizationManager
    where TContext : DbContext, IStringResourceDbContext
{
    private readonly object _lock = new();
    private readonly IDbContextFactory<TContext> _contextFactory;
    private ResourceSetLookup? _resourceLookup;

    public EntityFrameworkLocalizationManager(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public void Clear()
    {
        Volatile.Write(ref _resourceLookup, null);
    }

    public IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
    {
        var lookup = this.EnsureResourceLookup();
        var set = lookup.Find(culture);
        if (set is null)
        {
            yield break;
        }

        foreach (var key in set.Keys)
        {
            yield return key;
        }
    }

    public string? GetString(string name, CultureInfo culture)
    {
        var lookup = this.EnsureResourceLookup();
        do
        {
            var set = lookup.Find(culture);
            if (set is not null)
            {
                var s = set.Find(name);
                if (s is not null)
                {
                    return s;
                }
            }
        }
        while (culture != CultureInfo.InvariantCulture);

        return null;
    }

    private ResourceSetLookup EnsureResourceLookup()
    {
        var resourceLookup = Volatile.Read(ref _resourceLookup);
        if (resourceLookup is not null)
        {
            return resourceLookup;
        }

        lock (_lock)
        {
            resourceLookup = Volatile.Read(ref _resourceLookup);
            if (resourceLookup is not null)
            {
                return resourceLookup;
            }

            resourceLookup = this.CreateResourceLookup();
            Volatile.Write(ref _resourceLookup, resourceLookup);

            return resourceLookup;
        }
    }

    private ResourceSetLookup CreateResourceLookup()
    {
        var builder = new ResourceSetBuilder();
        using var db = _contextFactory.CreateDbContext();
        var entities = db.StringResources
            .Include(t => t.Cultures)
            .AsNoTracking()
            .ToArray();

        var invalid = new HashSet<StringResourceCultureId>();
        foreach (var entity in entities)
        {
            builder.Add(entity.Id, CultureInfo.InvariantCulture, entity.Invariant);

            foreach (var culture in entity.Cultures)
            {
                // we skip invalid cultures
                if (invalid.Contains(culture.CultureId))
                {
                    continue;
                }

                try
                {
                    // we try to resolve culture info based on id
                    var cultureInfo = CultureInfo.GetCultureInfo(culture.CultureId);

                    // if culture is invariant, we skip
                    if (cultureInfo == CultureInfo.InvariantCulture)
                    {
                        continue;
                    }

                    builder.Add(entity.Id, cultureInfo, culture.Value);
                }
                catch (Exception)
                {
                    invalid.Add(culture.CultureId);
                }
            }
        }

        return builder.ToImmutable();
    }
}
