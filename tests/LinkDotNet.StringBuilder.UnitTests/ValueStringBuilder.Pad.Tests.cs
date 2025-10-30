namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderPadTests
{
    [Fact]
    public void ShouldPadLeft()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadLeft(10, ' ');

        stringBuilder.ToString().ShouldBe("     Hello");
    }

    [Fact]
    public void ShouldPadRight()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadRight(10, ' ');

        stringBuilder.ToString().ShouldBe("Hello     ");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanCurrentLength_WhenPadLeft_ThenShouldNotChange()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadLeft(3, ' ');

        stringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanCurrentLength_WhenPadRight_ThenShouldNotChange()
    {
        using var stringBuilder = new ValueStringBuilder("Hello");

        stringBuilder.PadRight(3, ' ');

        stringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldPadLeftWithSource()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadLeft("Hello".AsSpan(), 10, ' ');

        stringBuilder.ToString().ShouldBe("     Hello");
    }

    [Fact]
    public void ShouldPadRightWithSource()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadRight("Hello".AsSpan(), 10, ' ');

        stringBuilder.ToString().ShouldBe("Hello     ");
    }

    [Fact]
    public void ShouldPadLeftWithSourceAndCustomChar()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadLeft("42".AsSpan(), 5, '0');

        stringBuilder.ToString().ShouldBe("00042");
    }

    [Fact]
    public void ShouldPadRightWithSourceAndCustomChar()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadRight("Test".AsSpan(), 8, '*');

        stringBuilder.ToString().ShouldBe("Test****");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanSourceLength_WhenPadLeftWithSource_ThenShouldAppendSourceOnly()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadLeft("Hello".AsSpan(), 3, ' ');

        stringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void GivenTotalWidthIsSmallerThanSourceLength_WhenPadRightWithSource_ThenShouldAppendSourceOnly()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadRight("Hello".AsSpan(), 3, ' ');

        stringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldPadLeftWithSourceMultipleTimes()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadLeft("A".AsSpan(), 3, '-');
        stringBuilder.PadLeft("B".AsSpan(), 3, '-');

        stringBuilder.ToString().ShouldBe("--A--B");
    }

    [Fact]
    public void ShouldPadRightWithSourceMultipleTimes()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.PadRight("A".AsSpan(), 3, '-');
        stringBuilder.PadRight("B".AsSpan(), 3, '-');

        stringBuilder.ToString().ShouldBe("A--B--");
    }

    [Fact]
    public void ShouldPadLeftWithSourceAndExistingContent()
    {
        using var stringBuilder = new ValueStringBuilder("Start ");

        stringBuilder.PadLeft("End".AsSpan(), 10, '.');

        stringBuilder.ToString().ShouldBe("Start .......End");
    }

    [Fact]
    public void ShouldPadRightWithSourceAndExistingContent()
    {
        using var stringBuilder = new ValueStringBuilder("Start ");

        stringBuilder.PadRight("End".AsSpan(), 10, '.');

        stringBuilder.ToString().ShouldBe("Start End.......");
    }
}