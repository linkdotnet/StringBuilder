using System;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderInsertTests
{
    [Fact]
    public void ShouldInsertString()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello World");

        valueStringBuilder.Insert(6, "dear ");

        valueStringBuilder.ToString().Should().Be("Hello dear World");
    }

    [Fact]
    public void ShouldInsertWhenEmpty()
    {
        var valueStringBuilder = new ValueStringBuilder();

        valueStringBuilder.Insert(0, "Hello");

        valueStringBuilder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void ShouldAppendFloat()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, 2.2f);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDouble()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, 2.2d);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDecimal()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, 2.2m);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendInteger()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, -2);

        builder.ToString().Should().Be("-2");
    }

    [Fact]
    public void ShouldAppendUnsignedInteger()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, 2U);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendLong()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, 2L);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendChar()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, '2');

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendShort()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, (short)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendBool()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, true);

        builder.ToString().Should().Be("True");
    }

    [Fact]
    public void ShouldAppendByte()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, (byte)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendSignedByte()
    {
        var builder = new ValueStringBuilder();

        builder.Insert(0, (sbyte)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldThrowWhenIndexIsNegative()
    {
        var builder = new ValueStringBuilder();

        try
        {
            builder.Insert(-1, "Hello");
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldThrowWhenIndexIsBehindBufferLength()
    {
        var builder = new ValueStringBuilder();

        try
        {
            builder.Insert(1, "Hello");
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldThrowWhenIndexIsNegativeForFormattableSpan()
    {
        var builder = new ValueStringBuilder();

        try
        {
            builder.Insert(-1, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldThrowWhenIndexIsBehindBufferLengthForFormattableSpan()
    {
        var builder = new ValueStringBuilder();

        try
        {
            builder.Insert(1, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }
}