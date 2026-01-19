using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Appends an interpolated string to the builder.
    /// </summary>
    /// <param name="handler">The interpolated string handler.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler)
    {
        this = handler.Builder;
    }

    /// <summary>
    /// Appends an interpolated string followed by a new line to the builder.
    /// </summary>
    /// <param name="handler">The interpolated string handler.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler)
    {
        this = handler.Builder;
        Append(Environment.NewLine);
    }

    /// <summary>
    /// Nested struct which handles interpolated strings for <see cref="ValueStringBuilder"/>.
    /// </summary>
    [InterpolatedStringHandler]
    public ref struct AppendInterpolatedStringHandler
    {
        internal ValueStringBuilder Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppendInterpolatedStringHandler"/> struct.
        /// </summary>
        /// <param name="literalLength">The length of the literal part of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted segments in the interpolated string.</param>
        /// <param name="builder">The builder to append to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AppendInterpolatedStringHandler(int literalLength, int formattedCount, ValueStringBuilder builder)
        {
            this.Builder = builder;

            // A conservative guess for the capacity.
            this.Builder.EnsureCapacity(this.Builder.Length + literalLength + (formattedCount * 11));
        }

        /// <summary>
        /// Appends a literal string to the handler.
        /// </summary>
        /// <param name="value">The literal string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(string value) => Builder.Append(value);

        /// <summary>
        /// Appends a formatted value to the handler.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value)
        {
            if (value is ISpanFormattable formattable)
            {
                Builder.Append(formattable);
            }
            else
            {
                Builder.Append(value?.ToString());
            }
        }

        /// <summary>
        /// Appends a formatted value with a given format.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, string? format)
        {
            if (value is ISpanFormattable formattable)
            {
                Builder.Append(formattable, format);
            }
            else
            {
                Builder.Append(value?.ToString());
            }
        }

        /// <summary>
        /// Appends a character span to the handler.
        /// </summary>
        /// <param name="value">The character span.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(ReadOnlySpan<char> value) => Builder.Append(value);

        /// <summary>
        /// Appends a string to the handler.
        /// </summary>
        /// <param name="value">The string value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string? value) => Builder.Append(value);
    }
}
