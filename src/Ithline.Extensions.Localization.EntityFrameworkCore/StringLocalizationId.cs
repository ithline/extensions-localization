using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

public sealed class StringLocalizationId : IEquatable<StringLocalizationId>, IParsable<StringLocalizationId>
{
    private readonly string _value;

    private StringLocalizationId(string value)
    {
        _value = value;
    }

    public static StringLocalizationId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new StringLocalizationId(s);
    }

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

    public bool Equals(StringLocalizationId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is StringLocalizationId other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return string.GetHashCode(_value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator ==(StringLocalizationId? left, StringLocalizationId? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(StringLocalizationId? left, StringLocalizationId? right)
    {
        return !(left == right);
    }

    public static implicit operator string(StringLocalizationId id) => id._value;
}