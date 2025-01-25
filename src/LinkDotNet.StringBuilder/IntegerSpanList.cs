using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// Represents a List based on the <see cref="Span{T}"/> type.
/// </summary>
[StructLayout(LayoutKind.Auto)]
[SkipLocalsInit]
internal ref struct IntegerSpanList
{
    private Span<int> buffer;
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerSpanList"/> struct.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IntegerSpanList()
    {
        buffer = GC.AllocateUninitializedArray<int>(8);
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<int> AsSpan() => buffer[..count];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int value)
    {
        if (count >= buffer.Length)
        {
            EnsureCapacity();
        }

        buffer[count] = value;
        count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int capacity = 0)
    {
        var currentSize = buffer.Length;
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = GC.AllocateUninitializedArray<int>(newSize);
        buffer[..count].CopyTo(rented);
        buffer = rented;
    }
}