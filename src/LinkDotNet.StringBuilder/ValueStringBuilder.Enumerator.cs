using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Creates an enumerator over the characters in the builder.
    /// </summary>
    /// <returns>An enumerator over the characters in the builder.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Enumerator GetEnumerator() => new(buffer[..bufferPosition]);

    /// <summary>Enumerates the elements of a <see cref="Span{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<char> span;
        private int index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ReadOnlySpan<char> span)
        {
            this.span = span;
            index = -1;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        /// <value>The element at the current position of the enumerator. </value>
        public readonly char Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => span[index];
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        /// <returns><see langword="true"/> if the enumerator successfully advanced to the next element; <see langword="false"/> if the enumerator reached the end of the span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++index < span.Length;
    }
}