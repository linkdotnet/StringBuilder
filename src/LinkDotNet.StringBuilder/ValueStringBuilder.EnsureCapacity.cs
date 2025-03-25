using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Ensures the builder's buffer size is at least <paramref name="newCapacity"/>, renting a larger buffer if not.
    /// </summary>
    /// <param name="newCapacity">New capacity for the builder.</param>
    /// <remarks>
    /// If <see cref="Length"/> is already &gt;= <paramref name="newCapacity"/>, nothing is done.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int newCapacity)
    {
        if (Capacity >= newCapacity)
        {
            return;
        }

        var newSize = FindSmallestPowerOf2Above(newCapacity);

        var rented = ArrayPool<char>.Shared.Rent(newSize);

        if (bufferPosition > 0)
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(buffer);
            ref var destinationRef = ref MemoryMarshal.GetReference(rented.AsSpan());

            Unsafe.CopyBlock(
                ref Unsafe.As<char, byte>(ref destinationRef),
                ref Unsafe.As<char, byte>(ref sourceRef),
                (uint)bufferPosition * sizeof(char));
        }

        if (arrayFromPool is not null)
        {
            ArrayPool<char>.Shared.Return(arrayFromPool);
        }

        buffer = rented;
        arrayFromPool = rented;
    }

    /// <summary>
    /// Finds the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    /// <param name="minimum">The value the result should be greater than or equal to.</param>
    /// <returns>The smallest power of 2 &gt;= <paramref name="minimum"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindSmallestPowerOf2Above(int minimum)
    {
        return 1 << (int)Math.Ceiling(Math.Log2(minimum));
    }
}
