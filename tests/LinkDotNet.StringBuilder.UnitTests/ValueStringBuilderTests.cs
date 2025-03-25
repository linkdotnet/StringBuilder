namespace LinkDotNet.StringBuilder.UnitTests;

public class ValueStringBuilderTests
{
    [Fact]
    public void ShouldThrowIndexOutOfRangeWhenStringShorterThanIndex()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        try
        {
            _ = stringBuilder[50];
        }
        catch (IndexOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldTryToCopySpan()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");
        var mySpan = new Span<char>(new char[5], 0, 5);

        var result = stringBuilder.TryCopyTo(mySpan);

        result.ShouldBeTrue();
        mySpan.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldReturnSpan()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        var output = stringBuilder.AsSpan().ToString();

        output.ShouldBe("Hello");
    }

    [Fact]
    public void ShouldReturnLength()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        var length = stringBuilder.Length;

        length.ShouldBe(5);
    }

    [Fact]
    public void ShouldClear()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        stringBuilder.Clear();

        stringBuilder.Length.ShouldBe(0);
        stringBuilder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void ShouldReturnEmptyStringWhenInitialized()
    {
        using var stringBuilder = new ValueStringBuilder();

        stringBuilder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void ShouldRemoveRange()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        stringBuilder.Remove(0, 6);

        stringBuilder.Length.ShouldBe(5);
        stringBuilder.ToString().ShouldBe("World");
    }

    [Theory]
    [InlineData(-1, 2)]
    [InlineData(1, -2)]
    [InlineData(90, 1)]
    public void ShouldThrowExceptionWhenOutOfRangeIndex(int startIndex, int length)
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        try
        {
            stringBuilder.Remove(startIndex, length);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void ShouldNotRemoveEntriesWhenLengthIsEqualToZero()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");

        stringBuilder.Remove(0, 0);

        stringBuilder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public unsafe void ShouldGetPinnableReference()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hey");

        fixed (char* c = stringBuilder)
        {
            c[0].ShouldBe('H');
            c[1].ShouldBe('e');
            c[2].ShouldBe('y');
        }
    }

    [Fact]
    public void ShouldGetIndexOfWord()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        var index = stringBuilder.IndexOf("World");

        index.ShouldBe(6);
    }

    [Fact]
    public void ShouldFindInSubstring()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        var index = stringBuilder.IndexOf("l", 6);

        index.ShouldBe(3);
    }

    [Fact]
    public void ShouldThrowExceptionWhenNegativeStartIndex()
    {
        using var stringBuilder = new ValueStringBuilder();

        try
        {
            stringBuilder.IndexOf("l", -1);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void ShouldThrowExceptionWhenNegativeStartIndexLastIndex()
    {
        using var stringBuilder = new ValueStringBuilder();

        try
        {
            stringBuilder.LastIndexOf("l", -1);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.True(true);
            return;
        }

        Assert.True(false);
    }

    [Fact]
    public void ShouldReturnMinusOneIfNotFound()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        var index = stringBuilder.IndexOf("Mountain");

        index.ShouldBe(-1);
    }

    [Fact]
    public void ShouldReturnZeroIfBothEmpty()
    {
        using var stringBuilder = new ValueStringBuilder();

        var index = stringBuilder.IndexOf(string.Empty);

        index.ShouldBe(0);
    }

    [Fact]
    public void ShouldReturnMinusOneWordIsLongerThanString()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        var index = stringBuilder.IndexOf("Hello World but longer");

        index.ShouldBe(-1);
    }

    [Fact]
    public void ShouldReturnMinusOneIfStringIsEmpty()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append(string.Empty);

        var index = stringBuilder.IndexOf("word");

        index.ShouldBe(-1);
    }

    [Fact]
    public void ShouldSetCapacity()
    {
        using var builder = new ValueStringBuilder();

        builder.EnsureCapacity(128);

        builder.Capacity.ShouldBe(128);
    }

    [Fact]
    public void ShouldNotSetCapacityWhenSmallerThanCurrentString()
    {
        using var builder = new ValueStringBuilder();
        builder.Append(new string('c', 128));

        builder.EnsureCapacity(16);

        builder.Length.ShouldBeGreaterThanOrEqualTo(128);
    }

    [Fact]
    public void ShouldFindLastOccurence()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello Hello");

        var index = builder.LastIndexOf("Hello");

        index.ShouldBe(6);
    }

    [Fact]
    public void ShouldFindLastOccurenceInSlice()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello Hello");

        var index = builder.LastIndexOf("Hello", 6);

        index.ShouldBe(0);
    }

    [Fact]
    public void ShouldFindLastIndex()
    {
        using var builder = new ValueStringBuilder("Hello");

        builder.LastIndexOf("o").ShouldBe(4);
    }

    [Fact]
    public void ShouldReturnZeroWhenEmptyStringInIndexOf()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello");

        var index = builder.IndexOf(string.Empty, 5);

        index.ShouldBe(0);
    }

    [Fact]
    public void ShouldReturnZeroIfBothEmptyLastIndexOf()
    {
        using var stringBuilder = new ValueStringBuilder();

        var index = stringBuilder.LastIndexOf(string.Empty);

        index.ShouldBe(0);
    }

    [Fact]
    public void ShouldReturnZeroWhenEmptyStringInLastIndexOf()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello");

        var index = builder.LastIndexOf(string.Empty, 5);

        index.ShouldBe(0);
    }

    [Theory]
    [InlineData("Hello", true)]
    [InlineData("hello", false)]
    [InlineData("", true)]
    public void ShouldReturnIfStringIsPresent(string word, bool expected)
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello");

        var index = builder.Contains(word);

        index.ShouldBe(expected);
    }

    [Fact]
    public void ShouldUseInitialBuffer()
    {
        Span<char> buffer = stackalloc char[16];
        using var builder = new ValueStringBuilder(buffer);

        builder.Append("Hello");

        builder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void ShouldReturnRentedArrayBuffer()
    {
        var builder = new ValueStringBuilder();

        builder.Append(new string('c', 1024));

        builder.Dispose();
    }

    [Fact]
    public void ShouldConcatStringsTogether()
    {
        var result = ValueStringBuilder.Concat("Hello", " ", "World");

        result.ShouldBe("Hello World");
    }

    [Fact]
    public void ConcatDifferentTypesWithTwoArguments()
    {
        var result = ValueStringBuilder.Concat("Test", 1);

        result.ShouldBe("Test1");
    }

    [Fact]
    public void ConcatDifferentTypesWithThreeArguments()
    {
        var result = ValueStringBuilder.Concat("Test", 1, 2);

        result.ShouldBe("Test12");
    }

    [Fact]
    public void ConcatDifferentTypesWithFourArguments()
    {
        var result = ValueStringBuilder.Concat("Test", 1, 2, 3);

        result.ShouldBe("Test123");
    }

    [Fact]
    public void ConcatDifferentTypesWithFiveArguments()
    {
        var result = ValueStringBuilder.Concat("Test", 1, 2, 3, 4);

        result.ShouldBe("Test1234");
    }

    [Fact]
    public void ShouldAcceptInitialCharBuffer()
    {
        var result = new ValueStringBuilder("Hello World").ToString();

        result.ShouldBe("Hello World");
    }

    [Fact]
    public void ReturnSubstring()
    {
        var result = new ValueStringBuilder("Hello World").ToString(1, 3);

        result.ShouldBe("ell");
    }

    [Fact]
    public void ImplicitCastFromStringToValueStringBuilder()
    {
        using ValueStringBuilder sb = "Hello World";

        sb.ToString().ShouldBe("Hello World");
    }

    [Fact]
    public void ImplicitCastFromReadOnlySpanToValueStringBuilder()
    {
        using ValueStringBuilder sb = "Hello World".AsSpan();

        sb.ToString().ShouldBe("Hello World");
    }

    [Fact]
    public void ConcatArbitraryValues()
    {
        var result = ValueStringBuilder.Concat("Hello", " ", "World");

        result.ShouldBe("Hello World");
    }

    [Fact]
    public void ShouldReturnEmptyStringIfEmptyArray()
    {
        var result = ValueStringBuilder.Concat(Array.Empty<string>());

        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ConcatBooleanWithNumber()
    {
        var result = ValueStringBuilder.Concat(true, 1);

        result.ShouldBe("True1");
    }

    [Theory]
    [InlineData("Hello", true)]
    [InlineData("Hallo", false)]
    public void GivenReadOnlySpan_WhenCallingEquals_ThenReturningWhenEqual(string input, bool expected)
    {
        using var builder = new ValueStringBuilder("Hello");

        var isEqual = builder.Equals(input);

        isEqual.ShouldBe(expected);
    }

    [Fact]
    public void ConcatShouldHandleNullValues()
    {
        string[]? array = null;

        ValueStringBuilder.Concat(array!).ShouldBe(string.Empty);
    }

    [Fact]
    public void ShouldReverseString()
    {
        using var builder = new ValueStringBuilder("Hello");

        builder.Reverse();

        builder.ToString().ShouldBe("olleH");
    }

    [Fact]
    public void GivenAString_WhenCallingToStringWithRange_ThenShouldReturnSubstring()
    {
        using var builder = new ValueStringBuilder("Hello World");

        builder.ToString(1..4).ShouldBe("ell");
    }

    [Fact]
    public void GivenAString_WhenEnumerating_ThenShouldReturnCharacters()
    {
        using var builder = new ValueStringBuilder("Hello World");
        var output = string.Empty;

        foreach (var c in builder)
        {
            output += c;
        }

        output.ShouldBe("Hello World");
    }

    [Fact]
    public void GivenStringBuilder_WhenDisposed_ThenEmptyStringReturned()
    {
        var builder = new ValueStringBuilder("Hello World");

        builder.Dispose();

        builder.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void ShouldInitializeWithCapacity()
    {
        using var builder = new ValueStringBuilder(128);

        builder.Capacity.ShouldBe(128);
    }
}