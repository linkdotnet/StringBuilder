using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates multiple objects together.
    /// </summary>
    /// <param name="values">Values, which will be concatenated together.</param>
    /// <typeparam name="T">Any given type, which can be translated to <see cref="string"/>.</typeparam>
    /// <returns>Concatenated string or an empty string if <see cref="values"/> is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T>(params T[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendJoin(string.Empty, values);
        return sb.ToString();
    }

    /// <summary>
    /// Concatenates two different types together.
    /// </summary>
    /// <typeparam name="T1">Typeparameter of <paramref name="arg1"/>.</typeparam>
    /// <typeparam name="T2">Typeparameter of <paramref name="arg2"/>.</typeparam>
    /// <param name="arg1">First argument.</param>
    /// <param name="arg2">Second argument.</param>
    /// <returns>String representation of the concateneted result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1, T2>(T1 arg1, T2 arg2)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);
        sb.AppendInternal(arg2);

        return sb.ToString();
    }

    /// <summary>
    /// Concatenates two different types together.
    /// </summary>
    /// <typeparam name="T1">Typeparameter of <paramref name="arg1"/>.</typeparam>
    /// <typeparam name="T2">Typeparameter of <paramref name="arg2"/>.</typeparam>
    /// <typeparam name="T3">Typeparameter of <paramref name="arg3"/>.</typeparam>
    /// <param name="arg1">First argument.</param>
    /// <param name="arg2">Second argument.</param>
    /// <param name="arg3">Third argument.</param>
    /// <returns>String representation of the concateneted result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);
        sb.AppendInternal(arg2);
        sb.AppendInternal(arg3);

        return sb.ToString();
    }

    /// <summary>
    /// Concatenates two different types together.
    /// </summary>
    /// <typeparam name="T1">Typeparameter of <paramref name="arg1"/>.</typeparam>
    /// <typeparam name="T2">Typeparameter of <paramref name="arg2"/>.</typeparam>
    /// <typeparam name="T3">Typeparameter of <paramref name="arg3"/>.</typeparam>
    /// <typeparam name="T4">Typeparameter of <paramref name="arg4"/>.</typeparam>
    /// <param name="arg1">First argument.</param>
    /// <param name="arg2">Second argument.</param>
    /// <param name="arg3">Third argument.</param>
    /// <param name="arg4">Fourth argument.</param>
    /// <returns>String representation of the concateneted result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);
        sb.AppendInternal(arg2);
        sb.AppendInternal(arg3);
        sb.AppendInternal(arg4);

        return sb.ToString();
    }

    /// <summary>
    /// Concatenates two different types together.
    /// </summary>
    /// <typeparam name="T1">Typeparameter of <paramref name="arg1"/>.</typeparam>
    /// <typeparam name="T2">Typeparameter of <paramref name="arg2"/>.</typeparam>
    /// <typeparam name="T3">Typeparameter of <paramref name="arg3"/>.</typeparam>
    /// <typeparam name="T4">Typeparameter of <paramref name="arg4"/>.</typeparam>
    /// <typeparam name="T5">Typeparameter of <paramref name="arg5"/>.</typeparam>
    /// <param name="arg1">First argument.</param>
    /// <param name="arg2">Second argument.</param>
    /// <param name="arg3">Third argument.</param>
    /// <param name="arg4">Fourth argument.</param>
    /// <param name="arg5">Fifth argument.</param>
    /// <returns>String representation of the concateneted result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);
        sb.AppendInternal(arg2);
        sb.AppendInternal(arg3);
        sb.AppendInternal(arg4);
        sb.AppendInternal(arg5);

        return sb.ToString();
    }
}