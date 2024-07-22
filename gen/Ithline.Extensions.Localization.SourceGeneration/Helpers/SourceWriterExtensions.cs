using System.Text;

namespace Ithline.Extensions.Localization.SourceGeneration;

internal static class SourceWriterExtensions
{
    /// <summary>
    /// Starts a block of source code.
    /// </summary>
    /// <param name="source">Source to write after the open brace.</param>
    public static void EmitStartBlock(this SourceWriter _writer, string? source = null)
    {
        if (source is not null)
        {
            _writer.WriteLine(source);
        }

        _writer.WriteLine("{");
        _writer.Indentation++;
    }

    /// <summary>
    /// Ends a block of source code.
    /// </summary>
    /// <param name="source">Source to write before the close brace.</param>
    /// <param name="endBraceTrailingSource">Trailing source after the end brace, e.g. ";" to end an init statement.</param>
    public static void EmitEndBlock(this SourceWriter _writer, string? source = null, string? endBraceTrailingSource = null)
    {
        if (source is not null)
        {
            _writer.WriteLine(source);
        }

        var endBlockSource = endBraceTrailingSource is null ? "}" : $"}}{endBraceTrailingSource}";
        _writer.Indentation--;
        _writer.WriteLine(endBlockSource);
    }

    public static string ToStringAndClear(this StringBuilder sb)
    {
        var result = sb.ToString();
        sb.Clear();

        return result;
    }
}
