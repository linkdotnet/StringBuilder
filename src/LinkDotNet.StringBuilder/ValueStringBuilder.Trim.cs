using System.Buffers;
using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// The exact set of characters for which <see cref="char.IsWhiteSpace(char)"/> returns <see langword="true"/>,
    /// used to vectorize the whitespace-based Trim methods via <see cref="MemoryExtensions.IndexOfAnyExcept{T}(ReadOnlySpan{T}, SearchValues{T})"/>.
    /// </summary>
    private static readonly SearchValues<char> WhiteSpaceChars = SearchValues.Create(BuildWhiteSpaceChars());

    /// <summary>
    /// Removes all whitespace characters from the start and end of this builder.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trim()
    {
        // Hint: We don't want to call TrimStart and TrimEnd because we don't want to copy the buffer twice.
        var span = buffer[..bufferPosition];
        var start = span.IndexOfAnyExcept(WhiteSpaceChars);
        if (start == -1)
        {
            // The whole builder consists of whitespace.
            bufferPosition = 0;
            return;
        }

        var end = span.LastIndexOfAnyExcept(WhiteSpaceChars);
        var newLength = end - start + 1;
        if (newLength < bufferPosition)
        {
            bufferPosition = newLength;
            buffer.Slice(start, newLength).CopyTo(buffer);
        }
    }

    /// <summary>
    /// Removes all occurrences of the specified character from the start and end of this builder.
    /// </summary>
    /// <param name="value">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trim(char value)
    {
        var span = buffer[..bufferPosition];
        var start = span.IndexOfAnyExcept(value);
        if (start == -1)
        {
            // The whole builder consists of value.
            bufferPosition = 0;
            return;
        }

        var end = span.LastIndexOfAnyExcept(value);
        var newLength = end - start + 1;
        if (newLength < bufferPosition)
        {
            bufferPosition = newLength;
            buffer.Slice(start, newLength).CopyTo(buffer);
        }
    }

    /// <summary>
    /// Removes all whitespace characters from the start of this builder.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimStart()
    {
        var start = buffer[..bufferPosition].IndexOfAnyExcept(WhiteSpaceChars);
        if (start == -1)
        {
            // The whole builder consists of whitespace.
            bufferPosition = 0;
            return;
        }

        if (start > 0)
        {
            var newLength = bufferPosition - start;
            buffer.Slice(start, newLength).CopyTo(buffer);
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
        var start = buffer[..bufferPosition].IndexOfAnyExcept(value);
        if (start == -1)
        {
            // The whole builder consists of value.
            bufferPosition = 0;
            return;
        }

        if (start > 0)
        {
            var newLength = bufferPosition - start;
            buffer.Slice(start, newLength).CopyTo(buffer);
            bufferPosition = newLength;
        }
    }

    /// <summary>
    /// Removes all whitespace characters from the end of this builder.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimEnd()
    {
        var end = buffer[..bufferPosition].LastIndexOfAnyExcept(WhiteSpaceChars);
        bufferPosition = end + 1;
    }

    /// <summary>
    /// Removes all occurrences of the specified character from the end of this builder.
    /// </summary>
    /// <param name="value">The character to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimEnd(char value)
    {
        var end = buffer[..bufferPosition].LastIndexOfAnyExcept(value);
        bufferPosition = end + 1;
    }

    /// <summary>
    /// Removes the specified sequence of characters from the start of this builder.
    /// </summary>
    /// <param name="value">The sequence of characters to remove.</param>
    /// <param name="comparisonType">The way to compare the sequences of characters.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimPrefix(scoped ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal)
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
    public void TrimSuffix(scoped ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (AsSpan().EndsWith(value, comparisonType))
        {
            Remove(Length - value.Length, value.Length);
        }
    }

    private static char[] BuildWhiteSpaceChars() =>
    [
        (char)0x0009, (char)0x000A, (char)0x000B, (char)0x000C, (char)0x000D, (char)0x0020, (char)0x0085, (char)0x00A0, (char)0x1680,
        (char)0x2000, (char)0x2001, (char)0x2002, (char)0x2003, (char)0x2004, (char)0x2005, (char)0x2006, (char)0x2007, (char)0x2008, (char)0x2009, (char)0x200A,
        (char)0x2028, (char)0x2029, (char)0x202F, (char)0x205F, (char)0x3000,
    ];
}
