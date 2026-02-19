using System.Globalization;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendTests
{
    [Fact]
    public void ShouldAddString()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("That is a string");

        stringBuilder.ToString().ShouldBe("That is a string");
    }

    [Fact]
    public void ShouldAddMultipleStrings()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("This");
        stringBuilder.Append("is");
        stringBuilder.Append("a");
        stringBuilder.Append("test");

        stringBuilder.ToString().ShouldBe("Thisisatest");
    }

    [Fact]
    public void ShouldAddLargeStrings()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append(new string('c', 99));

        stringBuilder.ToString().ShouldMatch("[c]{99}");
    }

    [Fact]
    public void ShouldAppendLine()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine("Hello");

        stringBuilder.ToString().ShouldContain("Hello" + Environment.NewLine);
    }

    [Fact]
    public void ShouldAppendSpan()
    {
        using var stringBuilder = new ValueStringBuilder();

        var returned = stringBuilder.AppendSpan(2);

        stringBuilder.Length.ShouldBe(2);

        stringBuilder.ToString().ShouldBe(returned.ToString());
    }

    [Fact]
    public void ShouldOnlyAddNewline()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine();

        stringBuilder.ToString().ShouldBe(Environment.NewLine);
    }

    [Fact]
    public void ShouldGetIndexIfGiven()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.Append("Hello");

        stringBuilder[2].ShouldBe('l');
    }

    [Fact]
    public void ShouldAppendSpanFormattable()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(2.2f);

        builder.ToString().ShouldBe("2.2");
    }

    [Fact]
    public void ShouldAppendSpanFormattableWithGivenCulture()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(1.2m, formatProvider: CultureInfo.GetCultureInfo("de-DE"));

        builder.ToString().ShouldBe("1,2");
    }

    [Fact]
    public void ShouldAppendSpanFormattableWithCurrentCultureWhenFormatProviderIsNull()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        using var builder = new ValueStringBuilder();
        var originalCulture = CultureInfo.CurrentCulture;
        builder.Append(1.2m, formatProvider: null);

        CultureInfo.CurrentCulture = originalCulture;
        builder.ToString().ShouldBe("1,2");
    }

    [Fact]
    public void ShouldAppendMultipleChars()
    {
        using var builder = new ValueStringBuilder();

        for (var i = 0; i < 64; i++)
        {
            builder.Append('c');
        }

        builder.ToString().ShouldMatch("[c]{64}");
    }

    [Fact]
    public void ShouldAppendMultipleDoubles()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(1d / 3d);
        builder.Append(1d / 3d);
        builder.Append(1d / 3d);

        builder.ToString().ShouldBe("0.33333333333333330.33333333333333330.3333333333333333");
    }

    [Fact]
    public void ShouldAppendGuid()
    {
        using var builder = new ValueStringBuilder();

        builder.Append(Guid.Empty);

        builder.ToString().ShouldBe("00000000-0000-0000-0000-000000000000");
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

        builder.ToString().ShouldBe(expected);
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

        builder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void GivenMemorySlice_ShouldAppend()
    {
        using var builder = new ValueStringBuilder();
        var memory = new Memory<char>(new char[100]);
        var slice = memory[..5];
        slice.Span.Fill('c');

        builder.Append(slice);

        builder.ToString().ShouldBe("ccccc");
    }

    [Fact]
    public void GivenAStringWithWhitespace_WhenTrimIsCalled_ThenTheStringShouldBeTrimmed()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("  Hello World  ");

        builder.Trim();

        builder.ToString().ShouldBe("Hello World");
    }

    [Fact]
    public void GivenMultipleValues_WhenCallingAppend_NotCrashing()
    {
        using var builder = new ValueStringBuilder();
        builder.Append(true);
        builder.Append(false);
        builder.Append(true);
        builder.Append(false);
        builder.Append(true);
        builder.Append(false);
        builder.Append(true);

        builder.Append(false);

        builder.ToString().ShouldNotBeNull();
    }

    [Fact]
    public void GivenStringBuilder_WhenAddingSingleCharacter_ThenShouldBeAdded()
    {
        using var builder = new ValueStringBuilder();
        builder.Append('c');

        builder.ToString().ShouldBe("c");
    }

    [Fact]
    public void GivenStringBuilder_WhenAddingIncreasinglyLargerStrings_ThenShouldBeAdded()
    {
        using var builder = new ValueStringBuilder();
        builder.Append(new string('a', 256));
        builder.Append(new string('b', 512));
        builder.Append(new string('c', 1024));
        builder.Append(new string('d', 2048));
        builder.Append(new string('e', 4096));
        builder.Append(new string('f', 8192));

        builder.ToString().ShouldMatch("[a]{256}[b]{512}[c]{1024}[d]{2048}[e]{4096}[f]{8192}");
    }
}