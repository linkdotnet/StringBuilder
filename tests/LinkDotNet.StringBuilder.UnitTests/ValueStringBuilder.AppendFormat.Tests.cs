using System;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendFormatTests
{
    [Theory]
    [InlineData("Hello {0}", 2, "Hello 2")]
    [InlineData("{0}{0}", 2, "22")]
    [InlineData("{0} World", "Hello", "Hello World")]
    [InlineData("Hello World", "2", "Hello World")]
    public void ShouldAppendFormatWithOneArgument(string format, object arg, string expected)
    {
        using var builder = new ValueStringBuilder();

        builder.AppendFormat(format, arg);

        builder.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("{0:00}")]
    [InlineData("{1000}")]
    [InlineData("{Text}")]
    public void ShouldThrowWhenFormatWrongOneArgument(string format)
    {
        using var builder = new ValueStringBuilder();

        try
        {
            builder.AppendFormat(format, 1);
        }
        catch (FormatException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Theory]
    [InlineData("Hello {0} {1}", 2, 3, "Hello 2 3")]
    [InlineData("{0}{0}{1}", 2, 3, "223")]
    [InlineData("{0} World", "Hello", "", "Hello World")]
    [InlineData("Hello World", "2", "", "Hello World")]
    public void ShouldAppendFormatWithTwoArguments(string format, object arg1, object arg2, string expected)
    {
        using var builder = new ValueStringBuilder();

        builder.AppendFormat(format, arg1, arg2);

        builder.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello {0} {1} {2}", 2, 3, 4, "Hello 2 3 4")]
    [InlineData("{0}{0}{1}{2}", 2, 3, 3, "2233")]
    [InlineData("{0} World", "Hello", "", "", "Hello World")]
    [InlineData("Hello World", "2", "", "", "Hello World")]
    public void ShouldAppendFormatWithThreeArguments(string format, object arg1, object arg2, object arg3, string expected)
    {
        using var builder = new ValueStringBuilder();

        builder.AppendFormat(format, arg1, arg2, arg3);

        builder.ToString().Should().Be(expected);
    }
}