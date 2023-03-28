using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Removes a set of whitespace characters from the beginning and ending of this string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trim()
    {
        // Hint: We don't want to call TrimStart and TrimEnd because we don't want to copy the buffer twice.
        var start = 0;
        var end = bufferPosition - 1;

        while (start < bufferPosition && char.IsWhiteSpace(buffer[start]))
        {
            start++;
        }

        while (end >= start && char.IsWhiteSpace(buffer[end]))
        {
            end--;
        }

        var newLength = end - start + 1;
        if (newLength < bufferPosition)
        {
            bufferPosition = newLength;
            buffer.Slice(start, start + newLength).CopyTo(buffer);
        }
    }

    /// <summary>
    /// Removes the specified character from the beginning and end of this string.
    /// </summary>
    /// <param name="c">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trim(char c)
    {
        // Remove character from the beginning
        var start = 0;
        while (start < bufferPosition && buffer[start] == c)
        {
            start++;
        }

        // Remove character from the end
        var end = bufferPosition - 1;
        while (end >= start && buffer[end] == c)
        {
            end--;
        }

        var newLength = end - start + 1;
        if (newLength < bufferPosition)
        {
            bufferPosition = newLength;
            buffer.Slice(start, start + newLength).CopyTo(buffer);
        }
    }

    /// <summary>
    /// Removes a set of whitespace characters from the beginning of this string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimStart()
    {
        var start = 0;
        while (start < bufferPosition && char.IsWhiteSpace(buffer[start]))
        {
            start++;
        }

        if (start > 0)
        {
            var newLength = bufferPosition - start;
            buffer.Slice(start, bufferPosition).CopyTo(buffer);
            bufferPosition = newLength;
        }
    }

    /// <summary>
    /// Removes the specified character from the beginning of this string.
    /// </summary>
    /// <param name="c">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimStart(char c)
    {
        var start = 0;
        while (start < bufferPosition && buffer[start] == c)
        {
            start++;
        }

        if (start > 0)
        {
            var newLength = bufferPosition - start;
            buffer.Slice(start, bufferPosition).CopyTo(buffer);
            bufferPosition = newLength;
        }
    }

    /// <summary>
    /// Removes a set of whitespace characters from the ending of this string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimEnd()
    {
        var end = bufferPosition - 1;
        while (end >= 0 && char.IsWhiteSpace(buffer[end]))
        {
            end--;
        }

        bufferPosition = end + 1;
    }

    /// <summary>
    /// Removes the specified character from the end of this string.
    /// </summary>
    /// <param name="c">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimEnd(char c)
    {
        var end = bufferPosition - 1;
        while (end >= 0 && buffer[end] == c)
        {
            end--;
        }

        bufferPosition = end + 1;
    }
}