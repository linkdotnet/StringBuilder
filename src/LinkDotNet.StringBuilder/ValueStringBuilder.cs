using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// A string builder which minimizes as many heap allocations as possible.
/// </summary>
/// <remarks>
/// This is a ref struct which has certain limitations. You can only store it in a local variable or another ref struct.<br/><br/>
/// You should dispose it after use to ensure the rented buffer is returned to the array pool.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
[SkipLocalsInit]
public ref partial struct ValueStringBuilder : IDisposable
{
    private int bufferPosition;
    private Span<char> buffer;
    private char[]? arrayFromPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct using a rented buffer of capacity 32.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder()
    {
        EnsureCapacity(32);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">Initial buffer for the string builder to begin with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET9_0_OR_GREATER
    [OverloadResolutionPriority(1)]
#endif
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        buffer = initialBuffer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialText">The initial text used to initialize this instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder(ReadOnlySpan<char> initialText)
    {
        Append(initialText);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity that will be allocated for this instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder(int initialCapacity)
    {
        EnsureCapacity(initialCapacity);
    }

    /// <summary>
    /// Gets the current length of the represented string.
    /// </summary>
    /// <value>
    /// The current length of the represented string.
    /// </value>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition;
    }

    /// <summary>
    /// Gets the current maximum capacity before the span must be resized.
    /// </summary>
    /// <value>
    /// The current maximum capacity before the span must be resized.
    /// </value>
    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length;
    }

    /// <summary>
    /// Returns the character at the given index or throws an <see cref="IndexOutOfRangeException"/> if the index is bigger than the string.
    /// </summary>
    /// <param name="index">Character position to be retrieved.</param>
    public readonly ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref buffer[index];
    }

    /// <summary>
    /// Defines the implicit conversion of a <see cref="string"/> to <see cref="ValueStringBuilder"/>.
    /// </summary>
    /// <param name="fromString">The string as initial buffer.</param>
#pragma warning disable CA2225
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueStringBuilder(string fromString) => new(fromString);
#pragma warning restore CA2225

    /// <summary>
    /// Defines the implicit conversion of a <see cref="ReadOnlySpan{Char}"/> to <see cref="ValueStringBuilder"/>.
    /// </summary>
    /// <param name="fromString">The string as initial buffer.</param>
#pragma warning disable CA2225
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueStringBuilder(ReadOnlySpan<char> fromString) => new(fromString);
#pragma warning restore CA2225

    /// <summary>
    /// Creates a <see cref="string"/> instance from the builder.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => AsSpan().ToString();

    /// <summary>
    /// Creates a <see cref="string"/> instance from the builder.
    /// </summary>
    /// <param name="startIndex">The starting position of the substring in this instance.</param>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(int startIndex) => AsSpan(startIndex).ToString();

    /// <summary>
    /// Creates a <see cref="string"/> instance from the builder.
    /// </summary>
    /// <param name="startIndex">The starting position of the substring in this instance.</param>
    /// <param name="length">The length of the substring.</param>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(int startIndex, int length) => AsSpan(startIndex, length).ToString();

    /// <summary>
    /// Creates a <see cref="string"/> instance from the builder in the given range.
    /// </summary>
    /// <param name="range">The range to be retrieved.</param>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(Range range) => AsSpan(range).ToString();

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="startIndex">The starting position of the substring in this instance.</param>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan(int startIndex) => buffer[startIndex..bufferPosition];

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="startIndex">The starting position of the substring in this instance.</param>
    /// <param name="length">The length of the substring.</param>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan(int startIndex, int length)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, bufferPosition);

        return buffer.Slice(startIndex, length);
    }

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="range">The range to be retrieved.</param>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan(Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(bufferPosition);
        return AsSpan(offset, length);
    }

    /// <summary>
    /// Gets a pinnable reference to the represented string from this builder.
    /// The content after <see cref="Length"/> is not guaranteed to be null terminated.
    /// </summary>
    /// <returns>The pointer to the first instance of the string represented by this builder.</returns>
    /// <remarks>
    /// This method is used for use-cases where the user wants to use "fixed" calls like the following:
    /// <code>
    /// using var stringBuilder = new ValueStringBuilder();
    /// stringBuilder.Append("Hello World");
    /// fixed (var* buffer = stringBuilder) { ... }
    /// </code>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref char GetPinnableReference() => ref MemoryMarshal.GetReference(buffer);

    /// <summary>
    /// Tries to copy the represented string into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The destination where the internal string is copied into.</param>
    /// <returns>True, if the copy was successful, otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    /// <summary>
    /// Clears the internal representation of the string.
    /// </summary>
    /// <remarks>
    /// This will not enforce some re-allocation or shrinking of the internal buffer. The size stays the same.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => bufferPosition = 0;

    /// <summary>
    /// Removes a range of characters from this builder.
    /// </summary>
    /// <param name="startIndex">The inclusive index from where the string gets removed.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <remarks>
    /// This method will not affect the internal size of the string.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(int startIndex, int length)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + length, Length);

        if (length == 0)
        {
            return;
        }

        var beginIndex = startIndex + length;
        buffer[beginIndex..bufferPosition].CopyTo(buffer[startIndex..]);
        bufferPosition -= length;
    }

    /// <summary>
    /// Returns the index within this string of the first occurrence of the specified substring.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int IndexOf(ReadOnlySpan<char> word) => IndexOf(word, 0);

    /// <summary>
    /// Returns the index within this string of the first occurrence of the specified substring, starting at the specified index.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <param name="startIndex">Index to begin with.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int IndexOf(ReadOnlySpan<char> word, int startIndex)
    {
        return buffer[startIndex..bufferPosition].IndexOf(word);
    }

    /// <summary>
    /// Returns the index within this string of the last occurrence of the specified substring.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int LastIndexOf(ReadOnlySpan<char> word) => LastIndexOf(word, 0);

    /// <summary>
    /// Returns the index within this string of the last occurrence of the specified substring, starting at the specified index.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <param name="startIndex">Index to begin with.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int LastIndexOf(ReadOnlySpan<char> word, int startIndex)
    {
        return buffer[startIndex..bufferPosition].LastIndexOf(word);
    }

    /// <summary>
    /// Returns whether a specified substring occurs within this string.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <returns>True if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
    /// <remarks>
    /// This method performs an ordinal (case-sensitive and culture-insensitive) comparison.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(ReadOnlySpan<char> word) => IndexOf(word) != -1;

    /// <summary>
    /// Returns whether the characters in this builder are equal to the characters in the given span.
    /// </summary>
    /// <param name="span">The character span to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the characters are equal to this instance, otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(ReadOnlySpan<char> span) => span.Equals(AsSpan(), StringComparison.Ordinal);

    /// <summary>
    /// Returns whether the characters in this builder are equal to the characters in the given span according to the given comparison type.
    /// </summary>
    /// <param name="span">The character span to compare with the current instance.</param>
    /// <param name="comparisonType">The way to compare the sequences of characters.</param>
    /// <returns><see langword="true"/> if the characters are equal to this instance, otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(ReadOnlySpan<char> span, StringComparison comparisonType) => span.Equals(AsSpan(), comparisonType);

    /// <summary>
    /// Disposes the instance and returns the rented buffer to the array pool if needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (arrayFromPool is not null)
        {
            ArrayPool<char>.Shared.Return(arrayFromPool);
        }

        this = default;
    }

    /// <summary>
    /// Reverses the sequence of elements in this instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Reverse() => buffer[..bufferPosition].Reverse();
}