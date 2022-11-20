using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Appends the string representation of the boolean to the builder.
    /// </summary>
    /// <param name="value">Bool value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(bool value) => Append(value.ToString());

    /// <summary>
    /// Appends the string representation of the character to the builder.
    /// </summary>
    /// <param name="value">Formattable span to add.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    /// <param name="bufferSize">Size of the buffer allocated on the stack.</param>
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
        Span<char> tempBuffer = stackalloc char[bufferSize];
        if (value.TryFormat(tempBuffer, out var written, format, null))
        {
            var newSize = written + bufferPosition;
            if (newSize >= buffer.Length)
            {
                Grow();
            }

            tempBuffer[..written].CopyTo(buffer[bufferPosition..]);
            bufferPosition = newSize;
        }
        else
        {
            throw new InvalidOperationException($"Could not insert {value} into given buffer. Is the buffer (size: {bufferSize}) large enough?");
        }
    }
}