using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Appends the string representation of the boolean to the builder.
    /// </summary>
    /// <param name="value">Bool value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(bool value)
    {
        // 5 is the length of the string "False"
        // So we can check if we have enough space in the buffer
        if (bufferPosition + 5 > buffer.Length)
        {
            Grow(bufferPosition * 2);
        }

        if (!value.TryFormat(buffer[bufferPosition..], out var charsWritten))
        {
            throw new InvalidOperationException($"Could not add {value} to the builder.");
        }

        bufferPosition += charsWritten;
    }

    /// <summary>
    /// Appends the string representation of the character to the builder.
    /// </summary>
    /// <param name="value">Formattable span to add.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    /// <param name="bufferSize">Size of the buffer allocated. If you have a custom type that implements <see cref="ISpanFormattable"/> that
    /// requires more space than the default (36 characters), adjust the value.</param>
    /// <typeparam name="T">Any <see cref="ISpanFormattable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append<T>(T value, ReadOnlySpan<char> format = default, int bufferSize = 36)
        where T : ISpanFormattable => AppendSpanFormattable(value, format, bufferSize);

    /// <summary>
    /// Appends a string to the string builder.
    /// </summary>
    /// <param name="str">String, which will be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> str)
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
    /// Appends a character buffer to this builder.
    /// </summary>
    /// <param name="value">The pointer to the start of the buffer.</param>
    /// <param name="length">The number of characters in the buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Append(char* value, int length)
    {
        Append(new ReadOnlySpan<char>(value, length));
    }

    /// <summary>
    /// Adds the default new line separator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Appends a slice of memory.
    /// </summary>
    /// <param name="memory">The memory to add.</param>
    public void Append(ReadOnlyMemory<char> memory)
    {
        Append(memory.Span);
    }

    /// <summary>
    /// Does the same as <see cref="Append(char)"/> but adds a newline at the end.
    /// </summary>
    /// <param name="str">String, which will be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(scoped ReadOnlySpan<char> str)
    {
        Append(str);
        Append(Environment.NewLine);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendSpanFormattable<T>(T value, ReadOnlySpan<char> format = default, int bufferSize = 36)
        where T : ISpanFormattable
    {
        if (bufferSize + bufferPosition >= Capacity)
        {
            Grow(bufferSize + bufferPosition);
        }

        if (!value.TryFormat(buffer[bufferPosition..], out var written, format, null))
        {
            throw new InvalidOperationException($"Could not insert {value} into given buffer. Is the buffer (size: {bufferSize}) large enough?");
        }

        bufferPosition += written;
    }
}