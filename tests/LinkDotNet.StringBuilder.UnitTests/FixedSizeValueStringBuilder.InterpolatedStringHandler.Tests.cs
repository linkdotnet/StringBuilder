namespace LinkDotNet.StringBuilder.UnitTests;

public class FixedSizeValueStringBuilderInterpolatedStringHandlerTests
{
    [Fact]
    public void ShouldAppendInterpolatedStringWhenItFits()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);

        builder.Append($"ab{42}cd");

        builder.ToString().ShouldBe("ab42cd");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenLiteralsAloneDoNotFit_WhenAppendingInterpolatedString_ThenNothingIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[3]);

        builder.Append($"ab{42}cd");

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenALaterPartDoesNotFit_WhenAppendingInterpolatedString_ThenEarlierPartsAreKept()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[5]);

        builder.Append($"ab{42}cd");

        builder.ToString().ShouldBe("ab42");
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenAHoleDoesNotFit_WhenAppendingInterpolatedString_ThenItIsNotTruncated()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[6]);

        builder.Append($"ab{12345}");

        builder.ToString().ShouldBe("ab");
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldRespectFormatInInterpolatedString()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);

        builder.Append($"{3.5:F2}");

        builder.ToString().ShouldBe(3.5.ToString("F2", null));
    }

    [Fact]
    public void ShouldAppendNonFormattableValue()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);
        var value = new object();

        builder.Append($"{value}");

        builder.ToString().ShouldBe(value.ToString());
    }

    [Fact]
    public void ShouldAppendNullValueAsEmpty()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);
        object? value = null;

        builder.Append($"a{value}b");

        builder.ToString().ShouldBe("ab");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void ShouldAppendLineWithInterpolatedString()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);

        builder.AppendLine($"ab{42}");

        builder.ToString().ShouldBe("ab42" + Environment.NewLine);
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenOverflow_WhenAppendingInterpolatedString_ThenNothingIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("123456789");

        builder.Append($"ab{42}");

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }
}
