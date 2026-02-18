using System.Globalization;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderInsertTests
{
    [Fact]
    public void ShouldInsertString()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello World");

        valueStringBuilder.Insert(6, "dear ");

        valueStringBuilder.ToString().ShouldBe("Hello dear World");
    }

    [Fact]
    public void ShouldInsertWhenEmpty()
    {
        var valueStringBuilder = new ValueStringBuilder();

        valueStringBuilder.Insert(0, "Hello");

        valueStringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldAppendSpanFormattable()
    {
        using var builder = new ValueStringBuilder();

        builder.Insert(0, 2.2f);

        builder.ToString().ShouldBe("2.2");
    }

    [Fact]
    public void ShouldInsertSpanFormattableWithGivenCulture()
    {
        using var builder = new ValueStringBuilder();

        builder.Insert(0, 1.2m, formatProvider: CultureInfo.GetCultureInfo("de-DE"));

        builder.ToString().ShouldBe("1,2");
    }

    [Fact]
    public void ShouldInsertSpanFormattableWithInvariantCultureWhenFormatProviderIsNull()
    {
        using var builder = new ValueStringBuilder();
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            builder.Insert(0, 1.2m, formatProvider: null);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }

        builder.ToString().ShouldBe("1.2");
    }

    [Fact]
    public void ShouldThrowWhenIndexIsNegative()
    {
        using var builder = new ValueStringBuilder();

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
        using var builder = new ValueStringBuilder();

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
        using var builder = new ValueStringBuilder();

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
        using var builder = new ValueStringBuilder();

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

    [Fact]
    public void ShouldInsertGuid()
    {
        using var builder = new ValueStringBuilder();

        builder.Insert(0, Guid.Empty);

        builder.ToString().ShouldBe("00000000-0000-0000-0000-000000000000");
    }

    [Fact]
    public void ShouldThrowIfNotInsertable()
    {
        using var builder = new ValueStringBuilder();

        try
        {
            builder.Insert(0, Guid.Empty, bufferSize: 1);
        }
        catch (InvalidOperationException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldInsertBool()
    {
        using var builder = new ValueStringBuilder();

        builder.Insert(0, true);

        builder.ToString().ShouldBe("True");
    }
}