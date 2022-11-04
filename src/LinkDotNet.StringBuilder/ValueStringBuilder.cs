using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// Represents a string builder which tried to reduce as much allocations as possible.
/// </summary>
/// <remarks>
/// The <see cref="ValueStringBuilder"/> is declared as ref struct which brings certain limitations with it.
/// You can only use it in another ref struct or as a local variable.
/// </remarks>
public ref partial struct ValueStringBuilder
{
    private int bufferPosition;
    private Span<char> buffer;
    private char[]? arrayFromPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder()
    {
        bufferPosition = 0;
        buffer = default;
        arrayFromPool = null;
        Grow(32);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">Initial buffer for the string builder to begin with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        bufferPosition = 0;
        buffer = initialBuffer;
        arrayFromPool = null;
    }

    /// <summary>
    /// Gets the current length of the represented string.
    /// </summary>
    /// <value>
    /// The current length of the represented string.
    /// </value>
    public int Length => bufferPosition;

    /// <summary>
    /// Gets the current maximum capacity before growing the array.
    /// </summary>
    /// <value>
    /// The current maximum capacity before growing the array.
    /// </value>
    public int Capacity => buffer.Length;

    /// <summary>
    /// Returns the character at the given index or throws an <see cref="IndexOutOfRangeException"/> if the index is bigger than the string.
    /// </summary>
    /// <param name="index">Index position, which should be retrieved.</param>
    public ref char this[int index] => ref buffer[index];

    /// <summary>
    /// Creates a <see cref="string"/> instance from that builder.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => new(buffer[..bufferPosition]);

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Get a pinnable reference to the represented string from this builder.
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
    public ref char GetPinnableReference() => ref MemoryMarshal.GetReference(buffer);

    /// <summary>
    /// Tries to copy the represented string into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The destination where the internal string is copied into.</param>
    /// <returns>True, if the copy was successful, otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    /// <summary>
    /// Clears the internal representation of the string.
    /// </summary>
    /// <remarks>
    /// This will not enforce some re-allocation or shrinking of the internal buffer. The size stays the same.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        bufferPosition = 0;
    }

    /// <summary>
    /// Ensures that the builder has at least <paramref name="newCapacity"/> amount of capacity.
    /// </summary>
    /// <param name="newCapacity">New capacity for the builder.</param>
    /// <remarks>
    /// If <paramref name="newCapacity"/> is smaller or equal to <see cref="Length"/> nothing will be done.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity)
    {
        if (newCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newCapacity), "Capacity can't be negative.");
        }

        if (newCapacity > Length)
        {
            Grow(newCapacity);
        }
    }

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
        if (length == 0)
        {
            return;
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "The given length can't be negative.");
        }

        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex), "The given start index can't be negative.");
        }

        if (length > Length - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(length), $"The given Span ({startIndex}..{length})length is outside the the represented string.");
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
    public int IndexOf(ReadOnlySpan<char> word) => IndexOf(word, 0);

    /// <summary>
    /// Returns the index within this string of the first occurrence of the specified substring, starting at the specified index.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <param name="startIndex">Index to begin with.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(ReadOnlySpan<char> word, int startIndex)
    {
        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index can't be smaller than 0.");
        }

        if (word.IsEmpty)
        {
            return 0;
        }

        return NaiveSearch.FindFirst(buffer[startIndex..bufferPosition], word);
    }

    /// <summary>
    /// Returns the index within this string of the last occurrence of the specified substring.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LastIndexOf(ReadOnlySpan<char> word) => LastIndexOf(word, 0);

    /// <summary>
    /// Returns the index within this string of the last occurrence of the specified substring, starting at the specified index.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <param name="startIndex">Index to begin with.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LastIndexOf(ReadOnlySpan<char> word, int startIndex)
    {
        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index can't be smaller than 0.");
        }

        if (word.IsEmpty)
        {
            return 0;
        }

        return NaiveSearch.FindLast(buffer[startIndex..bufferPosition], word);
    }

    /// <summary>
    /// Returns a value indicating whether a specified substring occurs within this string.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <returns>True if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
    /// <remarks>
    /// This method performs an ordinal (case-sensitive and culture-insensitive) comparison.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(ReadOnlySpan<char> word) => IndexOf(word) != -1;

    /// <summary>
    /// Disposes the instance and returns rented buffer from an array pool if needed.
    /// </summary>
    public void Dispose()
    {
        var toReturn = arrayFromPool;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow(int capacity = 0)
    {
        var currentSize = buffer.Length;

        var newSize = capacity > currentSize ? capacity : currentSize * 2;
        var rented = ArrayPool<char>.Shared.Rent(newSize);
        buffer.CopyTo(rented);
        var toReturn = arrayFromPool;
        buffer = arrayFromPool = rented;

        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}