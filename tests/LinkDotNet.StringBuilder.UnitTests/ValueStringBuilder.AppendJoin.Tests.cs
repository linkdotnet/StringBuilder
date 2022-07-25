using System;
using System.Collections.Generic;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderAppendJoinTests
{
    [Theory]
    [MemberData(nameof(StringSeparatorTestData))]
    public void ShouldAppendWithStringSeparator(string separator, IEnumerable<string?> values, string expected)
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(separator, values);

        stringBuilder.ToString().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(CharSeparatorTestData))]
    public void ShouldAppendWithCharSeparator(char separator, IEnumerable<string?> values, string expected)
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(separator, values);

        stringBuilder.ToString().Should().Be(expected);
    }

    [Fact]
    public void ShouldAddDataWithStringSeparator()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(",", new object[] { 1, new DateTime(1900, 1, 1) });

        stringBuilder.ToString().Should().Be("1,01/01/1900 00:00:00");
    }

    [Fact]
    public void ShouldAddDataWithCharSeparator()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(',', new object[] { 1, new DateTime(1900, 1, 1) });

        stringBuilder.ToString().Should().Be("1,01/01/1900 00:00:00");
    }

    private static IEnumerable<object[]> StringSeparatorTestData()
    {
        yield return new object[] { ",", new[] { "Hello", "World" }, "Hello,World" };
        yield return new object[] { ",", new[] { "Hello" }, "Hello" };
        yield return new object[] { ",", Array.Empty<string>(), string.Empty };
    }

    private static IEnumerable<object[]> CharSeparatorTestData()
    {
        yield return new object[] { ',', new[] { "Hello", "World" }, "Hello,World" };
        yield return new object[] { ',', new[] { "Hello" }, "Hello" };
        yield return new object[] { ',', Array.Empty<string>(), string.Empty };
    }
}