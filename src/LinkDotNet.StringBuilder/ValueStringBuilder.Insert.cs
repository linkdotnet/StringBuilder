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
    /// <param name="value">Character to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, char value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the signed byte to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Signed byte to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, sbyte value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the byte to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Byte to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, byte value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the short to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Short to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, short value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the integer to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Integer to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, int value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the long to the builder at the given index.
    /// </summary>
    /// <param name="index">Long where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, long value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the float to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Float to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, float value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the double to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Double to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, double value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the decimal to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Decimal to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, decimal value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

    /// <summary>
    /// Insert the string representation of the Guid to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Guid to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, Guid value, ReadOnlySpan<char> format = default) => InsertSpanFormattable(index, value, format);

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

        bufferPosition += value.Length;
        if (bufferPosition > buffer.Length)
        {
            Grow(bufferPosition * 2);
        }

        // Move Slice at beginning index
        var oldPosition = bufferPosition - value.Length;
        var shift = index + value.Length;
        buffer[index..oldPosition].CopyTo(buffer[shift..bufferPosition]);

        // Add new word
        value.CopyTo(buffer[index..shift]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InsertSpanFormattable<T>(int index, T value, ReadOnlySpan<char> format = default)
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

        Span<char> tempBuffer = stackalloc char[36];
        if (value.TryFormat(tempBuffer, out var written, format, null))
        {
            bufferPosition += written;
            if (bufferPosition > buffer.Length)
            {
                Grow(bufferPosition * 2);
            }

            // Move Slice at beginning index
            var oldPosition = bufferPosition - written;
            var shift = index + written;
            buffer[index..oldPosition].CopyTo(buffer[shift..bufferPosition]);

            // Add new word
            tempBuffer[..written].CopyTo(buffer[index..shift]);
        }
    }
}