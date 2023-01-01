using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendFormatBenchmark
{
    [Benchmark(Baseline = true)]
    public string DotNetStringBuilderAppendFormat()
    {
        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < 10; i++)
        {
            builder.AppendFormat("Hello {0} dear {1}. {2}", 2, "world", 30);
        }

        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderAppendFormat()
    {
        using var builder = new ValueStringBuilder();
        for (var i = 0; i < 10; i++)
        {
            builder.AppendFormat("Hello {0} dear {1}. {2}", 2, "world", 30);
        }

        return builder.ToString();
    }
}