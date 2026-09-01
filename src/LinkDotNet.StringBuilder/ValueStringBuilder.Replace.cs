using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Replaces all instances of one character with another in this builder.
    /// </summary>
    /// <param name="oldValue">The character to replace.</param>
    /// <param name="newValue">The character to replace <paramref name="oldValue"/> with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Replace(char oldValue, char newValue) => Replace(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one character with another in this builder.
    /// </summary>
    /// <param name="oldValue">The character to replace.</param>
    /// <param name="newValue">The character to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Replace(char oldValue, char newValue, int startIndex, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, Length, nameof(count));

        buffer.Slice(startIndex, count).Replace(oldValue, newValue);
    }

    /// <summary>
    /// Replaces all instances of one rune with another in this builder.
    /// </summary>
    /// <param name="oldValue">The rune to replace.</param>
    /// <param name="newValue">The rune to replace <paramref name="oldValue"/> with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Replace(Rune oldValue, Rune newValue) => Replace(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one rune with another in this builder.
    /// </summary>
    /// <param name="oldValue">The rune to replace.</param>
    /// <param name="newValue">The rune to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Replace(Rune oldValue, Rune newValue, int startIndex, int count)
    {
        Span<char> oldValueChars = stackalloc char[2];
        var oldValueCharsWritten = oldValue.EncodeToUtf16(oldValueChars);
        ReadOnlySpan<char> oldValueCharsSlice = oldValueChars[..oldValueCharsWritten];

        Span<char> newValueChars = stackalloc char[2];
        var newValueCharsWritten = newValue.EncodeToUtf16(newValueChars);
        ReadOnlySpan<char> newValueCharsSlice = newValueChars[..newValueCharsWritten];

        Replace(oldValueCharsSlice, newValueCharsSlice, startIndex, count);
    }

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">The string to replace <paramref name="oldValue"/> with.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is <c>empty</c>, instances of <paramref name="oldValue"/> are removed.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Replace(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue)
        => Replace(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">The string to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is <c>empty</c>, instances of <paramref name="oldValue"/> are removed.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Replace(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int startIndex, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, Length, nameof(count));

        if (oldValue.IsEmpty || oldValue.Equals(newValue, StringComparison.Ordinal))
        {
            return;
        }

        if (oldValue.Length == 1 && newValue.Length == 1)
        {
            Replace(oldValue[0], newValue[0], startIndex, count);
            return;
        }

        if (newValue.Length == oldValue.Length)
        {
            ReplaceEqualLength(oldValue, newValue, startIndex, count);
            return;
        }

        var matchCount = CountOccurrences(buffer.Slice(startIndex, count), oldValue, out var firstMatchOffset);
        if (matchCount == 0)
        {
            return;
        }

        if (matchCount == 1)
        {
            ReplaceSingle(oldValue, newValue, startIndex + firstMatchOffset);
            return;
        }

        if (newValue.Length < oldValue.Length)
        {
            ReplaceWithShorterValue(oldValue, newValue, startIndex, count);
            return;
        }

        Span<int> stackPositions = stackalloc int[Math.Min(matchCount, 128)];
        int[]? rentedPositions = null;
        var matchPositions = matchCount <= stackPositions.Length
            ? stackPositions[..matchCount]
            : (rentedPositions = ArrayPool<int>.Shared.Rent(matchCount)).AsSpan(0, matchCount);

        try
        {
            FillMatchPositions(buffer.Slice(startIndex, count), oldValue, matchPositions);
            ReplaceWithLongerValue(oldValue, newValue, startIndex, count, matchPositions);
        }
        finally
        {
            if (rentedPositions is not null)
            {
                ArrayPool<int>.Shared.Return(rentedPositions);
            }
        }
    }

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">Object to replace <paramref name="oldValue"/> with.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is from type <see cref="ISpanFormattable"/> an optimized version is taken.
    /// Otherwise the ToString method is called.
    /// </remarks>
    /// /// <typeparam name="T">Any type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReplaceGeneric<T>(scoped ReadOnlySpan<char> oldValue, T newValue)
        => ReplaceGeneric(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">Object to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is <see cref="ISpanFormattable"/>, <c>TryFormat</c> is used.
    /// Otherwise, <c>ToString</c> is used.
    /// </remarks>
    /// /// <typeparam name="T">Any type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReplaceGeneric<T>(scoped ReadOnlySpan<char> oldValue, T newValue, int startIndex, int count)
    {
        Span<char> tempBuffer = stackalloc char[128];
        if (TryFormatKnownSpanFormattable(newValue, tempBuffer, out var written)
            || (newValue is ISpanFormattable spanFormattable && spanFormattable.TryFormat(tempBuffer, out written, default, null)))
        {
            Replace(oldValue, tempBuffer[..written], startIndex, count);
            return;
        }

        Replace(oldValue, newValue?.ToString() ?? string.Empty, startIndex, count);
    }

    /// <summary>
    /// Formats <paramref name="value"/> into <paramref name="destination"/> for a fixed set of well known value
    /// types without boxing it. See <see cref="TryAppendKnownSpanFormattable{T}(T)"/> for the rationale.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatKnownSpanFormattable<T>(T value, Span<char> destination, out int charsWritten)
    {
        if (TryFormatKnownIntegralType(value, destination, out charsWritten))
        {
            return true;
        }

        return TryFormatKnownOtherType(value, destination, out charsWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountOccurrences(scoped ReadOnlySpan<char> value, scoped ReadOnlySpan<char> oldValue, out int firstMatchOffset)
    {
        var searchStart = 0;
        var matchCount = 0;
        firstMatchOffset = -1;

        while (searchStart < value.Length)
        {
            var matchIndex = value[searchStart..].IndexOf(oldValue, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return matchCount;
            }

            if (matchCount == 0)
            {
                firstMatchOffset = searchStart + matchIndex;
            }

            matchCount++;
            searchStart += matchIndex + oldValue.Length;
        }

        return matchCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillMatchPositions(scoped ReadOnlySpan<char> value, scoped ReadOnlySpan<char> oldValue, Span<int> matchPositions)
    {
        var searchStart = 0;
        var matchCount = 0;

        while (searchStart < value.Length)
        {
            var matchIndex = value[searchStart..].IndexOf(oldValue, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return;
            }

            matchPositions[matchCount] = searchStart + matchIndex;
            matchCount++;
            searchStart += matchIndex + oldValue.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReplaceSingle(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int index)
    {
        if (newValue.Length < oldValue.Length)
        {
            newValue.CopyTo(buffer[index..]);
            Remove(index + newValue.Length, oldValue.Length - newValue.Length);
            return;
        }

        newValue[..oldValue.Length].CopyTo(buffer[index..]);
        Insert(index + oldValue.Length, newValue[oldValue.Length..]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReplaceEqualLength(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int startIndex, int count)
    {
        var index = startIndex;
        var remainingChars = count;

        while (remainingChars > 0)
        {
            var foundSubIndex = buffer.Slice(index, remainingChars).IndexOf(oldValue, StringComparison.Ordinal);
            if (foundSubIndex < 0)
            {
                return;
            }

            index += foundSubIndex;
            newValue.CopyTo(buffer[index..]);
            index += oldValue.Length;
            remainingChars -= foundSubIndex + oldValue.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReplaceWithShorterValue(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int startIndex, int count)
    {
        var sourceEnd = startIndex + count;
        var sourceIndex = startIndex;
        var destinationIndex = startIndex;

        while (sourceIndex < sourceEnd)
        {
            var matchOffset = buffer.Slice(sourceIndex, sourceEnd - sourceIndex).IndexOf(oldValue, StringComparison.Ordinal);
            if (matchOffset < 0)
            {
                break;
            }

            var matchIndex = sourceIndex + matchOffset;
            buffer.Slice(sourceIndex, matchOffset).CopyTo(buffer[destinationIndex..]);
            destinationIndex += matchOffset;
            newValue.CopyTo(buffer[destinationIndex..]);
            destinationIndex += newValue.Length;
            sourceIndex = matchIndex + oldValue.Length;
        }

        var remainingLength = sourceEnd - sourceIndex;
        buffer.Slice(sourceIndex, remainingLength).CopyTo(buffer[destinationIndex..]);
        destinationIndex += remainingLength;

        var suffixLength = bufferPosition - sourceEnd;
        buffer.Slice(sourceEnd, suffixLength).CopyTo(buffer[destinationIndex..]);
        bufferPosition = destinationIndex + suffixLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReplaceWithLongerValue(scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int startIndex, int count, scoped ReadOnlySpan<int> matchPositions)
    {
        var oldLength = bufferPosition;
        var newLength = checked(oldLength + ((newValue.Length - oldValue.Length) * matchPositions.Length));
        EnsureCapacity(newLength);

        var sourceEnd = startIndex + count;
        var suffixLength = oldLength - sourceEnd;
        var destinationIndex = newLength - suffixLength;
        buffer.Slice(sourceEnd, suffixLength).CopyTo(buffer[destinationIndex..]);

        var searchEnd = sourceEnd;
        for (var i = matchPositions.Length - 1; i >= 0; i--)
        {
            var matchIndex = startIndex + matchPositions[i];
            var textAfterMatchLength = searchEnd - (matchIndex + oldValue.Length);
            destinationIndex -= textAfterMatchLength;
            buffer.Slice(matchIndex + oldValue.Length, textAfterMatchLength).CopyTo(buffer[destinationIndex..]);
            destinationIndex -= newValue.Length;
            newValue.CopyTo(buffer[destinationIndex..]);
            searchEnd = matchIndex;
        }

        var prefixLength = searchEnd - startIndex;
        destinationIndex -= prefixLength;
        buffer.Slice(startIndex, prefixLength).CopyTo(buffer[destinationIndex..]);
        bufferPosition = newLength;
    }

#pragma warning disable SA1204
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatKnownIntegralType<T>(T value, Span<char> destination, out int charsWritten)
    {
        var culture = CultureInfo.CurrentCulture;
        if (typeof(T) == typeof(byte))
        {
            return Unsafe.As<T, byte>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(sbyte))
        {
            return Unsafe.As<T, sbyte>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(short))
        {
            return Unsafe.As<T, short>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(ushort))
        {
            return Unsafe.As<T, ushort>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(int))
        {
            return Unsafe.As<T, int>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(uint))
        {
            return Unsafe.As<T, uint>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(long))
        {
            return Unsafe.As<T, long>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(ulong))
        {
            return Unsafe.As<T, ulong>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(Int128))
        {
            return Unsafe.As<T, Int128>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(UInt128))
        {
            return Unsafe.As<T, UInt128>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        charsWritten = 0;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatKnownOtherType<T>(T value, Span<char> destination, out int charsWritten)
    {
        var culture = CultureInfo.CurrentCulture;
        if (typeof(T) == typeof(float))
        {
            return Unsafe.As<T, float>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(double))
        {
            return Unsafe.As<T, double>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(decimal))
        {
            return Unsafe.As<T, decimal>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(DateTime))
        {
            return Unsafe.As<T, DateTime>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(DateTimeOffset))
        {
            return Unsafe.As<T, DateTimeOffset>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(TimeSpan))
        {
            return Unsafe.As<T, TimeSpan>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        if (typeof(T) == typeof(Guid))
        {
            return Unsafe.As<T, Guid>(ref value).TryFormat(destination, out charsWritten);
        }

        if (typeof(T) == typeof(Half))
        {
            return Unsafe.As<T, Half>(ref value).TryFormat(destination, out charsWritten, default, culture);
        }

        charsWritten = 0;
        return false;
    }
#pragma warning restore SA1204
}