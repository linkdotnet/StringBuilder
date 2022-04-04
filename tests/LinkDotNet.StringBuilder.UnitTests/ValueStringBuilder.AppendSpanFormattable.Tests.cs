namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendFormattableTests
{
    [Fact]
    public void ShouldAppendFloat()
    {
        var builder = new ValueStringBuilder();

        builder.Append(2.2f);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDouble()
    {
        var builder = new ValueStringBuilder();

        builder.Append(2.2d);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendDecimal()
    {
        var builder = new ValueStringBuilder();

        builder.Append(2.2m);

        builder.ToString().Should().Be("2.2");
    }

    [Fact]
    public void ShouldAppendInteger()
    {
        var builder = new ValueStringBuilder();

        builder.Append(-2);

        builder.ToString().Should().Be("-2");
    }

    [Fact]
    public void ShouldAppendUnsignedInteger()
    {
        var builder = new ValueStringBuilder();

        builder.Append(2U);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendLong()
    {
        var builder = new ValueStringBuilder();

        builder.Append(2L);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendChar()
    {
        var builder = new ValueStringBuilder();

        builder.Append('2');

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendShort()
    {
        var builder = new ValueStringBuilder();

        builder.Append((short)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendBool()
    {
        var builder = new ValueStringBuilder();

        builder.Append(true);

        builder.ToString().Should().Be("True");
    }

    [Fact]
    public void ShouldAppendByte()
    {
        var builder = new ValueStringBuilder();

        builder.Append((byte)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendSignedByte()
    {
        var builder = new ValueStringBuilder();

        builder.Append((sbyte)2);

        builder.ToString().Should().Be("2");
    }

    [Fact]
    public void ShouldAppendMultipleChars()
    {
        var builder = new ValueStringBuilder();

        for (var i = 0; i < 64; i++)
        {
            builder.Append('c');
        }

        builder.ToString().Should().MatchRegex("[c]{64}");
    }

    [Fact]
    public void ShouldAppendMultipleDoubles()
    {
        var builder = new ValueStringBuilder();

        builder.Append(1d / 3d);
        builder.Append(1d / 3d);
        builder.Append(1d / 3d);

        builder.ToString().Should().Be("0.33333333333333330.33333333333333330.3333333333333333");
    }
}