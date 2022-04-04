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
    /// by this <see cref="ValueStringBuilder"/>.
    /// </returns>
    public static System.Text.StringBuilder ToStringBuilder(ref this ValueStringBuilder builder)
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(builder.AsSpan());
        return stringBuilder;
    }
}