namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderExtensionsTests
{
    [Fact]
    public void ShouldConvertToStringBuilder()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello");

        var fromBuilder = valueStringBuilder.ToStringBuilder().ToString();

        fromBuilder.Should().Be("Hello");
    }
}