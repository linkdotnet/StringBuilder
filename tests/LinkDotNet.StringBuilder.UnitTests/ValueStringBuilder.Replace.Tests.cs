namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderReplaceTests
{
    [Fact]
    public void ShouldReplaceAllCharacters()
    {
        var builder = new ValueStringBuilder();
        builder.Append("CCCC");

        builder.Replace('C', 'B');

        builder.ToString().Should().Be("BBBB");
    }

    [Fact]
    public void ShouldReplaceAllCharactersInGivenSpan()
    {
        var builder = new ValueStringBuilder();
        builder.Append("CCCC");

        builder.Replace('C', 'B', 1, 2);

        builder.ToString().Should().Be("CBBC");
    }

    [Fact]
    public void ShouldReplaceAllText()
    {
        var builder = new ValueStringBuilder();
        builder.Append("Hello World. How are you doing. Hello world examples are always fun.");

        builder.Replace("Hello", "Hallöchen");

        builder.ToString().Should().Be("Hallöchen World. How are you doing. Hallöchen world examples are always fun.");
    }

    [Theory]
    [InlineData("", "word")]
    [InlineData("word", "")]
    [InlineData("wor", "word")]
    public void ShouldNotReplaceWhenLengthMismatch(string text, string word)
    {
        var builder = new ValueStringBuilder();
        builder.Append(text);

        builder.Replace(word, "Something");

        builder.ToString().Should().Be(text);
    }

    [Fact]
    public void ShouldBeTheSameWhenOldAndNewTheSame()
    {
        var builder = new ValueStringBuilder();
        builder.Append("text");

        builder.Replace("word", "word");

        builder.ToString().Should().Be("text");
    }

    [Fact]
    public void ShouldNotAlterIfNotFound()
    {
        var builder = new ValueStringBuilder();
        builder.Append("Hello");

        builder.Replace("Test", "Not");

        builder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void ShouldReplaceInSpan()
    {
        var builder = new ValueStringBuilder();
        builder.Append("Hello World. How are you doing. Hello world examples are always fun.");

        builder.Replace("Hello", "Hallöchen", 0, 10);

        builder.ToString().Should().Be("Hallöchen World. How are you doing. Hello world examples are always fun.");
    }

    [Fact]
    public void ShouldReplaceISpanFormattable()
    {
        var builder = new ValueStringBuilder();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", 1.2f);

        builder.ToString().Should().Be("1.2");
    }

    [Fact]
    public void ShouldReplaceISpanFormattableSlice()
    {
        var builder = new ValueStringBuilder();
        builder.Append("{0}{0}{0}");

        builder.ReplaceGeneric("{0}", 1, 0, 6);

        builder.ToString().Should().Be("11{0}");
    }

    [Fact]
    public void ShouldReplaceNonISpanFormattable()
    {
        var builder = new ValueStringBuilder();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", default(MyStruct));

        builder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void ShouldReplaceNonISpanFormattableInSlice()
    {
        var builder = new ValueStringBuilder();
        builder.Append("{0}{0}{0}");

        builder.ReplaceGeneric("{0}", default(MyStruct), 0, 6);

        builder.ToString().Should().Be("HelloHello{0}");
    }

    private struct MyStruct
    {
        public override string ToString() => "Hello";
    }
}