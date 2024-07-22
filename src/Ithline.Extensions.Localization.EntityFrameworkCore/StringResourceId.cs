using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

/// <summary>
/// Represents an ID of <see cref="StringResource"/>.
/// </summary>
public sealed class StringResourceId : IEquatable<StringResourceId>, IParsable<StringResourceId>
{
    private readonly string _value;

    private StringResourceId(string value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public static StringResourceId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new StringResourceId(s);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out StringResourceId result)
    {
        if (s is null)
        {
            result = null;
            return false;
        }

        result = new StringResourceId(s);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(StringResourceId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is StringResourceId other && this.Equals(other);
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
    public static bool operator ==(StringResourceId? left, StringResourceId? right)
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
    public static bool operator !=(StringResourceId? left, StringResourceId? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///   Defines an implicit conversion of a given <see cref="StringResourceId"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="id">A <see cref="StringResourceId"/> to implicitly convert.</param>
    /// <returns>A <see cref="string"/> instance converted from the <paramref name="id"/> parameter.</returns>
    public static implicit operator string(StringResourceId id) => id._value;
}
