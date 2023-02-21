using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(ReadOnlySpan<char> separator, IEnumerable<string?> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin(char separator, IEnumerable<string?> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    /// <typeparam name="T">Type of the given array.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(ReadOnlySpan<char> separator, IEnumerable<T> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    /// <typeparam name="T">Type of the given array.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendJoin<T>(char separator, IEnumerable<T> values)
        => AppendJoinInternalChar(separator, values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendJoinInternalString<T>(ReadOnlySpan<char> separator, IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return;
        }

        var current = enumerator.Current;
        if (current != null)
        {
            AppendInternal(current);
        }

        while (enumerator.MoveNext())
        {
            Append(separator);
            current = enumerator.Current;
            AppendInternal(current);
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