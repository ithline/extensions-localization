using Microsoft.EntityFrameworkCore;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Interface used to store instances of <see cref="StringLocalizationEntry"/> in a <see cref="DbContext"/>.
/// </summary>
public interface IStringLocalizationDbContext
{
    /// <summary>
    /// A collection of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    DbSet<StringLocalizationEntry> StringLocalizations { get; }
}
