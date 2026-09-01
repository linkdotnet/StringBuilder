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
}