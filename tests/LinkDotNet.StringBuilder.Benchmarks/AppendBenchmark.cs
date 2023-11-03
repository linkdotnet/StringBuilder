using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendBenchmarks
{
    [Benchmark]
    public string ValueStringBuilder()
    {
        using var builder = new ValueStringBuilder();
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }
}