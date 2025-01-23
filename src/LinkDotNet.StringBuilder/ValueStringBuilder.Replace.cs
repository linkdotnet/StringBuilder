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
        int oldValueCharsWritten = oldValue.EncodeToUtf16(oldValueChars);
        ReadOnlySpan<char> oldValueCharsReadOnly = oldValueChars[..oldValueCharsWritten];

        Span<char> newValueChars = stackalloc char[2];
        int newValueCharsWritten = newValue.EncodeToUtf16(newValueChars);
        ReadOnlySpan<char> newValueCharsReadOnly = newValueChars[..newValueCharsWritten];

        Replace(oldValueCharsReadOnly, newValueCharsReadOnly, startIndex, count);
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

        var length = startIndex + count;
        var slice = buffer[startIndex..length];

        if (oldValue.SequenceEqual(newValue))
        {
            return;
        }

        // We might want to check whether or not we want to introduce different
        // string search algorithms for longer strings.
        // I had checked initially with Boyer-Moore but it didn't make that much sense as we
        // don't expect very long strings and then the performance is literally the same. So I went with the easier solution.
        var hits = NaiveSearch.FindAll(slice, oldValue);

        if (hits.IsEmpty)
        {
            return;
        }

        var delta = newValue.Length - oldValue.Length;

        for (var i = 0; i < hits.Length; i++)
        {
            var index = startIndex + hits[i] + (delta * i);

            // newValue is smaller than old value
            // We can insert the slice and remove the overhead
            if (delta < 0)
            {
                newValue.CopyTo(buffer[index..]);
                Remove(index + newValue.Length, -delta);
            }

            // Same length -> We can just replace the memory slice
            else if (delta == 0)
            {
                newValue.CopyTo(buffer[index..]);
            }

            // newValue is larger than the old value
            // First add until the old memory region
            // and insert afterwards the rest
            else
            {
                newValue[..oldValue.Length].CopyTo(buffer[index..]);
                Insert(index + oldValue.Length, newValue[oldValue.Length..]);
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
    public void ReplaceGeneric<T>(ReadOnlySpan<char> oldValue, T newValue)
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
    public void ReplaceGeneric<T>(ReadOnlySpan<char> oldValue, T newValue, int startIndex, int count)
    {
        if (newValue is ISpanFormattable spanFormattable)
        {
            Span<char> tempBuffer = stackalloc char[24];
            if (spanFormattable.TryFormat(tempBuffer, out var written, default, null))
            {
                Replace(oldValue, tempBuffer[..written], startIndex, count);
            }
        }
        else
        {
            Replace(oldValue, (newValue?.ToString() ?? string.Empty).AsSpan(), startIndex, count);
        }
    }
}