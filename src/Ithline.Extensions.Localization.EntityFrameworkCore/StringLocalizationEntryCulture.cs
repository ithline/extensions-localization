namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Code first model used by <see cref="EntityFrameworkStringLocalizer{TContext}"/>.
/// </summary>
public sealed class StringLocalizationEntryCulture
{
    /// <summary>
    /// The entity identifier of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    public required StringLocalizationId Id { get; init; }

    /// <summary>
    /// The culture identifier of <see cref="StringLocalizationEntryCulture"/>.
    /// </summary>
    public required CultureId CultureId { get; init; }

    /// <summary>
    /// The value for the current culture of <see cref="StringLocalizationEntryCulture"/>.
    /// </summary>
    public string? Value { get; set; }
}
