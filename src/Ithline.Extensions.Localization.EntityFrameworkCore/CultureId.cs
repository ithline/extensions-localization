using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Represents a culture ID of <see cref="StringLocalizationEntryCulture"/>.
/// </summary>
public sealed class CultureId : IEquatable<CultureId>, IParsable<CultureId>
{
    private readonly string _value;

    private CultureId(string value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public static CultureId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new CultureId(s);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CultureId result)
    {
        if (s is null)
        {
            result = null;
            return false;
        }

        result = new CultureId(s);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(CultureId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CultureId other && this.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return string.GetHashCode(_value, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _value;
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are equal.
    /// </summary>
    public static bool operator ==(CultureId? left, CultureId? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are *not* equal.
    /// </summary>
    public static bool operator !=(CultureId? left, CultureId? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///   Defines an implicit conversion of a given <see cref="CultureId"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="id">A <see cref="CultureId"/> to implicitly convert.</param>
    /// <returns>A <see cref="string"/> instance converted from the <paramref name="id"/> parameter.</returns>
    public static implicit operator string(CultureId id) => id._value;
}
