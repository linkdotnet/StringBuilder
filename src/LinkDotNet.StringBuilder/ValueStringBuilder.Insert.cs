using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Insert the string representation of the boolean to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Boolean to insert into this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, bool value) => Insert(index, value.ToString());

    /// <summary>
    /// Insert the string representation of the char to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Formattable span to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    /// <param name="bufferSize">Size of the buffer allocated on the stack.</param>
    /// <typeparam name="T">Any <see cref="ISpanFormattable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert<T>(int index, T value, ReadOnlySpan<char> format = default, int bufferSize = 36)
        where T : ISpanFormattable => InsertSpanFormattable(index, value, format, bufferSize);

    /// <summary>
    /// Appends the string representation of the boolean to the builder.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, scoped ReadOnlySpan<char> value)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The given index can't be negative.");
        }

        if (index > bufferPosition)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The given index can't be bigger than the string itself.");
        }

        var newLength = bufferPosition + value.Length;
        if (newLength > buffer.Length)
        {
            Grow(newLength);
        }

        bufferPosition = newLength;

        // Move Slice at beginning index
        var oldPosition = bufferPosition - value.Length;
        var shift = index + value.Length;
        buffer[index..oldPosition].CopyTo(buffer[shift..bufferPosition]);

        // Add new word
        value.CopyTo(buffer[index..shift]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InsertSpanFormattable<T>(int index, T value, ReadOnlySpan<char> format, int bufferSize)
        where T : ISpanFormattable
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The given index can't be negative.");
        }

        if (index > bufferPosition)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The given index can't be bigger than the string itself.");
        }

        Span<char> tempBuffer = stackalloc char[bufferSize];
        if (value.TryFormat(tempBuffer, out var written, format, null))
        {
            var newLength = bufferPosition + written;
            if (newLength > buffer.Length)
            {
                Grow(newLength);
            }

            bufferPosition = newLength;

            // Move Slice at beginning index
            var oldPosition = bufferPosition - written;
            var shift = index + written;
            buffer[index..oldPosition].CopyTo(buffer[shift..bufferPosition]);

            // Add new word
            tempBuffer[..written].CopyTo(buffer[index..shift]);
        }
        else
        {
            throw new InvalidOperationException($"Could not insert {value} into given buffer. Is the buffer (size: {bufferSize}) large enough?");
        }
    }
}