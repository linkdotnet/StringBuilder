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
}