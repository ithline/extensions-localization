using Microsoft.Extensions.Localization;

namespace Ithline.Extensions.Localization;

/// <summary>
/// Provides information to guide the production of a strongly-typed localization method.
/// </summary>
/// <remarks>
/// <para>The method this attribute is applied to:</para>
/// <para>   - Must be a partial method.</para>
/// <para>   - Must return <see cref="LocalizedString"/>.</para>
/// <para>   - Must not be generic.</para>
/// <para>   - Must have an <see cref="IStringLocalizer"/> as one of its parameters.</para>
/// <para>   - None of the parameters can be generic.</para>
/// </remarks>
/// <example>
/// <format type="text/markdown"><![CDATA[
/// ```csharp
/// static partial class Localizations
/// {
///     [LocalizedString("MyLocalizedValue")]
///     static partial LocalizedString MyLocalizedValue(IStringLocalizer localizer, string s, int i);
/// }
/// ```
/// ]]></format>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LocalizedStringAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedStringAttribute"/>.
    /// </summary>
    public LocalizedStringAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedStringAttribute"/> with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    public LocalizedStringAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The name of the string resource.
    /// </summary>
    public string? Name { get; }
}
