using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Represents a culture ID of <see cref="StringResourceCulture"/>.
/// </summary>
public sealed class StringResourceCultureId : IEquatable<StringResourceCultureId>, IParsable<StringResourceCultureId>
{
    private readonly string _value;

    private StringResourceCultureId(string value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public static StringResourceCultureId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new StringResourceCultureId(s);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out StringResourceCultureId result)
    {
        if (s is null)
        {
            result = null;
            return false;
        }

        result = new StringResourceCultureId(s);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(StringResourceCultureId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is StringResourceCultureId other && this.Equals(other);
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
    public static bool operator ==(StringResourceCultureId? left, StringResourceCultureId? right)
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
    public static bool operator !=(StringResourceCultureId? left, StringResourceCultureId? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///   Defines an implicit conversion of a given <see cref="StringResourceCultureId"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="id">A <see cref="StringResourceCultureId"/> to implicitly convert.</param>
    /// <returns>A <see cref="string"/> instance converted from the <paramref name="id"/> parameter.</returns>
    public static implicit operator string(StringResourceCultureId id) => id._value;
}
