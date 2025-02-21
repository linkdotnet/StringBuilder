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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, Length);

        for (var i = startIndex; i < startIndex + count; i++)
        {
            if (buffer[i] == oldValue)
            {
                buffer[i] = newValue;
            }
        }
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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, Length);

        if (oldValue.IsEmpty || oldValue.Equals(newValue, StringComparison.Ordinal))
        {
            return;
        }

        var index = startIndex;
        var remainingChars = count;

        while (remainingChars > 0)
        {
            var foundSubIndex = buffer.Slice(index, remainingChars).IndexOf(oldValue, StringComparison.Ordinal);
            if (foundSubIndex < 0)
            {
                break;
            }

            index += foundSubIndex;
            remainingChars -= foundSubIndex;

            if (newValue.Length == oldValue.Length)
            {
                // Just replace the old slice
                newValue.CopyTo(buffer[index..]);
            }
            else if (newValue.Length < oldValue.Length)
            {
                // Replace the old slice and trim the unused slice
                newValue.CopyTo(buffer[index..]);
                Remove(index + newValue.Length, oldValue.Length - newValue.Length);
            }
            else
            {
                // Replace the old slice and append the extra slice
                newValue[..oldValue.Length].CopyTo(buffer[index..]);
                Insert(index + oldValue.Length, newValue[oldValue.Length..]);
            }

            index += newValue.Length;
            remainingChars -= oldValue.Length;
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
    /// If <paramref name="newValue"/> is from type <see cref="ISpanFormattable"/> an optimized version is taken.
    /// Otherwise the ToString method is called.
    /// </remarks>
    /// /// <typeparam name="T">Any type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReplaceGeneric<T>(scoped ReadOnlySpan<char> oldValue, T newValue, int startIndex, int count)
    {
        if (newValue is ISpanFormattable spanFormattable)
        {
            Span<char> tempBuffer = stackalloc char[128];
            if (spanFormattable.TryFormat(tempBuffer, out var written, default, null))
            {
                Replace(oldValue, tempBuffer[..written], startIndex, count);
                return;
            }
        }

        Replace(oldValue, (newValue?.ToString() ?? string.Empty).AsSpan(), startIndex, count);
    }
}