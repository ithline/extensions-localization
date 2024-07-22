using Microsoft.Extensions.Localization;

namespace Ithline.Extensions.Localization;

public sealed class LocalizationManagerStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ILocalizationManager _manager;
    private LocalizationManagerStringLocalizer? _localizer;

    public LocalizationManagerStringLocalizerFactory(ILocalizationManager manager)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(Type resourceSource)
    {
        return _localizer ??= new LocalizationManagerStringLocalizer(_manager);
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(string baseName, string location)
    {
        return _localizer ??= new LocalizationManagerStringLocalizer(_manager);
    }
}
