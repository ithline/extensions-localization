namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Code first model used by <see cref="EntityFrameworkStringLocalizer{TContext}"/>.
/// </summary>
public sealed class StringLocalizationEntry
{
    /// <summary>
    /// The entity identifier of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    public required StringLocalizationId Id { get; init; }

    /// <summary>
    /// The invariant value of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    public required string Invariant { get; set; }

    /// <summary>
    /// The optional description of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The collection of culture specific translations of <see cref="StringLocalizationEntry"/>.
    /// </summary>
    public List<StringLocalizationEntryCulture> Cultures { get; } = [];
}
