using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Concatenates the string representation of multiple objects together.
    /// </summary>
    /// <param name="values">Values to be concatenated.</param>
    /// <typeparam name="T">Any type for <paramref name="values"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <returns>The concatenated string, or an empty string if <paramref name="values"/> is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T>(params scoped ReadOnlySpan<T> values)
    {
        if (values.IsEmpty)
        {
            return string.Empty;
        }

        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendJoin(string.Empty, values);

        return sb.ToString();
    }

    /// <summary>
    /// Creates the string representation of an object.
    /// </summary>
    /// <typeparam name="T1">Any type for <paramref name="arg1"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <param name="arg1">First value to be concatenated.</param>
    /// <returns>The string representation of the object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1>(T1 arg1)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);

        return sb.ToString();
    }

    /// <summary>
    /// Concatenates the string representation of two objects together.
    /// </summary>
    /// <typeparam name="T1">Any type for <paramref name="arg1"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T2">Any type for <paramref name="arg2"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <param name="arg1">First value to be concatenated.</param>
    /// <param name="arg2">Second value to be concatenated.</param>
    /// <returns>The concatenated string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T1, T2>(T1 arg1, T2 arg2)
    {
        using var sb = new ValueStringBuilder(stackalloc char[128]);
        sb.AppendInternal(arg1);
        sb.AppendInternal(arg2);

        return sb.ToString();
    }

    /// <summary>
    /// Concatenates the string representation of three objects together.
    /// </summary>
    /// <typeparam name="T1">Any type for <paramref name="arg1"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T2">Any type for <paramref name="arg2"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T3">Any type for <paramref name="arg3"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <param name="arg1">First value to be concatenated.</param>
    /// <param name="arg2">Second value to be concatenated.</param>
    /// <param name="arg3">Third value to be concatenated.</param>
    /// <returns>The concatenated string.</returns>
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
    /// Concatenates the string representation of four objects together.
    /// </summary>
    /// <typeparam name="T1">Any type for <paramref name="arg1"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T2">Any type for <paramref name="arg2"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T3">Any type for <paramref name="arg3"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T4">Any type for <paramref name="arg4"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <param name="arg1">First value to be concatenated.</param>
    /// <param name="arg2">Second value to be concatenated.</param>
    /// <param name="arg3">Third value to be concatenated.</param>
    /// <param name="arg4">Fourth value to be concatenated.</param>
    /// <returns>The concatenated string.</returns>
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
    /// Concatenates the string representation of five objects together.
    /// </summary>
    /// <typeparam name="T1">Any type for <paramref name="arg1"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T2">Any type for <paramref name="arg2"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T3">Any type for <paramref name="arg3"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T4">Any type for <paramref name="arg4"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <typeparam name="T5">Any type for <paramref name="arg5"/> that can be converted to <see cref="string"/>.</typeparam>
    /// <param name="arg1">First value to be concatenated.</param>
    /// <param name="arg2">Second value to be concatenated.</param>
    /// <param name="arg3">Third value to be concatenated.</param>
    /// <param name="arg4">Fourth value to be concatenated.</param>
    /// <param name="arg5">Fifth value to be concatenated.</param>
    /// <returns>The concatenated string.</returns>
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