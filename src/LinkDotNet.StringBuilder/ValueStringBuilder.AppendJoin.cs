using System.Runtime.CompilerServices;
using System.Text;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Enumerable of strings to be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(ReadOnlySpan<char> separator, IEnumerable<string?> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Enumerable of strings to be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(ReadOnlySpan<char> separator, scoped ReadOnlySpan<string?> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Enumerable of strings to be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(char separator, scoped ReadOnlySpan<string?> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Enumerable of strings to be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(char separator, IEnumerable<string?> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Rune used as separator between the entries.</param>
    /// <param name="values">Enumerable of strings to be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(Rune separator, IEnumerable<string?> values)
        => AppendJoinInternalRune(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Enumerable to be concatenated.</param>
    /// <typeparam name="T">Type of the given enumerable.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(scoped ReadOnlySpan<char> separator, IEnumerable<T> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Enumerable to be concatenated.</param>
    /// <typeparam name="T">Type of the given enumerable.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(scoped ReadOnlySpan<char> separator, ReadOnlySpan<T> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Enumerable to be concatenated.</param>
    /// <typeparam name="T">Type of the given enumerable.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(char separator, IEnumerable<T> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Enumerable to be concatenated.</param>
    /// <typeparam name="T">Type of the given enumerable.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(char separator, scoped ReadOnlySpan<T> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Rune used as separator between the entries.</param>
    /// <param name="values">Enumerable to be concatenated.</param>
    /// <typeparam name="T">Type of the given enumerable.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(Rune separator, IEnumerable<T> values)
        => AppendJoinInternalRune(separator, values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalString<T>(scoped ReadOnlySpan<char> separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return;
        }

        var current = enumerator.Current;
        AppendInternal(current);

        while (enumerator.MoveNext())
        {
            Append(separator);
            current = enumerator.Current;
            AppendInternal(current);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalString<T>(scoped ReadOnlySpan<char> separator, scoped ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
        {
            return;
        }

        AppendInternal(values[0]);

        for (var i = 1; i < values.Length; i++)
        {
            Append(separator);
            AppendInternal(values[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalChar<T>(char separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return;
        }

        var current = enumerator.Current;
        AppendInternal(current);

        while (enumerator.MoveNext())
        {
            AppendInternal(separator);
            current = enumerator.Current;
            AppendInternal(current);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalChar<T>(char separator, scoped ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
        {
            return;
        }

        AppendInternal(values[0]);

        for (var i = 1; i < values.Length; i++)
        {
            Append(separator);
            AppendInternal(values[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalRune<T>(Rune separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return;
        }

        var current = enumerator.Current;
        AppendInternal(current);

        while (enumerator.MoveNext())
        {
            Append(separator);
            current = enumerator.Current;
            AppendInternal(current);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendInternal<T>(T value)
    {
        if (TryAppendKnownSpanFormattable(value))
        {
            return;
        }

        switch (value)
        {
            case ISpanFormattable spanFormattable:
                AppendSpanFormattable(spanFormattable);
                break;
            case string s:
                Append(s.AsSpan());
                break;
            default:
                Append(value?.ToString());
                break;
        }
    }

    /// <summary>
    /// Appends <paramref name="value"/> directly for a fixed set of well known value types without boxing it.
    /// </summary>
    /// <remarks>
    /// Pattern matching an unconstrained generic value against an interface (like <see cref="ISpanFormattable"/>)
    /// requires the value to be boxed. For the handful of value types that are used the vast majority of the time,
    /// we instead reinterpret the bits of <paramref name="value"/> via <see cref="Unsafe.As{TFrom,TTo}(ref TFrom)"/>
    /// and dispatch to the already boxing-free, constrained <see cref="Append{T}"/> overload.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownSpanFormattable<T>(T value) =>
        TryAppendKnownIntegralType(value) || TryAppendKnownOtherType(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownIntegralType<T>(T value)
    {
        if (typeof(T) == typeof(bool))
        {
            Append(Unsafe.As<T, bool>(ref value));
        }
        else if (typeof(T) == typeof(char))
        {
            Append(Unsafe.As<T, char>(ref value));
        }
        else if (typeof(T) == typeof(byte))
        {
            Append(Unsafe.As<T, byte>(ref value));
        }
        else if (typeof(T) == typeof(sbyte))
        {
            Append(Unsafe.As<T, sbyte>(ref value));
        }
        else if (typeof(T) == typeof(short))
        {
            Append(Unsafe.As<T, short>(ref value));
        }
        else if (typeof(T) == typeof(ushort))
        {
            Append(Unsafe.As<T, ushort>(ref value));
        }
        else if (typeof(T) == typeof(int))
        {
            Append(Unsafe.As<T, int>(ref value));
        }
        else if (typeof(T) == typeof(uint))
        {
            Append(Unsafe.As<T, uint>(ref value));
        }
        else if (typeof(T) == typeof(long))
        {
            Append(Unsafe.As<T, long>(ref value));
        }
        else if (typeof(T) == typeof(ulong))
        {
            Append(Unsafe.As<T, ulong>(ref value));
        }
        else if (typeof(T) == typeof(Int128))
        {
            Append(Unsafe.As<T, Int128>(ref value));
        }
        else if (typeof(T) == typeof(UInt128))
        {
            Append(Unsafe.As<T, UInt128>(ref value));
        }
        else
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownOtherType<T>(T value)
    {
        if (typeof(T) == typeof(float))
        {
            Append(Unsafe.As<T, float>(ref value));
        }
        else if (typeof(T) == typeof(double))
        {
            Append(Unsafe.As<T, double>(ref value));
        }
        else if (typeof(T) == typeof(decimal))
        {
            Append(Unsafe.As<T, decimal>(ref value));
        }
        else if (typeof(T) == typeof(DateTime))
        {
            Append(Unsafe.As<T, DateTime>(ref value));
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            Append(Unsafe.As<T, DateTimeOffset>(ref value));
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            Append(Unsafe.As<T, TimeSpan>(ref value));
        }
        else if (typeof(T) == typeof(Guid))
        {
            Append(Unsafe.As<T, Guid>(ref value));
        }
        else if (typeof(T) == typeof(Half))
        {
            Append(Unsafe.As<T, Half>(ref value));
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Same as <see cref="TryAppendKnownSpanFormattable{T}(T)"/> but forwards a format string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownSpanFormattable<T>(T value, scoped ReadOnlySpan<char> format) =>
        TryAppendKnownIntegralType(value, format) || TryAppendKnownOtherType(value, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownIntegralType<T>(T value, scoped ReadOnlySpan<char> format)
    {
        if (typeof(T) == typeof(bool))
        {
            Append(Unsafe.As<T, bool>(ref value));
        }
        else if (typeof(T) == typeof(char))
        {
            Append(Unsafe.As<T, char>(ref value));
        }
        else if (typeof(T) == typeof(byte))
        {
            Append(Unsafe.As<T, byte>(ref value), format);
        }
        else if (typeof(T) == typeof(sbyte))
        {
            Append(Unsafe.As<T, sbyte>(ref value), format);
        }
        else if (typeof(T) == typeof(short))
        {
            Append(Unsafe.As<T, short>(ref value), format);
        }
        else if (typeof(T) == typeof(ushort))
        {
            Append(Unsafe.As<T, ushort>(ref value), format);
        }
        else if (typeof(T) == typeof(int))
        {
            Append(Unsafe.As<T, int>(ref value), format);
        }
        else if (typeof(T) == typeof(uint))
        {
            Append(Unsafe.As<T, uint>(ref value), format);
        }
        else if (typeof(T) == typeof(long))
        {
            Append(Unsafe.As<T, long>(ref value), format);
        }
        else if (typeof(T) == typeof(ulong))
        {
            Append(Unsafe.As<T, ulong>(ref value), format);
        }
        else if (typeof(T) == typeof(Int128))
        {
            Append(Unsafe.As<T, Int128>(ref value), format);
        }
        else if (typeof(T) == typeof(UInt128))
        {
            Append(Unsafe.As<T, UInt128>(ref value), format);
        }
        else
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownOtherType<T>(T value, scoped ReadOnlySpan<char> format)
    {
        if (typeof(T) == typeof(float))
        {
            Append(Unsafe.As<T, float>(ref value), format);
        }
        else if (typeof(T) == typeof(double))
        {
            Append(Unsafe.As<T, double>(ref value), format);
        }
        else if (typeof(T) == typeof(decimal))
        {
            Append(Unsafe.As<T, decimal>(ref value), format);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            Append(Unsafe.As<T, DateTime>(ref value), format);
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            Append(Unsafe.As<T, DateTimeOffset>(ref value), format);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            Append(Unsafe.As<T, TimeSpan>(ref value), format);
        }
        else if (typeof(T) == typeof(Guid))
        {
            Append(Unsafe.As<T, Guid>(ref value), format);
        }
        else if (typeof(T) == typeof(Half))
        {
            Append(Unsafe.As<T, Half>(ref value), format);
        }
        else
        {
            return false;
        }

        return true;
    }
}