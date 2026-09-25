using System.Buffers;
using System.Numerics;
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
        if (Capacity < newCapacity)
        {
            Grow(newCapacity);
        }
    }

    /// <summary>
    /// Finds the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    /// <param name="minimum">The value the result should be greater than or equal to.</param>
    /// <returns>The smallest power of 2 &gt;= <paramref name="minimum"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindSmallestPowerOf2Above(int minimum)
    {
        return (int)BitOperations.RoundUpToPowerOf2((uint)minimum);
    }

    /// <remarks>
    /// The rent and copy live in a static method: a non-inlined call receiving <c>this</c> by reference would
    /// force the JIT to keep the builder's fields in memory instead of registers in every caller.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static char[] RentAndCopy(scoped ReadOnlySpan<char> content, char[]? toReturn, int newCapacity)
    {
        var rented = ArrayPool<char>.Shared.Rent(FindSmallestPowerOf2Above(newCapacity));

        if (!content.IsEmpty)
        {
            Unsafe.CopyBlock(
                ref Unsafe.As<char, byte>(ref MemoryMarshal.GetArrayDataReference(rented)),
                ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(content)),
                (uint)content.Length * sizeof(char));
        }

        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }

        return rented;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow(int newCapacity)
    {
        var rented = RentAndCopy(buffer[..bufferPosition], arrayFromPool, newCapacity);
        buffer = rented;
        arrayFromPool = rented;
    }
}
