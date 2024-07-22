using Microsoft.EntityFrameworkCore;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Interface used to store instances of <see cref="StringResource"/> in a <see cref="DbContext"/>.
/// </summary>
public interface IStringResourceDbContext
{
    /// <summary>
    /// A collection of <see cref="StringResource"/>.
    /// </summary>
    DbSet<StringResource> StringResources { get; }
}
