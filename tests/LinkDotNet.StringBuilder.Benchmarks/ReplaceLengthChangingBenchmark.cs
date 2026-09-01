using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class ReplaceLengthChangingBenchmark
{
    private string text = default!;

    [Params(1, 8, 1_024)]
    public int MatchCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        text = string.Concat(Enumerable.Repeat("ab-", MatchCount)) + new string('x', 3 * (1_024 - MatchCount));
    }

    [Benchmark]
    public string StringBuilderGrowing()
    {
        var builder = new System.Text.StringBuilder(text);
        builder.Replace("ab", "replacement");
        return builder.ToString();
    }

    [Benchmark]
    public string CurrentValueStringBuilderGrowing()
    {
        var builder = new ValueStringBuilder(text);
        ReplaceUsingCurrentAlgorithm(ref builder, "ab", "replacement");
        var result = builder.ToString();
        builder.Dispose();
        return result;
    }

    [Benchmark]
    public string ValueStringBuilderGrowing()
    {
        using var builder = new ValueStringBuilder(text);
        builder.Replace("ab", "replacement");
        return builder.ToString();
    }

    [Benchmark]
    public string StringBuilderShrinking()
    {
        var builder = new System.Text.StringBuilder(text);
        builder.Replace("ab", "x");
        return builder.ToString();
    }

    [Benchmark]
    public string CurrentValueStringBuilderShrinking()
    {
        var builder = new ValueStringBuilder(text);
        ReplaceUsingCurrentAlgorithm(ref builder, "ab", "x");
        var result = builder.ToString();
        builder.Dispose();
        return result;
    }

    [Benchmark]
    public string ValueStringBuilderShrinking()
    {
        using var builder = new ValueStringBuilder(text);
        builder.Replace("ab", "x");
        return builder.ToString();
    }

    private static void ReplaceUsingCurrentAlgorithm(ref ValueStringBuilder builder, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
    {
        var index = 0;
        var remainingChars = builder.Length;

        while (remainingChars > 0)
        {
            var foundSubIndex = builder.AsSpan(index, remainingChars).IndexOf(oldValue, StringComparison.Ordinal);
            if (foundSubIndex < 0)
            {
                return;
            }

            index += foundSubIndex;
            remainingChars -= foundSubIndex;

            if (newValue.Length < oldValue.Length)
            {
                builder.Remove(index + newValue.Length, oldValue.Length - newValue.Length);
            }
            else
            {
                builder.Insert(index + oldValue.Length, newValue[oldValue.Length..]);
            }

            for (var i = 0; i < newValue.Length; i++)
            {
                builder[index + i] = newValue[i];
            }

            index += newValue.Length;
            remainingChars -= oldValue.Length;
        }
    }
}
