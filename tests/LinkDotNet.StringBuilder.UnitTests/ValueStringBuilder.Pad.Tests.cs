namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderPadTests
{
    [Fact]
    public void ShouldPadLeft()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadLeft(10, ' ');

        stringBuilder.ToString().Should().Be("     Hello");
    }

    [Fact]
    public void ShouldPadRight()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadRight(10, ' ');

        stringBuilder.ToString().Should().Be("Hello     ");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanCurrentLength_WhenPadLeft_ThenShouldNotChange()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadLeft(3, ' ');

        stringBuilder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanCurrentLength_WhenPadRight_ThenShouldNotChange()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadRight(3, ' ');

        stringBuilder.ToString().Should().Be("Hello");
    }
}