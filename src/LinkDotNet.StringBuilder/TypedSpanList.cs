using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// Represents a List based on the <see cref="Span{T}"/> type.
/// </summary>
/// <typeparam name="T">Any struct.</typeparam>
[StructLayout(LayoutKind.Auto)]
internal ref struct TypedSpanList<T>
    where T : struct
{
    private Span<T> buffer;
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSpanList{T}"/> struct.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TypedSpanList()
    {
        buffer = GC.AllocateUninitializedArray<T>(8);
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan() => buffer[..count];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        if (count >= buffer.Length)
        {
            Grow();
        }

        buffer[count] = value;
        count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow(int capacity = 0)
    {
        var currentSize = buffer.Length;
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = GC.AllocateUninitializedArray<T>(newSize);
        buffer.CopyTo(rented);
        buffer = rented;
    }
}