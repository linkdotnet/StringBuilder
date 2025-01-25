using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Appends the string representation of the boolean.
    /// </summary>
    /// <param name="value">Bool value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Append(bool value)
    {
        const int trueLength = 4;
        const int falseLength = 5;

        var newSize = bufferPosition + falseLength;

        if (newSize > buffer.Length)
        {
            EnsureCapacity(newSize);
        }

        fixed (char* dest = &buffer[bufferPosition])
        {
            if (value)
            {
                *(dest + 0) = 'T';
                *(dest + 1) = 'r';
                *(dest + 2) = 'u';
                *(dest + 3) = 'e';
                bufferPosition += trueLength;
            }
            else
            {
                *(dest + 0) = 'F';
                *(dest + 1) = 'a';
                *(dest + 2) = 'l';
                *(dest + 3) = 's';
                *(dest + 4) = 'e';
                bufferPosition += falseLength;
            }
        }
    }

    /// <summary>
    /// Appends the string representation of the value.
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
    /// Appends a string.
    /// </summary>
    /// <param name="str">String to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> str)
    {
        var newSize = str.Length + bufferPosition;
        if (newSize > buffer.Length)
        {
            EnsureCapacity(newSize);
        }

        ref var strRef = ref MemoryMarshal.GetReference(str);
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer[bufferPosition..]);
        Unsafe.CopyBlock(
            ref Unsafe.As<char, byte>(ref bufferRef),
            ref Unsafe.As<char, byte>(ref strRef),
            (uint)(str.Length * sizeof(char)));

        bufferPosition += str.Length;
    }

    /// <summary>
    /// Appends a character buffer.
    /// </summary>
    /// <param name="value">The pointer to the start of the buffer.</param>
    /// <param name="length">The number of characters in the buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Append(char* value, int length)
    {
        Append(new ReadOnlySpan<char>(value, length));
    }

    /// <summary>
    /// Appends a slice of memory.
    /// </summary>
    /// <param name="memory">The memory to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlyMemory<char> memory)
    {
        Append(memory.Span);
    }

    /// <summary>
    /// Appends a single character.
    /// </summary>
    /// <param name="value">Character to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
        var newSize = bufferPosition + 1;
        if (newSize > buffer.Length)
        {
            EnsureCapacity(newSize);
        }

        buffer[bufferPosition] = value;
        bufferPosition++;
    }

    /// <summary>
    /// Appends a single rune to the string builder.
    /// </summary>
    /// <param name="value">Rune to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(Rune value)
    {
        Span<char> valueChars = stackalloc char[2];
        var valueCharsWritten = value.EncodeToUtf16(valueChars);
        ReadOnlySpan<char> valueCharsSlice = valueChars[..valueCharsWritten];

        Append(valueCharsSlice);
    }

    /// <summary>
    /// Appends <see cref="Environment.NewLine"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Calls <see cref="Append(ReadOnlySpan{char})"/> and appends <see cref="Environment.NewLine"/>.
    /// </summary>
    /// <param name="str">String to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(scoped ReadOnlySpan<char> str)
    {
        Append(str);
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Appends a span of the given length, which can be written to later.
    /// </summary>
    /// <param name="length">Integer representing the number of characters to be appended.</param>
    /// <returns>A span with the characters appended.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var origPos = bufferPosition;
        if (origPos > buffer.Length - length)
        {
            EnsureCapacity(length);
        }

        bufferPosition = origPos + length;
        return buffer.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendSpanFormattable<T>(T value, ReadOnlySpan<char> format = default, int bufferSize = 36)
        where T : ISpanFormattable
    {
        var newSize = bufferSize + bufferPosition;
        if (newSize >= Capacity)
        {
            EnsureCapacity(newSize);
        }

        if (!value.TryFormat(buffer[bufferPosition..], out var written, format, null))
        {
            throw new InvalidOperationException($"Could not insert {value} into given buffer. Is the buffer (size: {bufferSize}) large enough?");
        }

        bufferPosition += written;
    }
}