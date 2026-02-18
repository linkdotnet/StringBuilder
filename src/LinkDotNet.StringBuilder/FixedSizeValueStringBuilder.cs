using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// A string builder backed by a fixed-size buffer that never grows and never allocates on the heap.
/// </summary>
/// <remarks>
/// This is a ref struct which can only be stored in a local variable or another ref struct.<br/><br/>
/// When more characters are appended than the buffer can hold, the excess is silently truncated.
/// The buffer is typically stack-allocated via <c>stackalloc</c>:<br/>
/// <code>
/// Span&lt;char&gt; buffer = stackalloc char[32];
/// var builder = new FixedSizeValueStringBuilder(buffer);
/// builder.Append("Hello World");
/// string result = builder.ToString();
/// </code>
/// Unlike <see cref="ValueStringBuilder"/>, this type intentionally omits <see cref="IDisposable"/>
/// as a deliberate signal that it never touches the heap.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
[SkipLocalsInit]
public ref struct FixedSizeValueStringBuilder
{
    private readonly Span<char> buffer;
    private int bufferPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedSizeValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="buffer">The fixed-size buffer to write into. Typically stack-allocated via <c>stackalloc</c>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixedSizeValueStringBuilder(Span<char> buffer)
    {
        this.buffer = buffer;
    }

    /// <summary>
    /// Gets the number of characters currently written to the buffer.
    /// </summary>
    /// <value>
    /// The number of characters currently written to the buffer.
    /// </value>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition;
    }

    /// <summary>
    /// Gets the total capacity of the fixed buffer.
    /// </summary>
    /// <value>
    /// The total capacity of the fixed buffer.
    /// </value>
    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length;
    }

    /// <summary>
    /// Gets a value indicating whether no characters have been written.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the builder is empty; otherwise, <see langword="false"/>.
    /// </value>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition == 0;
    }

    /// <summary>
    /// Gets a value indicating whether the buffer is completely full and further appends will be no-ops.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the buffer is full; otherwise, <see langword="false"/>.
    /// </value>
    public readonly bool IsFull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition == buffer.Length;
    }

    /// <summary>
    /// Returns the character at the given index.
    /// </summary>
    /// <param name="index">Character position to retrieve.</param>
    public readonly ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref buffer[index];
    }

    /// <summary>
    /// Appends a string. Characters beyond the buffer capacity are silently truncated.
    /// </summary>
    /// <param name="str">String to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> str)
    {
        if (str.IsEmpty)
        {
            return;
        }

        var available = buffer.Length - bufferPosition;
        if (available <= 0)
        {
            return;
        }

        var toCopy = Math.Min(str.Length, available);
        ref var strRef = ref MemoryMarshal.GetReference(str);
        ref var bufferRef = ref MemoryMarshal.GetReference(buffer[bufferPosition..]);
        Unsafe.CopyBlock(
            ref Unsafe.As<char, byte>(ref bufferRef),
            ref Unsafe.As<char, byte>(ref strRef),
            (uint)(toCopy * sizeof(char)));

        bufferPosition += toCopy;
    }

    /// <summary>
    /// Appends a single character. No-op if the buffer is full.
    /// </summary>
    /// <param name="value">Character to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
        if (bufferPosition >= buffer.Length)
        {
            return;
        }

        buffer[bufferPosition] = value;
        bufferPosition++;
    }

    /// <summary>
    /// Appends the string representation of a <see cref="ISpanFormattable"/> value.
    /// Characters beyond the buffer capacity are silently truncated.
    /// </summary>
    /// <param name="value">The value to format and append.</param>
    /// <param name="format">Optional format string.</param>
    /// <param name="bufferSize">
    /// Intermediate buffer size used when formatting. Increase for types whose formatted
    /// representation can exceed 36 characters.
    /// </param>
    /// <param name="formatProvider">Format provider; defaults to <see cref="CultureInfo.InvariantCulture"/>.</param>
    /// <typeparam name="T">Any <see cref="ISpanFormattable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append<T>(T value, scoped ReadOnlySpan<char> format = default, int bufferSize = 36, IFormatProvider? formatProvider = null)
        where T : ISpanFormattable
    {
        var available = buffer.Length - bufferPosition;
        if (available <= 0)
        {
            return;
        }

        // Try to format directly into the remaining buffer space.
        if (value.TryFormat(buffer[bufferPosition..], out var written, format, formatProvider ?? CultureInfo.InvariantCulture))
        {
            bufferPosition += written;
            return;
        }

        // The remaining space was too small to hold the full formatted value.
        // Format into a temporary buffer and copy the truncated portion.
        Span<char> temp = bufferSize <= 256 ? stackalloc char[bufferSize] : new char[bufferSize];
        if (value.TryFormat(temp, out written, format, formatProvider ?? CultureInfo.InvariantCulture))
        {
            Append(temp[..written]);
        }
    }

    /// <summary>
    /// Appends <see cref="Environment.NewLine"/>. Truncates if the buffer cannot hold the full newline.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Appends <paramref name="str"/> followed by <see cref="Environment.NewLine"/>.
    /// Either part may be truncated if the buffer runs out of space.
    /// </summary>
    /// <param name="str">String to append before the newline.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(scoped ReadOnlySpan<char> str)
    {
        Append(str);
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Resets the builder so it can be reused from the start of the same buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => bufferPosition = 0;

    /// <summary>
    /// Returns the written content as a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The written content as a <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Tries to copy the written content into <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination">The target span.</param>
    /// <returns><see langword="true"/> if the copy succeeded; <see langword="false"/> if the destination is too small.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    /// <summary>
    /// Creates a <see cref="string"/> from the written content.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => AsSpan().ToString();
}
