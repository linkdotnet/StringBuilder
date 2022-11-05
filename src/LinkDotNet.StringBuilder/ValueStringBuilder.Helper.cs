namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates multiple objects together.
    /// </summary>
    /// <param name="values">Values, which will be concatenated together.</param>
    /// <typeparam name="T">Any given type, which can be translated to string.</typeparam>
    /// <returns>Concatenated string or an empty string if <see cref="values"/> is empty.</returns>
    public static string Concat<T>(params T[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        using var sb = new ValueStringBuilder();
        sb.AppendJoin(string.Empty, values);
        return sb.ToString();
    }
}