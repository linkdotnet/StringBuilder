using System.Buffers;

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
    /// Returns the character at the given index or throws an <see cref="IndexOutOfRangeException"/> if the index is bigger than the string.
    /// </summary>
    /// <param name="index">Index position, which should be retrieved.</param>
    public ref char this[int index] => ref buffer[index];

    /// <summary>
    /// Adds one character to the string builder.
    /// </summary>
    /// <param name="c">Character to append.</param>
    public void Append(char c)
    {
        if (bufferPosition == buffer.Length - 1)
        {
            Grow();
        }

        buffer[bufferPosition++] = c;
    }

    public void Append(bool b) => Append(b.ToString());

    /// <summary>
    /// Appends a string to the string builder.
    /// </summary>
    /// <param name="str">String, which will be added to this builder.</param>
    public void Append(ReadOnlySpan<char> str)
    {
        var newSize = str.Length + bufferPosition;
        if (newSize > buffer.Length)
        {
            Grow(newSize * 2);
        }

        str.CopyTo(buffer[bufferPosition..]);
        bufferPosition += str.Length;
    }

    /// <summary>
    /// Adds the default new line separator.
    /// </summary>
    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Does the same as <see cref="Append(char)"/> but adds a newline at the end.
    /// </summary>
    /// <param name="str">String, which will be added to this builder.</param>
    public void AppendLine(ReadOnlySpan<char> str)
    {
        Append(str);
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Creates a <see cref="string"/> instance from that builder.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    public override string ToString() => new(buffer[..bufferPosition]);

    /// <summary>
    /// Creates a <see cref="string"/> instance from that builder.
    /// </summary>
    /// <param name="start">Starting index of the string.</param>
    /// <param name="length">Length of the string.</param>
    /// <returns>The substring with the given boundaries.</returns>
    public string ToString(int start, int length)
    {
        var endIndex = start + length;
        return new(buffer[start..endIndex]);
    }

    /// <summary>
    /// Returns the string as an <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The filled array as <see cref="ReadOnlySpan{T}"/>.</returns>
    public ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Tries to copy the represented string into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The destination where the internal string is copied into.</param>
    /// <returns>True, if the copy was successful, otherwise false.</returns>
    public bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    private void Grow(int capacity = 0)
    {
        var currentSize = buffer.Length;
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = ArrayPool<char>.Shared.Rent(newSize);
        buffer.CopyTo(rented);
        buffer = rented;
        ArrayPool<char>.Shared.Return(rented);
    }
}