using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Removes all whitespace characters from the start and end of this builder.
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
    /// Removes all occurrences of the specified character from the start and end of this builder.
    /// </summary>
    /// <param name="value">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trim(char value)
    {
        // Remove character from the beginning
        var start = 0;
        while (start < bufferPosition && buffer[start] == value)
        {
            start++;
        }

        // Remove character from the end
        var end = bufferPosition - 1;
        while (end >= start && buffer[end] == value)
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
    /// Removes all whitespace characters from the start of this builder.
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
    /// Removes all occurrences of the specified character from the start of this builder.
    /// </summary>
    /// <param name="value">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimStart(char value)
    {
        var start = 0;
        while (start < bufferPosition && buffer[start] == value)
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
    /// Removes all whitespace characters from the end of this builder.
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
    /// Removes all occurrences of the specified character from the end of this builder.
    /// </summary>
    /// <param name="value">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimEnd(char value)
    {
        var end = bufferPosition - 1;
        while (end >= 0 && buffer[end] == value)
        {
            end--;
        }

        bufferPosition = end + 1;
    }

    /// <summary>
    /// Removes the specified sequence of characters from the start of this builder.
    /// </summary>
    /// <param name="value">The sequence of characters to remove.</param>
    /// <param name="comparisonType">The way to compare the sequences of characters.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimPrefix(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (AsSpan().StartsWith(value, comparisonType))
        {
            Remove(0, value.Length);
        }
    }

    /// <summary>
    /// Removes the specified sequence of characters from the end of this builder.
    /// </summary>
    /// <param name="value">The sequence of characters to remove.</param>
    /// <param name="comparisonType">The way to compare the sequences of characters.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimSuffix(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (AsSpan().EndsWith(value, comparisonType))
        {
            Remove(Length - value.Length, value.Length);
        }
    }
}