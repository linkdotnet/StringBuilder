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
}