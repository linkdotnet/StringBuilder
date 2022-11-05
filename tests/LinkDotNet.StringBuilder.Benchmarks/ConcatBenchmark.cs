using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class ConcatBenchmark
{
    [Params("Hello World. How are you? What's going on?")]
    public string SomeString { get; set; } = default!;

    [Params(2000)]
    public int SomeInt { get; set; }

    [Benchmark]
    public string ConcatDotNet() => string.Concat(SomeString, SomeInt, 2d, DateTime.Now, 1f / 3f);

    [Benchmark]
    public string Concat() => ValueStringBuilder.Concat(SomeString, SomeInt, 2d, DateTime.Now, 1f / 3f);
}
