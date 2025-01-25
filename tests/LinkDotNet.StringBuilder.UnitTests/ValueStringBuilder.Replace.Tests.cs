using System;
using System.Linq;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderReplaceTests
{
    [Fact]
    public void ShouldReplaceAllCharacters()
    {
        using var builder = new ValueStringBuilder(new string('C', 100));

        builder.Replace('C', 'B');

        builder.ToString().ShouldMatch("[B]{100}");
    }

    [Fact]
    public void ShouldReplaceAllCharactersInGivenSpan()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("CCCC");

        builder.Replace('C', 'B', 1, 2);

        builder.ToString().ShouldBe("CBBC");
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    public void ShouldThrowExceptionWhenOutOfRange(int startIndex, int count)
    {
        using var builder = new ValueStringBuilder();

        try
        {
            builder.Replace('a', 'b', startIndex, count);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void ShouldReplaceAllText()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello World. How are you doing. Hello world examples are always fun.");

        builder.Replace("Hello", "Hallöchen");

        builder.ToString().ShouldBe("Hallöchen World. How are you doing. Hallöchen world examples are always fun.");
    }

    [Fact]
    public void ShouldReplacePartThatIsShorter()
    {
        using var builder = new ValueStringBuilder("Hello World");

        builder.Replace("Hello", "Ha");

        builder.ToString().ShouldBe("Ha World");
    }

    [Fact]
    public void ShouldReplacePartThatIsLonger()
    {
        using var builder = new ValueStringBuilder("Hello World");

        builder.Replace("Hello", "Hallöchen");

        builder.ToString().ShouldBe("Hallöchen World");
    }

    [Fact]
    public void ShouldReplacePartThatIsPartiallySimilar()
    {
        using var builder = new ValueStringBuilder("Hello ##Key##");

        builder.Replace("##Key##", "World");

        builder.ToString().ShouldBe("Hello World");
    }

    [Theory]
    [InlineData("", "word")]
    [InlineData("word", "")]
    [InlineData("wor", "word")]
    public void ShouldNotReplaceWhenLengthMismatch(string text, string word)
    {
        using var builder = new ValueStringBuilder();
        builder.Append(text);

        builder.Replace(word, "Something");

        builder.ToString().ShouldBe(text);
    }

    [Fact]
    public void ShouldBeTheSameWhenOldAndNewTheSame()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("text");

        builder.Replace("word", "word");

        builder.ToString().ShouldBe("text");
    }

    [Fact]
    public void ShouldNotAlterIfNotFound()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello");

        builder.Replace("Test", "Not");

        builder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldReplaceInSpan()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello World. How are you doing. Hello world examples are always fun.");

        builder.Replace("Hello", "Hallöchen", 0, 10);

        builder.ToString().ShouldBe("Hallöchen World. How are you doing. Hello world examples are always fun.");
    }

    [Fact]
    public void ShouldReplaceISpanFormattable()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", 1.2f);

        builder.ToString().ShouldBe("1.2");
    }

    [Fact]
    public void ShouldReplaceISpanFormattableSlice()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}{0}{0}");

        builder.ReplaceGeneric("{0}", 1, 0, 6);

        builder.ToString().ShouldBe("11{0}");
    }

    [Fact]
    public void ShouldReplaceNonISpanFormattable()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", default(MyStruct));

        builder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldReplaceNonISpanFormattableInSlice()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}{0}{0}");

        builder.ReplaceGeneric("{0}", default(MyStruct), 0, 6);

        builder.ToString().ShouldBe("HelloHello{0}");
    }

    [Fact]
    public void ShouldReplaceAllOccurrences()
    {
        var content = string.Join(string.Empty, Enumerable.Range(0, 100).Select(_ => "AB"));
        using var builder = new ValueStringBuilder(content);

        builder.Replace("A", "C");

        builder.ToString().ShouldMatch("[CB]{100}");
    }

    private struct MyStruct
    {
        public override string ToString() => "Hello";
    }
}