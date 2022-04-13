using System.Buffers;
using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

/// <summary>
/// Represents a List based on the <see cref="Span{T}"/> type.
/// </summary>
/// <typeparam name="T">Any struct.</typeparam>
internal ref struct TypedSpanList<T>
    where T : struct
{
    private Span<T> buffer;
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedSpanList{T}"/> struct.
    /// </summary>
    public TypedSpanList()
    {
        buffer = new T[8];
        count = 0;
    }

    public ReadOnlySpan<T> AsSpan => buffer[..count];

    public void Add(T value)
    {
        if (count >= buffer.Length)
        {
            Grow();
        }

        buffer[count] = value;
        count++;
    }

    private void Grow(int capacity = 0)
    {
        var currentSize = buffer.Length;
        var newSize = capacity > 0 ? capacity : currentSize * 2;
        var rented = ArrayPool<T>.Shared.Rent(newSize);
        buffer.CopyTo(rented);
        buffer = rented;
        ArrayPool<T>.Shared.Return(rented);
    }
}