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
    public void ShouldReplaceGenericWithDecimal()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", 1.5m);

        builder.ToString().ShouldBe("1.5");
    }

    [Fact]
    public void ShouldReplaceGenericWithDateTime()
    {
        using var builder = new ValueStringBuilder();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", date);

        builder.ToString().ShouldBe($"{date}");
    }

    [Fact]
    public void ShouldReplaceGenericWithGuid()
    {
        using var builder = new ValueStringBuilder();
        var guid = Guid.NewGuid();
        builder.Append("{0}");

        builder.ReplaceGeneric("{0}", guid);

        builder.ToString().ShouldBe(guid.ToString());
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

    [Theory]
    [InlineData("ab-ab-ab-ab", "ab", "replacement")]
    [InlineData("replacement-replacement-replacement", "replacement", "x")]
    [InlineData("abcabcabc", "abc", "")]
    [InlineData("aaa", "aa", "bbbb")]
    public void ShouldMatchStringBuilderForLengthChangingReplacements(string text, string oldValue, string newValue)
    {
        var expected = new System.Text.StringBuilder(text)
            .Replace(oldValue, newValue)
            .ToString();
        using var builder = new ValueStringBuilder(text);

        builder.Replace(oldValue, newValue);

        builder.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("xabababz", "ab", "replacement", 1, 4)]
    [InlineData("xreplacementreplacementz", "replacement", "ab", 1, 22)]
    [InlineData("xabababz", "ab", "", 1, 4)]
    [InlineData("aaaa", "aa", "bbbb", 0, 3)]
    public void ShouldMatchStringBuilderForPartialLengthChangingReplacements(string text, string oldValue, string newValue, int startIndex, int count)
    {
        var expected = new System.Text.StringBuilder(text)
            .Replace(oldValue, newValue, startIndex, count)
            .ToString();
        using var builder = new ValueStringBuilder(text);

        builder.Replace(oldValue, newValue, startIndex, count);

        builder.ToString().ShouldBe(expected);
    }

    [Fact]
    public void ShouldReplaceDenseMatchesWhenGrowingFromStackBuffer()
    {
        const string oldValue = "ab";
        const string newValue = "replacement";
        var text = string.Concat(Enumerable.Repeat("ab-", 100));
        var expected = new System.Text.StringBuilder(text)
            .Replace(oldValue, newValue)
            .ToString();
        Span<char> initialBuffer = stackalloc char[32];
        using var builder = new ValueStringBuilder(initialBuffer);
        builder.Append(text);

        builder.Replace(oldValue, newValue);

        builder.ToString().ShouldBe(expected);
    }

    private struct MyStruct
    {
        public override string ToString() => "Hello";
    }
}