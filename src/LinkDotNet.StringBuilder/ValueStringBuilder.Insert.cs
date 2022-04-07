namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Insert the string representation of the boolean to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, bool value) => Insert(index, value.ToString());

    /// <summary>
    /// Insert the string representation of the char to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, char value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the signed byte to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, sbyte value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the byte to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, byte value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the short to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, short value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the integer to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, int value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the long to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, long value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the float to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, float value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the double to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, double value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Insert the string representation of the decimal to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, decimal value) => InsertSpanFormattable(index, value);

    /// <summary>
    /// Appends the string representation of the boolean to the builder.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, ReadOnlySpan<char> value)
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

    private void InsertSpanFormattable<T>(int index, T value)
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

        Span<char> tempBuffer = stackalloc char[24];
        if (value.TryFormat(tempBuffer, out var written, default, null))
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