using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

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
    /// Insert the string representation of the character to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Character to insert into this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, char value) => Insert(index, [value]);

    /// <summary>
    /// Insert the string representation of the rune to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Rune to insert into this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, Rune value)
    {
        if (value.IsBmp)
        {
            Insert(index, (char)value.Value);
            return;
        }

        Span<char> valueChars = stackalloc char[2];
        var valueCharsWritten = value.EncodeToUtf16(valueChars);
        ReadOnlySpan<char> valueCharsSlice = valueChars[..valueCharsWritten];

        Insert(index, valueCharsSlice);
    }

    /// <summary>
    /// Insert the string representation of the value to the builder at the given index.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">Formattable value to insert into this builder.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    /// <param name="bufferSize">Size of the buffer allocated on the stack.</param>
    /// <param name="formatProvider">Optional format provider.</param>
    /// <typeparam name="T">Any <see cref="ISpanFormattable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert<T>(int index, T value, scoped ReadOnlySpan<char> format = default, int bufferSize = DefaultFormatBufferSize, IFormatProvider? formatProvider = null)
        where T : ISpanFormattable => InsertSpanFormattable(index, value, format, bufferSize, formatProvider);

    /// <summary>
    /// Insert the given span into the builder at the given index.
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

        if (value.IsEmpty)
        {
            return;
        }

        // The shift below (or a grow) would overwrite a value that points into this builder before it is copied.
        if (value.Overlaps(buffer))
        {
            var copy = ArrayPool<char>.Shared.Rent(value.Length);
            value.CopyTo(copy);
            Insert(index, copy.AsSpan(0, value.Length));
            ArrayPool<char>.Shared.Return(copy);
            return;
        }

        var newLength = bufferPosition + value.Length;
        if (newLength > buffer.Length)
        {
            EnsureCapacity(newLength);
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
    private void InsertSpanFormattable<T>(int index, T value, scoped ReadOnlySpan<char> format, int bufferSize, IFormatProvider? formatProvider = null)
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

        const int maxStackBufferSize = 256;
        char[]? rented = null;
        var tempBuffer = bufferSize <= maxStackBufferSize
            ? stackalloc char[bufferSize]
            : rented = ArrayPool<char>.Shared.Rent(bufferSize);

        try
        {
            int written;
            while (!value.TryFormat(tempBuffer, out written, format, formatProvider))
            {
                if (bufferSize != DefaultFormatBufferSize)
                {
                    throw new InvalidOperationException($"Could not insert {value} into given buffer. Is the buffer (size: {bufferSize}) large enough?");
                }

                var larger = ArrayPool<char>.Shared.Rent(checked(tempBuffer.Length * 2));
                if (rented is not null)
                {
                    ArrayPool<char>.Shared.Return(rented);
                }

                tempBuffer = rented = larger;
            }

            Insert(index, tempBuffer[..written]);
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }
}