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
    public void ShouldAppendBoolInInterpolatedString()
    {
        using var builder = new ValueStringBuilder();

        builder.Append($"Flag: {true}, {false}");

        builder.ToString().ShouldBe("Flag: True, False");
    }

    [Fact]
    public void ShouldAppendDecimalWithFormatInInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        var amount = 1.5m;

        builder.Append($"Amount: {amount:F3}");

        builder.ToString().ShouldBe("Amount: 1.500");
    }

    [Fact]
    public void ShouldAppendDateTimeInInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.Append($"Date: {date}");

        builder.ToString().ShouldBe($"Date: {date}");
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

    [Fact]
    public void ShouldAppendNoHoleInterpolatedString()
    {
        using var builder = new ValueStringBuilder();

        // Regression guard: a no-hole interpolated string is a constant that converts to both
        // ReadOnlySpan<char> and AppendInterpolatedStringHandler. On C# 13 and earlier this was
        // ambiguous (CS0121) because ValueStringBuilder had no Append(string) overload.
        builder.Append($"Test1\n");

        builder.ToString().ShouldBe("Test1\n");
    }

    [Fact]
    public void ShouldAppendLineNoHoleInterpolatedString()
    {
        using var builder = new ValueStringBuilder();

        builder.AppendLine($"NoHole");

        builder.ToString().ShouldBe($"NoHole{Environment.NewLine}");
    }

    [Fact]
    public void ShouldAppendNoHoleVerbatimInterpolatedString()
    {
        using var builder = new ValueStringBuilder();

        builder.Append($@"C:\temp");

        builder.ToString().ShouldBe(@"C:\temp");
    }

    [Fact]
    public void ShouldAppendNoHoleRawInterpolatedString()
    {
        using var builder = new ValueStringBuilder();

        builder.Append($"""raw""");

        builder.ToString().ShouldBe("raw");
    }

    [Fact]
    public void ShouldAlignInterpolatedValue()
    {
        using var builder = new ValueStringBuilder();
        var value = 42;

        builder.Append($"[{value,5}|{value,-5}|{value,1}]");

        builder.ToString().ShouldBe("[   42|42   |42]");
    }

    [Fact]
    public void ShouldAlignFormattedInterpolatedValue()
    {
        using var builder = new ValueStringBuilder();
        var price = 1.2345;

        builder.Append($"[{price,8:F2}]");

        builder.ToString().ShouldBe("[    1.23]");
    }

    [Fact]
    public void ShouldAlignStringAndSpanInInterpolatedString()
    {
        using var builder = new ValueStringBuilder();
        var name = "ab";
        ReadOnlySpan<char> span = "cd";

        builder.Append($"[{name,-4}|{span,4}]");

        builder.ToString().ShouldBe("[ab  |  cd]");
    }

    [Fact]
    public void ShouldAlignWhenBufferMustGrow()
    {
        using var builder = new ValueStringBuilder(stackalloc char[2]);
        var value = 7;

        builder.Append($"{value,40}");

        builder.ToString().ShouldBe(new string(' ', 39) + "7");
    }

    [Fact]
    public void ShouldAppendCustomSpanFormattableStruct()
    {
        using var builder = new ValueStringBuilder();
        var point = new Point(1, 2);

        builder.Append($"{point} {point:X}");

        builder.ToString().ShouldBe("(1,2) X(1,2)");
    }

    private readonly record struct Point(int X, int Y) : ISpanFormattable
    {
        public override string ToString() => ToString(null, null);

        public string ToString(string? format, IFormatProvider? formatProvider) => $"{format}({X},{Y})";

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => destination.TryWrite(provider, $"{format}({X},{Y})", out charsWritten);
    }

    private class CustomType
    {
        public string Value { get; set; } = string.Empty;

        public override string ToString() => Value;
    }
}
