using System;
using System.Collections.Generic;

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

        stringBuilder.AppendJoin(",", new object[] { 1, 1.05f });

        stringBuilder.ToString().Should().Be("1,1.05");
    }

    [Fact]
    public void ShouldAddDataWithCharSeparator()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendJoin(',', new object[] { 1, 1.05f });

        stringBuilder.ToString().Should().Be("1,1.05");
    }
}