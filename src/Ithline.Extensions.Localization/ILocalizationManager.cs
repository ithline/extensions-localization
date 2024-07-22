using System.Globalization;

namespace Ithline.Extensions.Localization;

/// <summary>
/// 
/// </summary>
public interface ILocalizationManager
{
    void Clear();
    IEnumerable<string> GetAllResourceStrings(CultureInfo culture);
    string? GetString(string name, CultureInfo culture);
}
