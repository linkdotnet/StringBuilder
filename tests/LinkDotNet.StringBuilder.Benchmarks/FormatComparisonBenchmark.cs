using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class FormatComparisonBenchmark
{
    [Benchmark(Baseline = true)]
    public string ValueStringBuilderAppendFormat()
    {
        using var builder = new ValueStringBuilder();
        builder.AppendFormat("{0} is {1} years old and owes {2}", "Alice", 30, 19.99m);
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderInterpolated()
    {
        using var builder = new ValueStringBuilder();
        builder.Append($"{"Alice"} is {30} years old and owes {19.99m}");
        return builder.ToString();
    }
}
