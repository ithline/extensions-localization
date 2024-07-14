using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Localization.EntityFrameworkCore;

public sealed class CultureId : IEquatable<CultureId>, IParsable<CultureId>
{
    private readonly string _value;

    private CultureId(string value)
    {
        _value = value;
    }

    public static CultureId Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        return new CultureId(s);
    }

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

    public bool Equals(CultureId? other)
    {
        return other is not null && string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is CultureId other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return string.GetHashCode(_value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator ==(CultureId? left, CultureId? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(CultureId? left, CultureId? right)
    {
        return !(left == right);
    }

    public static implicit operator string(CultureId id) => id._value;
}
