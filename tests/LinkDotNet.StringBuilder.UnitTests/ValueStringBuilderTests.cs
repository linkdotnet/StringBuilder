using System;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderTests
{
    [Fact]
    public void ShouldAddString()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("That is a string");

        stringBuilder.ToString().Should().Be("That is a string");
    }

    [Fact]
    public void ShouldAddMultipleStrings()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("This");
        stringBuilder.Append("is");
        stringBuilder.Append("a");
        stringBuilder.Append("test");

        stringBuilder.ToString().Should().Be("Thisisatest");
    }

    [Fact]
    public void ShouldAddLargeStrings()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append(new string('c', 99));

        stringBuilder.ToString().Should().MatchRegex("[c]{99}");
    }

    [Fact]
    public void ShouldAppendLine()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine("Hello");

        stringBuilder.ToString().Should().Contain("Hello").And.Contain(Environment.NewLine);
    }

    [Fact]
    public void ShouldOnlyAddNewline()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine();

        stringBuilder.ToString().Should().Be(Environment.NewLine);
    }

    [Fact]
    public void ShouldGetIndexIfGiven()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("Hello");

        stringBuilder[2].Should().Be('l');
    }

    [Fact]
    public void ShouldThrowIndexOutOfRangeWhenStringShorterThanIndex()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        try
        {
            _ = stringBuilder[50];
        }
        catch (IndexOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldTryToCopySpan()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");
        var mySpan = new Span<char>(new char[5], 0, 5);

        var result = stringBuilder.TryCopyTo(mySpan);

        result.Should().BeTrue();
        mySpan.ToString().Should().Be("Hello");
    }

    [Fact]
    public void ShouldReturnSpan()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        var output = stringBuilder.AsSpan().ToString();

        output.Should().Be("Hello");
    }

    [Fact]
    public void ShouldReturnLength()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        var length = stringBuilder.Length;

        length.Should().Be(5);
    }

    [Fact]
    public void ShouldClear()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        stringBuilder.Clear();

        stringBuilder.Length.Should().Be(0);
        stringBuilder.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void ShouldReturnEmptyStringWhenInitialized()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.ToString().Should().Be(string.Empty);
    }
}