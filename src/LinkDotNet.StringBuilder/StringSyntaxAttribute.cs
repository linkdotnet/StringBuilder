#if NET6_0
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Fake version of the StringSyntaxAttribute, which was introduced in .NET 7.</summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "The sole purpose is to have the same public surface as the class in .NET7 and above.")]
internal sealed class StringSyntaxAttribute : Attribute
{
    /// <summary>The syntax identifier for strings containing composite formats.</summary>
    public const string CompositeFormat = nameof(CompositeFormat);

    /// <summary>
    /// Initializes a new instance of the <see cref="StringSyntaxAttribute"/> class.
    /// </summary>
    public StringSyntaxAttribute(string syntax)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringSyntaxAttribute"/> class.
    /// </summary>
    public StringSyntaxAttribute(string syntax, params object?[] arguments)
    {
    }
}
#endif