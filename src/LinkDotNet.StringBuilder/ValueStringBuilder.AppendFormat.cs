using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Appends the format string to the given <see cref="ValueStringBuilder"/> instance.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="arg">Argument for <c>{0}</c>.</param>
    /// <typeparam name="T">Any type.</typeparam>
    /// <remarks>
    /// The current version does not allow for a custom format.
    /// So: <code>AppendFormat("{0:00}")</code> is not allowed and will result in an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AppendFormat<T>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] ReadOnlySpan<char> format,
        T arg)
    {
        var formatIndex = 0;
        var start = 0;
        while (formatIndex < format.Length)
        {
            var c = format[formatIndex];
            if (c == '{')
            {
                var endIndex = format[(formatIndex + 1)..].IndexOf('}');
                if (endIndex == -1)
                {
                    Append(format);
                    return;
                }

                if (start != formatIndex)
                {
                    Append(format[start..formatIndex]);
                }

                var placeholder = format.Slice(formatIndex, endIndex + 2);

                GetValidArgumentIndex(placeholder, 0);

                AppendInternal(arg);
                formatIndex += endIndex + 2;
                start = formatIndex;
            }
            else
            {
                formatIndex++;
            }
        }

        if (start != formatIndex)
        {
            Append(format[start..formatIndex]);
        }
    }

    /// <summary>
    /// Appends the format string to the given <see cref="ValueStringBuilder"/> instance.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="arg1">Argument for <c>{0}</c>.</param>
    /// <param name="arg2">Argument for <c>{1}</c>.</param>
    /// <typeparam name="T1">Any type for <param name="arg1"></param>.</typeparam>
    /// <typeparam name="T2">Any type for <param name="arg2"></param>.</typeparam>
    /// <remarks>
    /// The current version does not allow for a custom format.
    /// So: <code>AppendFormat("{0:00}")</code> is not allowed and will result in an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AppendFormat<T1, T2>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] ReadOnlySpan<char> format,
        T1 arg1,
        T2 arg2)
    {
        var formatIndex = 0;
        var start = 0;
        while (formatIndex < format.Length)
        {
            var c = format[formatIndex];
            if (c == '{')
            {
                var endIndex = format[(formatIndex + 1)..].IndexOf('}');
                if (endIndex == -1)
                {
                    Append(format);
                    return;
                }

                if (start != formatIndex)
                {
                    Append(format[start..formatIndex]);
                }

                var placeholder = format.Slice(formatIndex, endIndex + 2);

                var index = GetValidArgumentIndex(placeholder, 1);

                switch (index)
                {
                    case 0:
                        AppendInternal(arg1);
                        break;
                    case 1:
                        AppendInternal(arg2);
                        break;
                }

                formatIndex += endIndex + 2;
                start = formatIndex;
            }
            else
            {
                formatIndex++;
            }
        }

        if (start != formatIndex)
        {
            Append(format[start..formatIndex]);
        }
    }

    /// <summary>
    /// Appends the format string to the given <see cref="ValueStringBuilder"/> instance.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="arg1">Argument for <c>{0}</c>.</param>
    /// <param name="arg2">Argument for <c>{1}</c>.</param>
    /// <param name="arg3">Argument for <c>{2}</c>.</param>
    /// <typeparam name="T1">Any type for <param name="arg1"></param>.</typeparam>
    /// <typeparam name="T2">Any type for <param name="arg2"></param>.</typeparam>
    /// <typeparam name="T3">Any type for <param name="arg3"></param>.</typeparam>
    /// <remarks>
    /// The current version does not allow for a custom format.
    /// So: <code>AppendFormat("{0:00}")</code> is not allowed and will result in an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AppendFormat<T1, T2, T3>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] ReadOnlySpan<char> format,
        T1 arg1,
        T2 arg2,
        T3 arg3)
    {
        var formatIndex = 0;
        var start = 0;
        while (formatIndex < format.Length)
        {
            var c = format[formatIndex];
            if (c == '{')
            {
                var endIndex = format[(formatIndex + 1)..].IndexOf('}');
                if (endIndex == -1)
                {
                    Append(format);
                    return;
                }

                if (start != formatIndex)
                {
                    Append(format[start..formatIndex]);
                }

                var placeholder = format.Slice(formatIndex, endIndex + 2);

                var index = GetValidArgumentIndex(placeholder, 2);

                switch (index)
                {
                    case 0:
                        AppendInternal(arg1);
                        break;
                    case 1:
                        AppendInternal(arg2);
                        break;
                    case 2:
                        AppendInternal(arg3);
                        break;
                }

                formatIndex += endIndex + 2;
                start = formatIndex;
            }
            else
            {
                formatIndex++;
            }
        }

        if (start != formatIndex)
        {
            Append(format[start..formatIndex]);
        }
    }

    /// <summary>
    /// Appends the format string to the given <see cref="ValueStringBuilder"/> instance.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="arg1">Argument for <c>{0}</c>.</param>
    /// <param name="arg2">Argument for <c>{1}</c>.</param>
    /// <param name="arg3">Argument for <c>{2}</c>.</param>
    /// <param name="arg4">Argument for <c>{3}</c>.</param>
    /// <typeparam name="T1">Any type for <param name="arg1"></param>.</typeparam>
    /// <typeparam name="T2">Any type for <param name="arg2"></param>.</typeparam>
    /// <typeparam name="T3">Any type for <param name="arg3"></param>.</typeparam>
    /// <typeparam name="T4">Any type for <param name="arg4"></param>.</typeparam>
    /// <remarks>
    /// The current version does not allow for a custom format.
    /// So: <code>AppendFormat("{0:00}")</code> is not allowed and will result in an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AppendFormat<T1, T2, T3, T4>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] ReadOnlySpan<char> format,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4)
    {
        var formatIndex = 0;
        var start = 0;
        while (formatIndex < format.Length)
        {
            var c = format[formatIndex];
            if (c == '{')
            {
                var endIndex = format[(formatIndex + 1)..].IndexOf('}');
                if (endIndex == -1)
                {
                    Append(format);
                    return;
                }

                if (start != formatIndex)
                {
                    Append(format[start..formatIndex]);
                }

                var placeholder = format.Slice(formatIndex, endIndex + 2);

                var index = GetValidArgumentIndex(placeholder, 3);

                switch (index)
                {
                    case 0:
                        AppendInternal(arg1);
                        break;
                    case 1:
                        AppendInternal(arg2);
                        break;
                    case 2:
                        AppendInternal(arg3);
                        break;
                    case 3:
                        AppendInternal(arg4);
                        break;
                }

                formatIndex += endIndex + 2;
                start = formatIndex;
            }
            else
            {
                formatIndex++;
            }
        }

        if (start != formatIndex)
        {
            Append(format[start..formatIndex]);
        }
    }

    /// <summary>
    /// Appends the format string to the given <see cref="ValueStringBuilder"/> instance.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="arg1">Argument for <c>{0}</c>.</param>
    /// <param name="arg2">Argument for <c>{1}</c>.</param>
    /// <param name="arg3">Argument for <c>{2}</c>.</param>
    /// <param name="arg4">Argument for <c>{3}</c>.</param>
    /// <param name="arg5">Argument for <c>{4}</c>.</param>
    /// <typeparam name="T1">Any type for <param name="arg1"></param>.</typeparam>
    /// <typeparam name="T2">Any type for <param name="arg2"></param>.</typeparam>
    /// <typeparam name="T3">Any type for <param name="arg3"></param>.</typeparam>
    /// <typeparam name="T4">Any type for <param name="arg4"></param>.</typeparam>
    /// <typeparam name="T5">Any type for <param name="arg5"></param>.</typeparam>
    /// <remarks>
    /// The current version does not allow for a custom format.
    /// So: <code>AppendFormat("{0:00}")</code> is not allowed and will result in an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AppendFormat<T1, T2, T3, T4, T5>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] ReadOnlySpan<char> format,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5)
    {
        var formatIndex = 0;
        var start = 0;
        while (formatIndex < format.Length)
        {
            var c = format[formatIndex];
            if (c == '{')
            {
                var endIndex = format[(formatIndex + 1)..].IndexOf('}');
                if (endIndex == -1)
                {
                    Append(format);
                    return;
                }

                if (start != formatIndex)
                {
                    Append(format[start..formatIndex]);
                }

                var placeholder = format.Slice(formatIndex, endIndex + 2);

                var index = GetValidArgumentIndex(placeholder, 4);

                switch (index)
                {
                    case 0:
                        AppendInternal(arg1);
                        break;
                    case 1:
                        AppendInternal(arg2);
                        break;
                    case 2:
                        AppendInternal(arg3);
                        break;
                    case 3:
                        AppendInternal(arg4);
                        break;
                    case 4:
                        AppendInternal(arg5);
                        break;
                }

                formatIndex += endIndex + 2;
                start = formatIndex;
            }
            else
            {
                formatIndex++;
            }
        }

        if (start != formatIndex)
        {
            Append(format[start..formatIndex]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetValidArgumentIndex(ReadOnlySpan<char> placeholder, int allowedRange)
    {
#pragma warning disable MA0011
        if (!int.TryParse(placeholder[1..^1], out var argIndex))
#pragma warning restore MA0011
        {
            throw new FormatException("Invalid argument index in format string: " + placeholder.ToString());
        }

        if (argIndex < 0 || argIndex > allowedRange)
        {
            throw new FormatException("Invalid argument index in format string: " + placeholder.ToString());
        }

        return argIndex;
    }
}