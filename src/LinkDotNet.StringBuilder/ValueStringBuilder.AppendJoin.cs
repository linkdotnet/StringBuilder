namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    public void AppendJoin(ReadOnlySpan<char> separator, IEnumerable<string?> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    public void AppendJoin(char separator, IEnumerable<string?> values)
        => AppendJoinInternalChar(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">String used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    /// <typeparam name="T">Type of the given array.</typeparam>
    public void AppendJoin<T>(ReadOnlySpan<char> separator, IEnumerable<T> values)
        => AppendJoinInternalString(separator, values);

    /// <summary>
    /// Concatenates and appends all values with the given separator between each entry at the end of the string.
    /// </summary>
    /// <param name="separator">Character used as separator between the entries.</param>
    /// <param name="values">Array of strings, which will be concatenated.</param>
    /// <typeparam name="T">Type of the given array.</typeparam>
    public void AppendJoin<T>(char separator, IEnumerable<T> values)
        => AppendJoinInternalChar(separator, values);

    private void AppendJoinInternalString<T2>(ReadOnlySpan<char> separator, IEnumerable<T2> values)
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
            if (current != null)
            {
                AppendInternal(current);
            }
        }
    }

    private void AppendJoinInternalChar<T2>(char separator, IEnumerable<T2> values)
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
            AppendInternal(separator);
            current = enumerator.Current;
            if (current != null)
            {
                AppendInternal(current);
            }
        }
    }

    private void AppendInternal<T>(T value)
    {
        if (value is ISpanFormattable spanFormattable)
        {
            AppendSpanFormattable(spanFormattable);
        }
        else
        {
            Append(value?.ToString());
        }
    }
}