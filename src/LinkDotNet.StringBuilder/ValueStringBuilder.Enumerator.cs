using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    public readonly Enumerator GetEnumerator() => new(buffer[..bufferPosition]);

    /// <summary>Enumerates the elements of a <see cref="Span{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public ref struct Enumerator
    {
        private readonly Span<char> span;
        private int index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<char> span)
        {
            this.span = span;
            index = -1;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public readonly ref char Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref span[index];
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var nextIndex = index + 1;
            if (nextIndex < span.Length)
            {
                index = nextIndex;
                return true;
            }

            return false;
        }
    }
}