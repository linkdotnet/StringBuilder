namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderTrimTests
{
    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData(" Hello World", "Hello World")]
    [InlineData("Hello World ", "Hello World ")]
    [InlineData(" Hello World ", "Hello World ")]
    public void GivenStringWithWhitespaces_WhenTrimStart_ThenShouldRemoveWhitespaces(string input, string expected)
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append(input);

        valueStringBuilder.TrimStart();

        valueStringBuilder.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData(" Hello World", " Hello World")]
    [InlineData("Hello World ", "Hello World")]
    [InlineData(" Hello World ", " Hello World")]
    public void GivenStringWithWhitespaces_WhenTrimEnd_ThenShouldRemoveWhitespaces(string input, string expected)
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append(input);

        valueStringBuilder.TrimEnd();

        valueStringBuilder.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData(" Hello World", "Hello World")]
    [InlineData("Hello World ", "Hello World")]
    [InlineData(" Hello World ", "Hello World")]
    public void GivenStringWithWhitespaces_WhenTrim_ThenShouldRemoveWhitespaces(string input, string expected)
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append(input);

        valueStringBuilder.Trim();

        valueStringBuilder.ToString().ShouldBe(expected);
    }

    [Fact]
    public void GivenString_WhenTrimStartCharacter_ThenShouldRemoveCharacter()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHeeHH");

        valueStringBuilder.TrimStart('H');

        valueStringBuilder.ToString().ShouldBe("eeHH");
    }

    [Fact]
    public void GivenString_WhenTrimEndCharacter_ThenShouldRemoveCharacter()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHeeHH");

        valueStringBuilder.TrimEnd('H');

        valueStringBuilder.ToString().ShouldBe("HHee");
    }

    [Fact]
    public void GivenString_WhenTrimCharacter_ThenShouldRemoveCharacter()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHeeHH");

        valueStringBuilder.Trim('H');

        valueStringBuilder.ToString().ShouldBe("ee");
    }

    [Theory]
    [InlineData(" ")] // no-break space
    [InlineData(" ")] // em space
    [InlineData("　")] // ideographic space
    public void GivenStringWithNonAsciiWhitespace_WhenTrim_ThenShouldRemoveWhitespace(string whitespace)
    {
        var input = whitespace + "Hello World" + whitespace;
        using var valueStringBuilder = new ValueStringBuilder(input);

        valueStringBuilder.Trim();

        valueStringBuilder.ToString().ShouldBe("Hello World");
    }

    [Fact]
    public void GivenBufferExactlyAtCapacity_WhenTrim_ThenShouldNotThrow()
    {
        using var valueStringBuilder = new ValueStringBuilder(stackalloc char[8]);
        valueStringBuilder.Append("  abcd  ");

        valueStringBuilder.Trim();

        valueStringBuilder.ToString().ShouldBe("abcd");
    }

    [Fact]
    public void GivenBufferExactlyAtCapacity_WhenTrimStart_ThenShouldNotThrow()
    {
        using var valueStringBuilder = new ValueStringBuilder(stackalloc char[8]);
        valueStringBuilder.Append("    abcd");

        valueStringBuilder.TrimStart();

        valueStringBuilder.ToString().ShouldBe("abcd");
    }

    [Fact]
    public void GivenBufferExactlyAtCapacity_WhenTrimCharacter_ThenShouldNotThrow()
    {
        using var valueStringBuilder = new ValueStringBuilder(stackalloc char[8]);
        valueStringBuilder.Append("HHabcdHH");

        valueStringBuilder.Trim('H');

        valueStringBuilder.ToString().ShouldBe("abcd");
    }

    [Fact]
    public void GivenBufferExactlyAtCapacity_WhenTrimStartCharacter_ThenShouldNotThrow()
    {
        using var valueStringBuilder = new ValueStringBuilder(stackalloc char[8]);
        valueStringBuilder.Append("HHHHabcd");

        valueStringBuilder.TrimStart('H');

        valueStringBuilder.ToString().ShouldBe("abcd");
    }

    [Fact]
    public void GivenStringOfOnlyTrimCharacter_WhenTrim_ThenShouldBeEmpty()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHHHHH");

        valueStringBuilder.Trim('H');

        valueStringBuilder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void GivenStringOfOnlyTrimCharacter_WhenTrimStart_ThenShouldBeEmpty()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHHHHH");

        valueStringBuilder.TrimStart('H');

        valueStringBuilder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void GivenStringOfOnlyTrimCharacter_WhenTrimEnd_ThenShouldBeEmpty()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("HHHHHH");

        valueStringBuilder.TrimEnd('H');

        valueStringBuilder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void GivenString_WhenTrimPrefix_ThenShouldRemoveSpan()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello world");

        valueStringBuilder.TrimPrefix("hell", StringComparison.InvariantCultureIgnoreCase);

        valueStringBuilder.ToString().ShouldBe("o world");
    }

    [Fact]
    public void GivenString_WhenTrimSuffix_ThenShouldRemoveSpan()
    {
        using var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello world");

        valueStringBuilder.TrimSuffix("RlD", StringComparison.InvariantCultureIgnoreCase);

        valueStringBuilder.ToString().ShouldBe("Hello wo");
    }
}