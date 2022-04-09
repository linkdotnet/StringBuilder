using System.Buffers;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    public ValueStringBuilder()
    {
        bufferPosition = 0;
        buffer = new char[32];
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
    public override string ToString() => new(buffer[..bufferPosition]);

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    public ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Get a pinnable reference to the represented string from this builder.
    /// The content after <see cref="Length"/> is not guaranteed to be null terminated.
    /// </summary>
    /// <returns>The pointer to the first instance of the string represented by this builder.</returns>
    /// <remarks>
    /// This method is used for use-cases where the user wants to use "fixed" calls like the following:
    /// <code>
    /// var stringBuilder = new ValueStringBuilder();
    /// stringBuilder.Append("Hello World");
    /// fixed (var* buffer = stringBuilder) { ... }
    /// </code>
    /// </remarks>
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(buffer);
    }

    /// <summary>
    /// Tries to copy the represented string into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The destination where the internal string is copied into.</param>
    /// <returns>True, if the copy was successful, otherwise false.</returns>
    public bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    /// <summary>
    /// Clears the internal representation of the string.
    /// </summary>
    /// <remarks>
    /// This will not enforce some re-allocation or shrinking of the internal buffer. The size stays the same.
    /// </remarks>
    public void Clear()
    {
        bufferPosition = 0;
    }

    /// <summary>
    /// Removes a range of characters from this builder.
    /// </summary>
    /// <param name="startIndex">The inclusive index from where the string gets removed.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <remarks>
    /// This method will not affect the internal size of the string.
    /// </remarks>
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
    public int IndexOf(ReadOnlySpan<char> word)
    {
        return NaiveSearch.FindFirst(buffer[..bufferPosition], word);
    }

    /// <summary>
    /// Returns the index within this string of the first occurrence of the specified substring, starting at the specified index.
    /// </summary>
    /// <param name="word">Word to look for in this string.</param>
    /// <param name="startIndex">Index to begin with.</param>
    /// <returns>The index of the found <paramref name="word"/> in this string or -1 if not found.</returns>
    public int IndexOf(ReadOnlySpan<char> word, int startIndex)
    {
        if (startIndex < 0)
        {
            throw new ArgumentException("Start index can't be smaller than 0.", nameof(startIndex));
        }

        return NaiveSearch.FindFirst(buffer[startIndex..bufferPosition], word);
    }

    private void Grow(int capacity = 0)
    {
        var currentSize = buffer.Length;

        // This could lead to the potential problem that an user sets the capacity smaller than the current length
        // which would lead to a truncated string.
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = ArrayPool<char>.Shared.Rent(newSize);
        buffer.CopyTo(rented);
        buffer = rented;
        ArrayPool<char>.Shared.Return(rented);
    }
}