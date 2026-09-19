using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// A string builder backed by a fixed-size, caller-supplied buffer which never grows and never allocates on the heap.
/// </summary>
/// <remarks>
/// This is a ref struct which has certain limitations. You can only store it in a local variable or another ref struct.
/// <br/><br/>
/// Unlike <see cref="ValueStringBuilder"/>, this type never rents from an array pool. Appending follows two rules:
/// <list type="number">
/// <item><description>Every append is atomic - it either fits entirely or writes nothing at all.</description></item>
/// <item><description>The first append that does not fit sets <see cref="Overflowed"/>, after which every further
/// append is a no-op. The content is therefore always a valid prefix of what was intended.</description></item>
/// </list>
/// Because of the second rule a later, smaller append is dropped even when it would still fit. Call
/// <see cref="ClearOverflow"/> to deliberately carry on regardless.
/// <br/><br/>
/// There is no <see cref="IDisposable"/> implementation: nothing is ever rented, so there is nothing to return.
/// <code>
/// var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);
/// builder.Append("Hello World");
/// var result = builder.ToString();
/// </code>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public ref partial struct FixedSizeValueStringBuilder
{
    private Span<char> buffer;
    private int bufferPosition;
    private bool overflowed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedSizeValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="buffer">The buffer to write into. It is never replaced or resized, so its length is the hard
    /// upper bound for the content. Typically stack-allocated via <c>stackalloc</c>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixedSizeValueStringBuilder(Span<char> buffer) => this.buffer = buffer;

    /// <summary>
    /// Gets the number of characters written so far.
    /// </summary>
    /// <value>
    /// The number of characters written so far.
    /// </value>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition;
    }

    /// <summary>
    /// Gets the length of the buffer this instance was created with.
    /// </summary>
    /// <value>
    /// The length of the buffer this instance was created with.
    /// </value>
    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length;
    }

    /// <summary>
    /// Gets the number of characters which still fit into the buffer.
    /// </summary>
    /// <value>
    /// The number of characters which still fit into the buffer. This can be greater than zero while
    /// <see cref="Overflowed"/> is <see langword="true"/>, in which case nothing more will be written until
    /// <see cref="ClearOverflow"/> or <see cref="Clear"/> is called.
    /// </value>
    public readonly int Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length - bufferPosition;
    }

    /// <summary>
    /// Gets a value indicating whether nothing has been written yet.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if nothing has been written yet; otherwise, <see langword="false"/>.
    /// </value>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bufferPosition == 0;
    }

    /// <summary>
    /// Gets a value indicating whether an append did not fit and was therefore dropped.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if an append was dropped; otherwise, <see langword="false"/>. Once set, every further
    /// append is a no-op until <see cref="ClearOverflow"/> or <see cref="Clear"/> is called.
    /// </value>
    public readonly bool Overflowed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => overflowed;
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
    /// Appends a string. Dropped if it does not fit completely.
    /// </summary>
    /// <param name="str">String to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> str) => TryAppend(str);

    /// <summary>
    /// Appends a string. Dropped if it does not fit completely.
    /// </summary>
    /// <param name="value">The string to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? value) => TryAppend(value.AsSpan());

    /// <summary>
    /// Appends a single character. Dropped if the buffer has no room left.
    /// </summary>
    /// <param name="value">Character to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
        if (overflowed)
        {
            return;
        }

        if (bufferPosition == buffer.Length)
        {
            overflowed = true;
            return;
        }

        buffer[bufferPosition] = value;
        bufferPosition++;
    }

    /// <summary>
    /// Appends the string representation of a boolean. Dropped if it does not fit completely.
    /// </summary>
    /// <param name="value">Bool value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(bool value) => TryAppend(value ? bool.TrueString : bool.FalseString);

    /// <summary>
    /// Appends a single rune. Dropped if it does not fit completely, so a surrogate pair is never split.
    /// </summary>
    /// <param name="value">Rune to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(Rune value) => TryAppend(value);

    /// <summary>
    /// Appends the string representation of the value. Dropped if it does not fit completely, so a formatted value is
    /// never written out half-way.
    /// </summary>
    /// <param name="value">Formattable span to add.</param>
    /// <param name="format">Optional formatter. If not provided the default of the given instance is taken.</param>
    /// <param name="formatProvider">Optional format provider.</param>
    /// <typeparam name="T">Any <see cref="ISpanFormattable"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append<T>(T value, scoped ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
        where T : ISpanFormattable => TryAppend(value, format, formatProvider);

    /// <summary>
    /// Appends <see cref="Environment.NewLine"/>. Dropped if it does not fit completely.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine() => TryAppend(Environment.NewLine);

    /// <summary>
    /// Appends a string followed by <see cref="Environment.NewLine"/>. Both are dropped together unless both fit.
    /// </summary>
    /// <param name="str">String to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(scoped ReadOnlySpan<char> str) => TryAppendLine(str);

    /// <summary>
    /// Appends a string followed by <see cref="Environment.NewLine"/>. Both are dropped together unless both fit.
    /// </summary>
    /// <param name="value">The string to be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(string? value) => TryAppendLine(value.AsSpan());

    /// <summary>
    /// Discards the written content and resets <see cref="Overflowed"/> so the buffer can be reused.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        bufferPosition = 0;
        overflowed = false;
    }

    /// <summary>
    /// Resets <see cref="Overflowed"/> while keeping the written content, so appending continues into whatever room is
    /// left.
    /// </summary>
    /// <remarks>
    /// Read <see cref="Overflowed"/> before calling this if you need to know whether anything was actually dropped.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearOverflow() => overflowed = false;

    /// <summary>
    /// Returns the written content as a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The written content as a <see cref="ReadOnlySpan{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> AsSpan() => buffer[..bufferPosition];

    /// <summary>
    /// Tries to copy the written content into the given <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The destination to copy the content into.</param>
    /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryCopyTo(Span<char> destination) => buffer[..bufferPosition].TryCopyTo(destination);

    /// <summary>
    /// Creates a <see cref="string"/> instance from the written content.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => AsSpan().ToString();

    /// <summary>
    /// Hands the buffer and its content over to a <see cref="ValueStringBuilder"/> which can grow beyond the fixed
    /// capacity, and consumes this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueStringBuilder"/> continuing where this instance left off. Nothing is copied and nothing is
    /// rented, so the move itself never allocates. Dispose the result as usual.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <see cref="Overflowed"/> is <see langword="true"/>. The content is an incomplete prefix and
    /// <see cref="ValueStringBuilder"/> has nowhere to carry that information. Call <see cref="ClearOverflow"/> first
    /// if the truncation was intended.
    /// </exception>
    /// <remarks>
    /// Both builders would otherwise write into the same memory, so this instance is left consumed: an empty builder
    /// with zero capacity whose <see cref="Overflowed"/> is <see langword="true"/>. Reading it is safe, and any
    /// further append is a no-op rather than a write into a buffer somebody else now owns.
    /// </remarks>
    public ValueStringBuilder MoveToValueStringBuilder()
    {
        if (overflowed)
        {
            throw new InvalidOperationException(
                "Cannot move an overflowed FixedSizeValueStringBuilder. Call ClearOverflow() first if the dropped content is acceptable.");
        }

        var moved = new ValueStringBuilder(buffer, bufferPosition);

        buffer = default;
        bufferPosition = 0;
        overflowed = true;

        return moved;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppend(scoped ReadOnlySpan<char> str)
    {
        if (overflowed)
        {
            return false;
        }

        if (str.Length > Remaining)
        {
            overflowed = true;
            return false;
        }

        str.CopyTo(buffer[bufferPosition..]);
        bufferPosition += str.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendLine(scoped ReadOnlySpan<char> str)
    {
        if (overflowed)
        {
            return false;
        }

        var newLine = Environment.NewLine.AsSpan();
        if (str.Length + newLine.Length > Remaining)
        {
            overflowed = true;
            return false;
        }

        str.CopyTo(buffer[bufferPosition..]);
        bufferPosition += str.Length;
        newLine.CopyTo(buffer[bufferPosition..]);
        bufferPosition += newLine.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppend(Rune value)
    {
        if (overflowed)
        {
            return false;
        }

        if (!value.TryEncodeToUtf16(buffer[bufferPosition..], out var written))
        {
            overflowed = true;
            return false;
        }

        bufferPosition += written;
        return true;
    }

    private bool TryAppendFormatted<T>(T value, scoped ReadOnlySpan<char> format)
    {
        // The cast to ISpanFormattable reads like a box, but because the interface method is invoked directly on the
        // cast expression the JIT emits a constrained call and elides the allocation for value types. Hoisting it into
        // an ISpanFormattable local, or routing it through a T : ISpanFormattable helper, would box for real and break
        // this type's zero-allocation guarantee. Same shape the BCL uses in DefaultInterpolatedStringHandler.
        if (value is ISpanFormattable)
        {
            if (overflowed)
            {
                return false;
            }

            if (!((ISpanFormattable)value).TryFormat(buffer[bufferPosition..], out var written, format, null))
            {
                overflowed = true;
                return false;
            }

            bufferPosition += written;
            return true;
        }

        var text = value?.ToString();
        return TryAppend(text.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppend<T>(T value, scoped ReadOnlySpan<char> format, IFormatProvider? formatProvider)
        where T : ISpanFormattable
    {
        if (overflowed)
        {
            return false;
        }

        if (!value.TryFormat(buffer[bufferPosition..], out var written, format, formatProvider))
        {
            overflowed = true;
            return false;
        }

        bufferPosition += written;
        return true;
    }
}
