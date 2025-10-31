using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Pads the left side of the string with the given character.
    /// </summary>
    /// <param name="totalWidth">Total width of the string after padding.</param>
    /// <param name="paddingChar">Character to pad the string with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PadLeft(int totalWidth, char paddingChar)
    {
        if (totalWidth <= bufferPosition)
        {
            return;
        }

        EnsureCapacity(totalWidth);

        var padding = totalWidth - bufferPosition;
        buffer[..bufferPosition].CopyTo(buffer[padding..]);
        buffer[..padding].Fill(paddingChar);
        bufferPosition = totalWidth;
    }

    /// <summary>
    /// Pads the right side of the string with the given character.
    /// </summary>
    /// <param name="totalWidth">Total width of the string after padding.</param>
    /// <param name="paddingChar">Character to pad the string with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PadRight(int totalWidth, char paddingChar)
    {
        if (totalWidth <= bufferPosition)
        {
            return;
        }

        EnsureCapacity(totalWidth);

        buffer[bufferPosition..totalWidth].Fill(paddingChar);
        bufferPosition = totalWidth;
    }

    /// <summary>
    /// Appends the source string padded on the left with the given character to reach the specified total width.
    /// </summary>
    /// <param name="source">The source string to pad and append.</param>
    /// <param name="totalWidth">Total width of the padded string.</param>
    /// <param name="paddingChar">Character to pad the string with. Defaults to space.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PadLeft(ReadOnlySpan<char> source, int totalWidth, char paddingChar = ' ')
    {
        if (totalWidth <= source.Length)
        {
            Append(source);
            return;
        }

        var padding = totalWidth - source.Length;
        EnsureCapacity(bufferPosition + totalWidth);

        buffer.Slice(bufferPosition, padding).Fill(paddingChar);
        bufferPosition += padding;

        source.CopyTo(buffer[bufferPosition..]);
        bufferPosition += source.Length;
    }

    /// <summary>
    /// Appends the source string padded on the right with the given character to reach the specified total width.
    /// </summary>
    /// <param name="source">The source string to pad and append.</param>
    /// <param name="totalWidth">Total width of the padded string.</param>
    /// <param name="paddingChar">Character to pad the string with. Defaults to space.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PadRight(ReadOnlySpan<char> source, int totalWidth, char paddingChar = ' ')
    {
        if (totalWidth <= source.Length)
        {
            Append(source);
            return;
        }

        var padding = totalWidth - source.Length;
        EnsureCapacity(bufferPosition + totalWidth);

        source.CopyTo(buffer[bufferPosition..]);
        bufferPosition += source.Length;

        buffer.Slice(bufferPosition, padding).Fill(paddingChar);
        bufferPosition += padding;
    }
}