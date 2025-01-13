namespace LinkDotNet.StringBuilder;

/// <summary>
/// Extension methods for the <see cref="ValueStringBuilder"/>.
/// </summary>
public static class ValueStringBuilderExtensions
{
    /// <summary>
    /// Creates a new <see cref="System.Text.StringBuilder"/> from this <see cref="ValueStringBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder from which the new instance is derived.</param>
    /// <returns>A new <see cref="System.Text.StringBuilder"/> instance with the string represented
    /// by this <paramref name="builder"/>.
    /// </returns>
    public static System.Text.StringBuilder ToStringBuilder(this ValueStringBuilder builder)
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(builder.AsSpan());
        return stringBuilder;
    }

    /// <summary>
    /// Creates a new <see cref="ValueStringBuilder"/> from the given <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The builder from which the new instance is derived.</param>
    /// <returns>A new <see cref="ValueStringBuilder"/> instance with the string represented by this builder.</returns>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="builder"/> is null.</exception>
    public static ValueStringBuilder ToValueStringBuilder(this System.Text.StringBuilder? builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var valueStringBuilder = new ValueStringBuilder();
        foreach (var chunk in builder.GetChunks())
        {
            valueStringBuilder.Append(chunk.Span);
        }

        return valueStringBuilder;
    }
}