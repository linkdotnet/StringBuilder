namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderInterpolatedStringTests
{
    [Fact]
    public void ShouldAppendInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        var name = "World";
        var version = 1.0;

        builder.Append($"Hello {name}, version {version}");

        builder.ToString().ShouldBe("Hello World, version 1");
    }

    [Fact]
    public void ShouldAppendInterpolatedStringWithFormat()
    {
        using var builder = new ValueStringBuilder();
        var price = 1.2345;

        builder.Append($"Price: {price:N2}");

        builder.ToString().ShouldBe("Price: 1.23");
    }

    [Fact]
    public void ShouldAppendLineInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        var name = "World";

        builder.AppendLine($"Hello {name}");

        builder.ToString().ShouldBe($"Hello World{Environment.NewLine}");
    }

    [Fact]
    public void ShouldHandleSpanInInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        ReadOnlySpan<char> span = "from span";

        builder.Append($"Value {span}");

        builder.ToString().ShouldBe("Value from span");
    }

    [Fact]
    public void ShouldHandleCustomType()
    {
        using var builder = new ValueStringBuilder();
        var custom = new CustomType { Value = "Test" };

        builder.Append($"Custom: {custom}");

        builder.ToString().ShouldBe("Custom: Test");
    }

    [Fact]
    public void ShouldAppendToExistingContent()
    {
        using var builder = new ValueStringBuilder("Initial ");

        builder.Append($"Appended {123}");

        builder.ToString().ShouldBe("Initial Appended 123");
    }

    [Fact]
    public void ShouldAppendMultipleInterpolatedStrings()
    {
        using var builder = new ValueStringBuilder();

        builder.Append($"First {1} ");
        builder.Append($"Second {2}");

        builder.ToString().ShouldBe("First 1 Second 2");
    }

    [Fact]
    public void ShouldClearAndThenAppendInterpolatedString()
    {
        using var builder = new ValueStringBuilder("Initial");
        builder.Clear();

        builder.Append($"New {1}");

        builder.ToString().ShouldBe("New 1");
    }

    private class CustomType
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString() => Value;
    }
}
