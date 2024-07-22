using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Ithline.Extensions.Localization.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using static Ithline.Extensions.Localization.EntityFrameworkCore.LinkerFlags;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

internal sealed class EFStringLocalizer<[DynamicallyAccessedMembers(ContextFlags)] TContext> :
    IStringLocalizerFactory,
    IStringLocalizer,
    IStringResourceChangeToken<TContext>
    where TContext : DbContext, IStringResourceDbContext
{
    private readonly object _lock = new();
    private readonly IDbContextFactory<TContext> _contextFactory;
    private ResourceManager? _resourceManager;

    public EFStringLocalizer(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <inheritdoc/>
    public LocalizedString this[string name]
    {
        get
        {
            var format = this.GetResourceManager().GetString(name, CultureInfo.CurrentUICulture);
            return new LocalizedString(name, format ?? name, resourceNotFound: format is null);
        }
    }

    /// <inheritdoc/>
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture;
            var format = this.GetResourceManager().GetString(name, culture);
            var value = string.Format(culture, format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format is null);
        }
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(Type resourceSource)
    {
        return this;
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(string baseName, string location)
    {
        return this;
    }

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture;
        var resourceManager = this.GetResourceManager();

        var resourceNames = new HashSet<string>();
        var currentCulture = culture;
        while (true)
        {
            foreach (var key in resourceManager.GetAllResourceStrings(currentCulture))
            {
                resourceNames.Add(key);
            }

            if (!includeParentCultures || currentCulture == currentCulture.Parent)
            {
                break;
            }

            currentCulture = currentCulture.Parent;
        }

        foreach (var name in resourceNames)
        {
            var value = resourceManager.GetString(name, culture);
            yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }

    public void Raise()
    {
        Volatile.Write(ref _resourceManager, null);
    }

    private ResourceManager GetResourceManager()
    {
        var resourceLookup = Volatile.Read(ref _resourceManager);
        if (resourceLookup is not null)
        {
            return resourceLookup;
        }

        lock (_lock)
        {
            resourceLookup = Volatile.Read(ref _resourceManager);
            if (resourceLookup is not null)
            {
                return resourceLookup;
            }

            var builder = new ResourceManagerBuilder();
            using var db = _contextFactory.CreateDbContext();
            var entities = db.StringResources
                .Include(t => t.Cultures)
                .AsNoTracking()
                .ToArray();

            var invalid = new HashSet<StringResourceCultureId>();
            foreach (var entity in entities)
            {
                builder.Add(CultureInfo.InvariantCulture, entity.Id, entity.Invariant);

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

                        builder.Add(cultureInfo, entity.Id, culture.Value);
                    }
                    catch (Exception)
                    {
                        invalid.Add(culture.CultureId);
                    }
                }
            }

            resourceLookup = builder.ToImmutable();
            Volatile.Write(ref _resourceManager, resourceLookup);

            return resourceLookup;
        }
    }
}
