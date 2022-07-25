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
    public void ShouldAppendFloat()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2.2f);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDouble()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2.2d);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDecimal()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2.2m);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendInteger()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(-2);

        builder.ToString().Should().Be("-2");
    }

    [Fact]
    public void ShouldAppendUnsignedInteger()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2U);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendLong()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2L);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendChar()
    {
        using var builder = new ValueStringBuilder();

        builder.Append('2');

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendShort()
    {
        using var builder = new ValueStringBuilder();

        builder.Append((short)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendBool()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(true);

        builder.ToString().Should().Be("True");
    }

    [Fact]
    public void ShouldAppendByte()
    {
        using var builder = new ValueStringBuilder();

        builder.Append((byte)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendSignedByte()
    {
        using var builder = new ValueStringBuilder();

        builder.Append((sbyte)2);

        builder.ToString().Should().Be("2");
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
}