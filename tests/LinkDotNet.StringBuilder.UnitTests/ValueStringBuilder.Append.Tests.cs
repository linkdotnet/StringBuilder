namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendTests
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
}