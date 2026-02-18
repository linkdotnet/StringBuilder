namespace LinkDotNet.StringBuilder.UnitTests;

public class FixedSizeValueStringBuilderTests
{
    [Fact]
    public void Append_WithinCapacity_PreservesFullString()
    {
        Span<char> buf = stackalloc char[16];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hello");

        builder.ToString().ShouldBe("Hello");
        builder.Length.ShouldBe(5);
    }

    [Fact]
    public void Append_ExactlyAtCapacity_PreservesFullString()
    {
        Span<char> buf = stackalloc char[5];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hello");

        builder.ToString().ShouldBe("Hello");
        builder.Length.ShouldBe(5);
    }

    [Fact]
    public void Append_ExceedingCapacity_TruncatesToCapacity()
    {
        Span<char> buf = stackalloc char[3];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hello");

        builder.ToString().ShouldBe("Hel");
        builder.Length.ShouldBe(3);
    }

    [Fact]
    public void Append_MultipleCallsThatCollectivelyOverflow_TruncatesAtBoundary()
    {
        Span<char> buf = stackalloc char[7];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hello");  // 5 chars used
        builder.Append(" World"); // only " W" fits (2 remaining)

        builder.ToString().ShouldBe("Hello W");
    }

    [Fact]
    public void Append_WhenAlreadyFull_IsNoOp()
    {
        Span<char> buf = stackalloc char[3];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hey");
        builder.Append("!"); // should be silently ignored

        builder.ToString().ShouldBe("Hey");
    }

    [Fact]
    public void AppendChar_WithinCapacity_AppendsCharacter()
    {
        Span<char> buf = stackalloc char[4];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append('H');
        builder.Append('i');

        builder.ToString().ShouldBe("Hi");
    }

    [Fact]
    public void AppendChar_WhenFull_IsNoOp()
    {
        Span<char> buf = stackalloc char[1];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append('X');
        builder.Append('Y'); // should be ignored

        builder.ToString().ShouldBe("X");
    }

    [Fact]
    public void AppendSpanFormattable_Integer_WithinCapacity()
    {
        Span<char> buf = stackalloc char[16];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append(42);

        builder.ToString().ShouldBe("42");
    }

    [Fact]
    public void AppendSpanFormattable_Integer_TruncatesWhenBufferTooSmall()
    {
        Span<char> buf = stackalloc char[1];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append(123); // only "1" fits

        builder.ToString().ShouldBe("1");
    }

    [Fact]
    public void AppendLine_AddsNewLine()
    {
        Span<char> buf = stackalloc char[32];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.AppendLine("Hello");

        builder.ToString().ShouldBe("Hello" + Environment.NewLine);
    }

    [Fact]
    public void AppendLine_TruncatesNewLineWhenBufferFull()
    {
        Span<char> buf = stackalloc char[5];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.AppendLine("Hello"); // newline won't fit

        builder.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void IsFull_ReturnsTrueWhenCapacityReached()
    {
        Span<char> buf = stackalloc char[3];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.IsFull.ShouldBeFalse();
        builder.Append("Hey");
        builder.IsFull.ShouldBeTrue();
    }

    [Fact]
    public void IsEmpty_ReturnsTrueInitially()
    {
        Span<char> buf = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Clear_ResetsLengthAndAllowsReuse()
    {
        Span<char> buf = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hello");
        builder.Clear();

        builder.Length.ShouldBe(0);
        builder.IsEmpty.ShouldBeTrue();
        builder.Append("Hi");
        builder.ToString().ShouldBe("Hi");
    }

    [Fact]
    public void ZeroCapacityBuffer_AllAppendsAreNoOps()
    {
        var builder = new FixedSizeValueStringBuilder(Span<char>.Empty);

        builder.Append("Hello");
        builder.Append('!');

        builder.ToString().ShouldBe(string.Empty);
        builder.Length.ShouldBe(0);
    }

    [Fact]
    public void AsSpan_ReturnsOnlyWrittenPortion()
    {
        Span<char> buf = stackalloc char[16];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Append("Hi");

        builder.AsSpan().Length.ShouldBe(2);
        builder.AsSpan().ToString().ShouldBe("Hi");
    }

    [Fact]
    public void TryCopyTo_Succeeds_WhenDestinationLargeEnough()
    {
        Span<char> buf = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buf);
        builder.Append("Hello");
        Span<char> dest = stackalloc char[8];

        var result = builder.TryCopyTo(dest);

        result.ShouldBeTrue();
        dest[..5].ToString().ShouldBe("Hello");
    }

    [Fact]
    public void TryCopyTo_Fails_WhenDestinationTooSmall()
    {
        Span<char> buf = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buf);
        builder.Append("Hello");
        Span<char> dest = stackalloc char[2];

        var result = builder.TryCopyTo(dest);

        result.ShouldBeFalse();
    }

    [Fact]
    public void Capacity_ReflectsBufferSize()
    {
        Span<char> buf = stackalloc char[32];
        var builder = new FixedSizeValueStringBuilder(buf);

        builder.Capacity.ShouldBe(32);
    }

    [Fact]
    public void Indexer_ReturnsCorrectCharacter()
    {
        Span<char> buf = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buf);
        builder.Append("Hello");

        builder[0].ShouldBe('H');
        builder[4].ShouldBe('o');
    }
}
