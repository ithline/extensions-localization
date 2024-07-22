namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Code first model used by <see cref="EFStringLocalizer{TContext}"/>.
/// </summary>
public sealed class StringResourceCulture
{
    /// <summary>
    /// The entity identifier of <see cref="StringResourceCulture"/>.
    /// </summary>
    public required StringResourceId Id { get; init; }

    /// <summary>
    /// The culture identifier of <see cref="StringResourceCulture"/>.
    /// </summary>
    public required StringResourceCultureId CultureId { get; init; }

    /// <summary>
    /// The value for the current culture of <see cref="StringResourceCulture"/>.
    /// </summary>
    public required string Value { get; set; }
}
