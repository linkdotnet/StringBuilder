using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct FixedSizeValueStringBuilder
{
    /// <summary>
    /// Appends <paramref name="value"/> directly for a fixed set of well known value types without boxing it.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <param name="format">Optional formatter.</param>
    /// <param name="appended">Whether the value fit into the remaining buffer. Only meaningful if this method
    /// returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> if <typeparamref name="T"/> is one of the well known types and was handled.</returns>
    /// <remarks>
    /// Casting an unconstrained generic value to an interface (like <see cref="ISpanFormattable"/>) boxes it. For the
    /// handful of value types that are used the vast majority of the time, we instead reinterpret the bits of
    /// <paramref name="value"/> via <see cref="Unsafe.As{TFrom,TTo}(ref TFrom)"/> and dispatch to the constrained,
    /// boxing-free <see cref="TryAppend{T}(T, ReadOnlySpan{char}, IFormatProvider)"/> overload.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownSpanFormattable<T>(T value, scoped ReadOnlySpan<char> format, out bool appended) =>
        TryAppendKnownIntegralType(value, format, out appended) || TryAppendKnownOtherType(value, format, out appended);

    private bool TryAppendKnownIntegralType<T>(T value, scoped ReadOnlySpan<char> format, out bool appended)
    {
        if (typeof(T) == typeof(bool))
        {
            appended = TryAppend(Unsafe.As<T, bool>(ref value) ? bool.TrueString : bool.FalseString);
        }
        else if (typeof(T) == typeof(char))
        {
            appended = TryAppend(Unsafe.As<T, char>(ref value));
        }
        else if (typeof(T) == typeof(byte))
        {
            appended = TryAppend(Unsafe.As<T, byte>(ref value), format, null);
        }
        else if (typeof(T) == typeof(sbyte))
        {
            appended = TryAppend(Unsafe.As<T, sbyte>(ref value), format, null);
        }
        else if (typeof(T) == typeof(short))
        {
            appended = TryAppend(Unsafe.As<T, short>(ref value), format, null);
        }
        else if (typeof(T) == typeof(ushort))
        {
            appended = TryAppend(Unsafe.As<T, ushort>(ref value), format, null);
        }
        else if (typeof(T) == typeof(int))
        {
            appended = TryAppend(Unsafe.As<T, int>(ref value), format, null);
        }
        else if (typeof(T) == typeof(uint))
        {
            appended = TryAppend(Unsafe.As<T, uint>(ref value), format, null);
        }
        else if (typeof(T) == typeof(long))
        {
            appended = TryAppend(Unsafe.As<T, long>(ref value), format, null);
        }
        else if (typeof(T) == typeof(ulong))
        {
            appended = TryAppend(Unsafe.As<T, ulong>(ref value), format, null);
        }
        else if (typeof(T) == typeof(Int128))
        {
            appended = TryAppend(Unsafe.As<T, Int128>(ref value), format, null);
        }
        else if (typeof(T) == typeof(UInt128))
        {
            appended = TryAppend(Unsafe.As<T, UInt128>(ref value), format, null);
        }
        else
        {
            appended = false;
            return false;
        }

        return true;
    }

    private bool TryAppendKnownOtherType<T>(T value, scoped ReadOnlySpan<char> format, out bool appended)
    {
        if (typeof(T) == typeof(float))
        {
            appended = TryAppend(Unsafe.As<T, float>(ref value), format, null);
        }
        else if (typeof(T) == typeof(double))
        {
            appended = TryAppend(Unsafe.As<T, double>(ref value), format, null);
        }
        else if (typeof(T) == typeof(decimal))
        {
            appended = TryAppend(Unsafe.As<T, decimal>(ref value), format, null);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            appended = TryAppend(Unsafe.As<T, DateTime>(ref value), format, null);
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            appended = TryAppend(Unsafe.As<T, DateTimeOffset>(ref value), format, null);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            appended = TryAppend(Unsafe.As<T, TimeSpan>(ref value), format, null);
        }
        else if (typeof(T) == typeof(Guid))
        {
            appended = TryAppend(Unsafe.As<T, Guid>(ref value), format, null);
        }
        else if (typeof(T) == typeof(Half))
        {
            appended = TryAppend(Unsafe.As<T, Half>(ref value), format, null);
        }
        else
        {
            appended = false;
            return false;
        }

        return true;
    }
}
