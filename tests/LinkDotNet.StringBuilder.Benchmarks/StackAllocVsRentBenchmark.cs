using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class StackAllocVsRentBenchmark
{
    [Benchmark(Baseline = true)]
    public string PooledBuffer()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("Hello World");
        return builder.ToString();
    }

    [Benchmark]
    public string StackAllocBuffer()
    {
        using var builder = new ValueStringBuilder(stackalloc char[32]);
        builder.Append("Hello World");
        return builder.ToString();
    }
}
