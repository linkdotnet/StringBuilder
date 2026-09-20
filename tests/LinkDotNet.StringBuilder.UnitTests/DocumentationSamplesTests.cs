namespace LinkDotNet.StringBuilder.UnitTests;

public class DocumentationSamplesTests
{
    [Fact]
    public void ReadmeValueStringBuilderSampleShouldWork()
    {
        using ValueStringBuilder stringBuilder = new();

        stringBuilder.AppendLine("Hello World");

        string result = stringBuilder.ToString();

        result.ShouldBe($"Hello World{Environment.NewLine}");
    }

    [Fact]
    public void ReadmeConcatSampleShouldWork()
    {
        string result1 = ValueStringBuilder.Concat("Hello ", "World");
        string result2 = ValueStringBuilder.Concat("Hello", 1, 2, 3, "!");

        result1.ShouldBe("Hello World");
        result2.ShouldBe("Hello123!");
    }

    [Fact]
    public void ReadmeFixedSizeSampleShouldWork()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

        builder.Append("123456789");

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GettingStartedSampleShouldWork()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine("Hello World!");
        stringBuilder.Append(0.3f);
        stringBuilder.Insert(6, "dear ");

        stringBuilder.ToString().ShouldBe($"Hello dear World!{Environment.NewLine}0.3");
    }

    [Fact]
    public void ChoosingBuilderFallbackSampleShouldWork()
    {
        const string userName = "Ada";
        const int userId = 42;

        var builder = new FixedSizeValueStringBuilder(stackalloc char[12]);
        builder.Append("id=");
        builder.Append(userId);

        if (builder.Remaining < 8)
        {
            using var grown = builder.MoveToValueStringBuilder();
            grown.Append(" name=");
            grown.Append(userName);

            grown.ToString().ShouldBe("id=42 name=Ada");
            return;
        }

        builder.Append(" name=");
        builder.Append(userName);

        builder.ToString().ShouldBe("id=42 name=Ada");
        builder.Overflowed.ShouldBeFalse();
    }
}
