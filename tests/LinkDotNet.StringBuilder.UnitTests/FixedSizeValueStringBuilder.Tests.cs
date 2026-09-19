using System.Globalization;
using System.Text;

namespace LinkDotNet.StringBuilder.UnitTests;

public class FixedSizeValueStringBuilderTests
{
    [Fact]
    public void ShouldAppendWhenContentFits()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);

        builder.Append("Hello");
        builder.Append(' ');
        builder.Append("World");

        builder.ToString().ShouldBe("Hello World");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void ShouldFillBufferExactlyWithoutOverflowing()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[5]);

        builder.Append("Hello");

        builder.ToString().ShouldBe("Hello");
        builder.Overflowed.ShouldBeFalse();
        builder.Remaining.ShouldBe(0);
    }

    [Fact]
    public void GivenSingleAppendIsTooLarge_WhenAppending_ThenNothingIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

        builder.Append("123456789");

        builder.ToString().ShouldBe(string.Empty);
        builder.Length.ShouldBe(0);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenOverflow_WhenAppendingSomethingThatWouldFit_ThenItIsStillDropped()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

        builder.Append("1234");
        builder.Append("56789");
        builder.Append("!");

        builder.ToString().ShouldBe("1234");
        builder.Overflowed.ShouldBeTrue();
        builder.Remaining.ShouldBe(4);
    }

    [Fact]
    public void GivenOverflow_WhenClearing_ThenBuilderIsReusable()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("123456789");

        builder.Clear();
        builder.Append("Hello");

        builder.ToString().ShouldBe("Hello");
        builder.Overflowed.ShouldBeFalse();
        builder.Length.ShouldBe(5);
    }

    [Fact]
    public void GivenOverflow_WhenClearingOverflow_ThenContentIsKeptAndAppendingContinues()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");
        builder.Append("56789");

        builder.ClearOverflow();
        builder.Append("!");

        builder.ToString().ShouldBe("1234!");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenNoOverflow_WhenClearingOverflow_ThenNothingChanges()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");

        builder.ClearOverflow();

        builder.ToString().ShouldBe("1234");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenFullBuffer_WhenClearingOverflow_ThenNextAppendOverflowsAgain()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[4]);
        builder.Append("1234");
        builder.Append('!');

        builder.ClearOverflow();
        builder.Append('!');

        builder.ToString().ShouldBe("1234");
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenNewLineDoesNotFit_WhenAppendLine_ThenNeitherPartIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[5 + Environment.NewLine.Length - 1]);

        builder.AppendLine("Hello");

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldAppendLineWhenItFits()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[5 + Environment.NewLine.Length]);

        builder.AppendLine("Hello");

        builder.ToString().ShouldBe("Hello" + Environment.NewLine);
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenSurrogatePairDoesNotFit_WhenAppendingRune_ThenNoLoneSurrogateIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[2]);
        builder.Append('a');

        builder.Append(new Rune(0x1F600));

        builder.ToString().ShouldBe("a");
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldAppendRuneWhenItFits()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[2]);

        builder.Append(new Rune(0x1F600));

        builder.ToString().ShouldBe("\U0001F600");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenFormattedValueDoesNotFit_WhenAppending_ThenItIsNotTruncated()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[4]);

        builder.Append(12345);

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldAppendFormattableWithFormatAndProvider()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

        builder.Append(3.14159f, "F2", CultureInfo.InvariantCulture);

        builder.ToString().ShouldBe("3.14");
    }

    [Theory]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    public void ShouldAppendBool(bool value, string expected)
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[5]);

        builder.Append(value);

        builder.ToString().ShouldBe(expected);
    }

    [Fact]
    public void GivenBoolDoesNotFit_WhenAppending_ThenNothingIsWritten()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[3]);

        builder.Append(true);

        builder.ToString().ShouldBe(string.Empty);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenEmptyOrNullInput_WhenAppending_ThenNothingOverflows()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[1]);
        builder.Append('x');

        builder.Append(string.Empty);
        builder.Append((string?)null);
        builder.Append(ReadOnlySpan<char>.Empty);

        builder.ToString().ShouldBe("x");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void GivenOverflow_WhenAppendingEmptyString_ThenItStaysOverflowed()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[2]);
        builder.Append("abc");

        builder.Append(string.Empty);

        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenZeroLengthBuffer_WhenAppending_ThenItOverflowsWithoutThrowing()
    {
        var builder = new FixedSizeValueStringBuilder([]);

        builder.Append('a');

        builder.Length.ShouldBe(0);
        builder.Capacity.ShouldBe(0);
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void GivenDefaultInstance_WhenAppending_ThenItOverflowsWithoutThrowing()
    {
        var builder = default(FixedSizeValueStringBuilder);

        builder.Append("Hello");

        builder.ToString().ShouldBe(string.Empty);
        builder.IsEmpty.ShouldBeTrue();
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldExposeOnlyWrittenContent()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");
        builder.Append("56789");

        Span<char> destination = stackalloc char[4];

        builder.AsSpan().ToString().ShouldBe("1234");
        builder.TryCopyTo(destination).ShouldBeTrue();
        destination.ToString().ShouldBe("1234");
        builder[0].ShouldBe('1');
    }

    [Fact]
    public void GivenDestinationIsTooSmall_WhenTryCopyTo_ThenItReturnsFalse()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");

        Span<char> destination = stackalloc char[2];

        builder.TryCopyTo(destination).ShouldBeFalse();
    }

    [Fact]
    public void ShouldReportCapacityAndRemaining()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[10]);

        builder.Append("abc");

        builder.Capacity.ShouldBe(10);
        builder.Length.ShouldBe(3);
        builder.Remaining.ShouldBe(7);
        builder.IsEmpty.ShouldBeFalse();
    }

    [Fact]
    public void ShouldContinueInValueStringBuilderWhenMoved()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");

        using var grown = builder.MoveToValueStringBuilder();
        grown.Append("567890123456");

        grown.ToString().ShouldBe("1234567890123456");
    }

    [Fact]
    public void ShouldKeepContentWithoutCopyWhenMoved()
    {
        Span<char> buffer = stackalloc char[8];
        var builder = new FixedSizeValueStringBuilder(buffer);
        builder.Append("1234");

        using var grown = builder.MoveToValueStringBuilder();

        grown.Length.ShouldBe(4);
        grown.Capacity.ShouldBe(8);
        grown[0].ShouldBe('1');
    }

    [Fact]
    public void ShouldConsumeSourceWhenMoved()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");

        using var grown = builder.MoveToValueStringBuilder();

        builder.Length.ShouldBe(0);
        builder.Capacity.ShouldBe(0);
        builder.Overflowed.ShouldBeTrue();
        builder.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotWriteIntoMovedBufferWhenSourceIsUsedAgain()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("1234");

        using var grown = builder.MoveToValueStringBuilder();
        builder.Append("XYZ");

        grown.ToString().ShouldBe("1234");
    }

    [Fact]
    public void ShouldThrowWhenMovingOverflowedBuilder()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            var builder = new FixedSizeValueStringBuilder(stackalloc char[4]);
            builder.Append("12345");

            using var grown = builder.MoveToValueStringBuilder();
        });
    }

    [Fact]
    public void ShouldMoveDeliberatelyTruncatedBuilderWhenOverflowWasCleared()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[4]);
        builder.Append("12345");
        builder.ClearOverflow();

        using var grown = builder.MoveToValueStringBuilder();
        grown.Append("ab");

        grown.ToString().ShouldBe("ab");
    }

    [Fact]
    public void ShouldIndexOnlyWrittenCharacters()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("a");

        builder[0].ShouldBe('a');
        Should.Throw<IndexOutOfRangeException>(() =>
        {
            var b = new FixedSizeValueStringBuilder(stackalloc char[8]);
            b.Append("a");
            _ = b[1];
        });
    }

    [Fact]
    public void ShouldNotAllocateForInterpolatedValueTypeHoles()
    {
        var guid = Guid.NewGuid();
        Append();

        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        Append();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        allocated.ShouldBe(0);

        void Append()
        {
            var builder = new FixedSizeValueStringBuilder(stackalloc char[128]);
            builder.Append($"{1} {2L} {3.5:F2} {true} {'c'} {guid}");
            builder.Overflowed.ShouldBeFalse();
        }
    }

    [Fact]
    public void ShouldRespectAlignmentInInterpolatedHoles()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[64]);
        var name = "ab";

        builder.Append($"[{42,6}][{name,-5}][{3.5,8:F2}][{'x',3}]");

        builder.ToString().ShouldBe("[    42][ab   ][    3.50][  x]");
        builder.Overflowed.ShouldBeFalse();
    }

    [Fact]
    public void ShouldIgnoreAlignmentSmallerThanTheValue()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);

        builder.Append($"{12345,2}");

        builder.ToString().ShouldBe("12345");
    }

    [Fact]
    public void ShouldDropValueAndPaddingTogetherWhenAlignmentDoesNotFit()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append("12345");

        builder.Append($"{7,6}");

        builder.ToString().ShouldBe("12345");
        builder.Overflowed.ShouldBeTrue();
    }

    [Fact]
    public void ShouldNotAllocateForAlignedInterpolatedHoles()
    {
        Append();

        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        Append();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        allocated.ShouldBe(0);

        void Append()
        {
            var builder = new FixedSizeValueStringBuilder(stackalloc char[64]);
            builder.Append($"{1,8}{2L,-8}{3.5,6:F1}");
            builder.Overflowed.ShouldBeFalse();
        }
    }

    [Fact]
    public void ShouldLetTheOriginalSpanCorruptTheMovedBuilderWhenOwnershipIsNotUnique()
    {
        Span<char> shared = stackalloc char[16];
        var builder = new FixedSizeValueStringBuilder(shared);
        builder.Append("hello");

        using var grown = builder.MoveToValueStringBuilder();
        shared[0] = 'X';

        grown.ToString().ShouldBe("Xello");
    }
}
