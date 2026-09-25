using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    /// <remarks>
    /// Enumerating a <see cref="List{T}"/> or an array through <see cref="IEnumerable{T}"/> boxes the enumerator.
    /// </remarks>
    private static bool TryGetSpan<T>(IEnumerable<T> values, out ReadOnlySpan<T> span)
    {
        switch (values)
        {
            case T[] array:
                span = array;
                return true;
            case List<T> list:
                span = CollectionsMarshal.AsSpan(list);
                return true;
            default:
                span = default;
                return false;
        }
    }

    private void AppendJoinInternalString<T>(scoped ReadOnlySpan<char> separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (TryGetSpan(values, out var span))
        {
            AppendJoinInternalString(separator, span);
            return;
        }

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

    private void AppendJoinInternalChar<T>(char separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (TryGetSpan(values, out var span))
        {
            AppendJoinInternalChar(separator, span);
            return;
        }

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

        if (value is string s)
        {
            Append(s.AsSpan());
            return;
        }

        Append(value?.ToString());
    }

    /// <summary>
    /// Appends <paramref name="value"/> without boxing it: <see cref="bool"/> and <see cref="char"/> directly, any other
    /// <see cref="ISpanFormattable"/> through <see cref="TryAppendSpanFormattable{T}"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="bool"/> is not <see cref="ISpanFormattable"/>. The small IL keeps this inlinable at the end of deep
    /// call chains (e.g. <c>AppendJoin</c>), which a per-type <c>typeof</c> table was too large for.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownSpanFormattable<T>(T value) => TryAppendKnownSpanFormattable(value, default);

    /// <summary>
    /// Same as <see cref="TryAppendKnownSpanFormattable{T}(T)"/> but forwards a format string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAppendKnownSpanFormattable<T>(T value, scoped ReadOnlySpan<char> format)
    {
        if (typeof(T) == typeof(bool))
        {
            Append(Unsafe.As<T, bool>(ref value));
            return true;
        }

        if (typeof(T) == typeof(char))
        {
            Append(Unsafe.As<T, char>(ref value));
            return true;
        }

        return TryAppendSpanFormattable(value, format);
    }
}
