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
}