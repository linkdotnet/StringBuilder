using System;

namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderExtensionsTests
{
    [Fact]
    public void ShouldConvertToStringBuilder()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello");

        var fromBuilder = valueStringBuilder.ToStringBuilder().ToString();

        fromBuilder.ShouldBe("Hello");
    }

    [Fact]
    public void ShouldConvertFromStringBuilder()
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append("Hello");

        var toBuilder = stringBuilder.ToValueStringBuilder();

        toBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldThrowWhenStringBuilderNull()
    {
        System.Text.StringBuilder? sb = null;

        Action act = () => sb!.ToValueStringBuilder();

        act.ShouldThrow<ArgumentNullException>();
    }
}