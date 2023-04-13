using System;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendTests
{
    [Fact]
    public void ShouldAddString()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("That is a string");

        stringBuilder.ToString().Should().Be("That is a string");
    }

    [Fact]
    public void ShouldAddMultipleStrings()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("This");
        stringBuilder.Append("is");
        stringBuilder.Append("a");
        stringBuilder.Append("test");

        stringBuilder.ToString().Should().Be("Thisisatest");
    }

    [Fact]
    public void ShouldAddLargeStrings()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append(new string('c', 99));

        stringBuilder.ToString().Should().MatchRegex("[c]{99}");
    }

    [Fact]
    public void ShouldAppendLine()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine("Hello");

        stringBuilder.ToString().Should().Contain("Hello").And.Contain(Environment.NewLine);
    }

    [Fact]
    public void ShouldOnlyAddNewline()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine();

        stringBuilder.ToString().Should().Be(Environment.NewLine);
    }

    [Fact]
    public void ShouldGetIndexIfGiven()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("Hello");

        stringBuilder[2].Should().Be('l');
    }

    [Fact]
    public void ShouldAppendSpanFormattable()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2.2f);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendMultipleChars()
    {
        using var builder = new ValueStringBuilder();

        for (var i = 0; i < 64; i++)
        {
            builder.Append('c');
        }

        builder.ToString().Should().MatchRegex("[c]{64}");
    }

    [Fact]
    public void ShouldAppendMultipleDoubles()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(1d / 3d);
        builder.Append(1d / 3d);
        builder.Append(1d / 3d);

        builder.ToString().Should().Be("0.33333333333333330.33333333333333330.3333333333333333");
    }

    [Fact]
    public void ShouldAppendGuid()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(Guid.Empty);

        builder.ToString().Should().Be("00000000-0000-0000-0000-000000000000");
    }

    [Fact]
    public void ShouldThrowIfNotAppendable()
    {
        using var builder = new ValueStringBuilder();

        try
        {
            builder.Append(Guid.Empty, bufferSize: 1);
        }
        catch (InvalidOperationException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Theory]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    public void ShouldAppendBoolean(bool value, string expected)
    {
        using var builder = new ValueStringBuilder();

        builder.Append(value);

        builder.ToString().Should().Be(expected);
    }

    [Fact]
    public unsafe void ShouldAddCharPointer()
    {
        using var builder = new ValueStringBuilder();
        const string text = "Hello World";

        fixed (char* pText = text)
        {
            builder.Append(pText, 5);
        }

        builder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void GivenMemorySlice_ShouldAppend()
    {
        using var builder = new ValueStringBuilder();
        var memory = new Memory<char>(new char[100]);
        var slice = memory[..5];
        slice.Span.Fill('c');

        builder.Append(slice);

        builder.ToString().Should().Be("ccccc");
    }

    [Fact]
    public void GivenAStringWithWhitespace_WhenTrimIsCalled_ThenTheStringShouldBeTrimmed()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("  Hello World  ");

        builder.Trim();

        builder.ToString().Should().Be("Hello World");
    }
}