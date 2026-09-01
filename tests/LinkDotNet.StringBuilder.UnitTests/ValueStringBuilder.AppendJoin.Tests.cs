namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendJoinTests
{
    public static IEnumerable<object[]> StringSeparatorTestData()
    {
        yield return new object[] { ",", new[] { "Hello", "World" }, "Hello,World" };
        yield return new object[] { ",", new[] { "Hello" }, "Hello" };
        yield return new object[] { ",", Array.Empty<string>(), string.Empty };
        yield return new object[] { ",", new string?[] { null }, string.Empty };
    }

    public static IEnumerable<object[]> CharSeparatorTestData()
    {
        yield return new object[] { ',', new[] { "Hello", "World" }, "Hello,World" };
        yield return new object[] { ',', new[] { "Hello" }, "Hello" };
        yield return new object[] { ',', Array.Empty<string>(), string.Empty };
        yield return new object[] { ',', new string?[] { null }, string.Empty };
    }

    [Theory]
    [MemberData(nameof(StringSeparatorTestData))]
    public void ShouldAppendWithStringSeparator(string separator, IEnumerable<string?> values, string expected)
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(separator, values);

        stringBuilder.ToString().ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(CharSeparatorTestData))]
    public void ShouldAppendWithCharSeparator(char separator, IEnumerable<string?> values, string expected)
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(separator, values);

        stringBuilder.ToString().ShouldBe(expected);
    }

    [Fact]
    public void ShouldAddDataWithStringSeparator()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(",", new object[] { 1, 1.05f });

        stringBuilder.ToString().ShouldBe("1,1.05");
    }

    [Fact]
    public void ShouldAddDataWithCharSeparator()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(',', new object[] { 1, 1.05f });

        stringBuilder.ToString().ShouldBe("1,1.05");
    }

    [Fact]
    public void ShouldAppendJoinWithConcreteIntSpanWithoutBoxing()
    {
        using var stringBuilder = new ValueStringBuilder();
        ReadOnlySpan<int> values = [1, 2, 3, 4, 5];

        stringBuilder.AppendJoin(',', values);

        stringBuilder.ToString().ShouldBe("1,2,3,4,5");
    }

    [Fact]
    public void ShouldAppendJoinWithConcreteBoolArrayWithoutBoxing()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(',', new[] { true, false, true });

        stringBuilder.ToString().ShouldBe("True,False,True");
    }

    [Fact]
    public void ShouldAppendJoinWithConcreteCharArrayWithoutBoxing()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin('-', new[] { 'a', 'b', 'c' });

        stringBuilder.ToString().ShouldBe("a-b-c");
    }

    [Fact]
    public void ShouldAppendJoinWithConcreteDateTimeArrayWithoutBoxing()
    {
        using var stringBuilder = new ValueStringBuilder();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        stringBuilder.AppendJoin(',', new[] { date, date });

        stringBuilder.ToString().ShouldBe($"{date:G},{date:G}");
    }

    [Theory]
    [InlineData(byte.MaxValue)]
    [InlineData(sbyte.MinValue)]
    [InlineData(short.MinValue)]
    [InlineData(ushort.MaxValue)]
    [InlineData(uint.MaxValue)]
    [InlineData(long.MinValue)]
    [InlineData(ulong.MaxValue)]
    [InlineData(double.MaxValue)]
    public void ShouldConcatKnownValueTypesTheSameAsToString<T>(T value)
        where T : struct, ISpanFormattable
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(',', new[] { value, value });

        stringBuilder.ToString().ShouldBe($"{value},{value}");
    }

    [Fact]
    public void ShouldAppendJoinWithConcreteDecimalArrayWithoutBoxing()
    {
        using var stringBuilder = new ValueStringBuilder();
        var value = decimal.MaxValue;

        stringBuilder.AppendJoin(',', new[] { value, value });

        stringBuilder.ToString().ShouldBe($"{value},{value}");
    }
}