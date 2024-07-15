using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Represents an ID of <see cref="StringLocalizationEntry"/>.
/// </summary>
public sealed class StringLocalizationId : IEquatable<StringLocalizationId>, IParsable<StringLocalizationId>
{
    private readonly string _value;

    private StringLocalizationId(string value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public static StringLocalizationId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new StringLocalizationId(s);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out StringLocalizationId result)
    {
        if (s is null)
        {
            result = null;
            return false;
        }

        result = new StringLocalizationId(s);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(StringLocalizationId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is StringLocalizationId other && this.Equals(other);
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
    public static bool operator ==(StringLocalizationId? left, StringLocalizationId? right)
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
    public static bool operator !=(StringLocalizationId? left, StringLocalizationId? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///   Defines an implicit conversion of a given <see cref="StringLocalizationId"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="id">A <see cref="StringLocalizationId"/> to implicitly convert.</param>
    /// <returns>A <see cref="string"/> instance converted from the <paramref name="id"/> parameter.</returns>
    public static implicit operator string(StringLocalizationId id) => id._value;
}
