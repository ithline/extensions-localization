namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Code first model used by <see cref="EFStringLocalizer{TContext}"/>.
/// </summary>
public sealed class StringResource
{
    /// <summary>
    /// The entity identifier of <see cref="StringResource"/>.
    /// </summary>
    public required StringResourceId Id { get; init; }

    /// <summary>
    /// The invariant value of <see cref="StringResource"/>.
    /// </summary>
    public required string Invariant { get; set; }

    /// <summary>
    /// The optional description of <see cref="StringResource"/>.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The collection of culture specific translations of <see cref="StringResource"/>.
    /// </summary>
    public List<StringResourceCulture> Cultures { get; } = [];
}
