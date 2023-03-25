using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendFormatBenchmark
{
    [Benchmark]
    public string ValueStringBuilderAppendFormat1()
    {
        using var builder = new ValueStringBuilder();
        for (var i = 0; i < 100; i++)
        {
            builder.Append(true);
            builder.Append(false);
            builder.Append(true);
            builder.Append(false);
        }

        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderAppendFormat2()
    {
        using var builder = new ValueStringBuilder();
        for (var i = 0; i < 100; i++)
        {
            builder.Append(true);
            builder.Append(false);
            builder.Append(true);
            builder.Append(false);
        }

        return builder.ToString();
    }
}