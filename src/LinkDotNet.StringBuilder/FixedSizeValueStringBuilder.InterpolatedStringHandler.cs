using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LinkDotNet.StringBuilder;

public ref partial struct FixedSizeValueStringBuilder
{
    /// <summary>
    /// Appends an interpolated string to the builder.
    /// </summary>
    /// <param name="handler">The interpolated string handler.</param>
    /// <remarks>
    /// Atomicity applies per literal and per hole, not to the interpolated string as a whole. The first part which
    /// does not fit sets <see cref="Overflowed"/> and the remaining parts are skipped, so the content stays a valid
    /// prefix of the interpolated string.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler)
    {
        this = handler.Builder;
    }

    /// <summary>
    /// Appends an interpolated string followed by <see cref="Environment.NewLine"/> to the builder.
    /// </summary>
    /// <param name="handler">The interpolated string handler.</param>
    /// <remarks>
    /// Atomicity applies per literal and per hole, not to the interpolated string as a whole. The first part which
    /// does not fit sets <see cref="Overflowed"/> and the remaining parts - including the new line - are skipped.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler)
    {
        this = handler.Builder;
        AppendLine();
    }

    /// <summary>
    /// Nested struct which handles interpolated strings for <see cref="FixedSizeValueStringBuilder"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public ref struct AppendInterpolatedStringHandler
    {
        internal FixedSizeValueStringBuilder Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppendInterpolatedStringHandler"/> struct.
        /// </summary>
        /// <param name="literalLength">The length of the literal part of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted segments in the interpolated string.</param>
        /// <param name="builder">The builder to append to.</param>
        /// <param name="shouldAppend">Set to <see langword="false"/> when the literals alone already do not fit, in
        /// which case the compiler skips the whole interpolation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AppendInterpolatedStringHandler(
            int literalLength,
            int formattedCount,
            FixedSizeValueStringBuilder builder,
            out bool shouldAppend)
        {
            _ = formattedCount;
            Builder = builder;

            if (Builder.overflowed)
            {
                shouldAppend = false;
                return;
            }

            if (literalLength > Builder.Remaining)
            {
                Builder.overflowed = true;
                shouldAppend = false;
                return;
            }

            shouldAppend = true;
        }

        /// <summary>
        /// Appends a literal string to the handler.
        /// </summary>
        /// <param name="value">The literal string.</param>
        /// <returns><see langword="true"/> if it fit; otherwise, <see langword="false"/>, which makes the compiler
        /// skip the rest of the interpolated string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AppendLiteral(string value) => Builder.TryAppend(value.AsSpan());

        /// <summary>
        /// Appends a formatted value to the handler.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns><see langword="true"/> if it fit; otherwise, <see langword="false"/>, which makes the compiler
        /// skip the rest of the interpolated string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AppendFormatted<T>(T value) => Builder.TryAppendFormatted(value, default);

        /// <summary>
        /// Appends a formatted value to the handler.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns><see langword="true"/> if it fit; otherwise, <see langword="false"/>, which makes the compiler
        /// skip the rest of the interpolated string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AppendFormatted<T>(T value, string? format) => AppendFormatted(value, format.AsSpan());

        /// <summary>
        /// Appends a span to the handler.
        /// </summary>
        /// <param name="value">The span to append.</param>
        /// <returns><see langword="true"/> if it fit; otherwise, <see langword="false"/>, which makes the compiler
        /// skip the rest of the interpolated string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AppendFormatted(scoped ReadOnlySpan<char> value) => Builder.TryAppend(value);

        /// <summary>
        /// Appends a string to the handler.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns><see langword="true"/> if it fit; otherwise, <see langword="false"/>, which makes the compiler
        /// skip the rest of the interpolated string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AppendFormatted(string? value) => Builder.TryAppend(value.AsSpan());

        private bool AppendFormatted<T>(T value, scoped ReadOnlySpan<char> format)
            => Builder.TryAppendFormatted(value, format);
    }
}
