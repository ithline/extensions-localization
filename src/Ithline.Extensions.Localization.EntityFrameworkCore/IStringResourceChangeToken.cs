using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Provides a method to notify the <see cref="IStringLocalizer"/> based on <typeparamref name="TContext"/> that the underlying data source has been changed.
/// </summary>
/// <typeparam name="TContext">The type of the db context.</typeparam>
public interface IStringResourceChangeToken<TContext>
    where TContext : DbContext, IStringResourceDbContext
{
    /// <summary>
    /// Notifies the <see cref="IStringLocalizer"/> based on <typeparamref name="TContext"/> that the underlying data source has been changed.
    /// </summary>
    void Raise();
}
