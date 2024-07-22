using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Ithline.Extensions.Localization;

internal sealed class LocalizationManagerStringLocalizer : IStringLocalizer
{
    private readonly ILocalizationManager _manager;

    public LocalizationManagerStringLocalizer(ILocalizationManager manager)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    public LocalizedString this[string name]
    {
        get
        {
            var format = _manager.GetString(name, CultureInfo.CurrentUICulture);
            return new LocalizedString(name, format ?? name, resourceNotFound: format is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = _manager.GetString(name, CultureInfo.CurrentUICulture);
            var value = string.Format(CultureInfo.CurrentUICulture, format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format is null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture;
        var resourceNames = includeParentCultures
            ? this.GetResourceNamesFromCultureHierarchy(culture)
            : _manager.GetAllResourceStrings(culture);

        foreach (var name in resourceNames)
        {
            var value = _manager.GetString(name, culture);
            yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }

    private HashSet<string> GetResourceNamesFromCultureHierarchy(CultureInfo startingCulture)
    {
        var currentCulture = startingCulture;
        var resourceNames = new HashSet<string>();

        do
        {
            var cultureResourceNames = _manager.GetAllResourceStrings(currentCulture);
            if (cultureResourceNames is not null)
            {
                foreach (var resourceName in cultureResourceNames)
                {
                    resourceNames.Add(resourceName);
                }
            }

            currentCulture = currentCulture.Parent;
        }
        // currentCulture at currentCulture, probably time to leave
        while (currentCulture != currentCulture.Parent);

        return resourceNames;
    }
}
